using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Attributes;
using Domain.Exceptions;
using Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ap = DocumentFormat.OpenXml.ExtendedProperties;
using Vt = DocumentFormat.OpenXml.VariantTypes;

namespace Domain.Exports.Excel.Base
{
    public abstract class ExcelExport<T>
    {
        // Do not use 0U because it is used for the default cell format.
        private readonly UInt32Value _cellTitle = 1U;
        private readonly UInt32Value _cellString = 2U;
        private readonly UInt32Value _cellDate = 3U;
        private readonly UInt32Value _cellTime = 4U;
        private readonly UInt32Value _cellDateTime = 5U;
        private readonly UInt32Value _cellNumber = 6U;

        private readonly UInt32Value _formatString = 0U;
        private readonly UInt32Value _formatDate = 14U;
        private readonly UInt32Value _formatTime = 20U;
        private readonly UInt32Value _formatDateTime = 164U;
        private readonly UInt32Value _formatNumber = 165U;

        private readonly char[] _chars = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (Char)i).ToArray();
        private readonly IDictionary<int, double> _columnsSize = new Dictionary<int, double>();

        private readonly string _extension = ".xlsx";
        private readonly bool _incDateFileName;
        private readonly double _fontSize = 11;
        private long _currentRow = 0L;

        private IDictionary<PropertyInfo, ExcelAttribute> PropertiesInfo { get; }

        protected string FileExtension { get => _extension; }

        public string FileName { get => PrefixFileName() + (_incDateFileName ? '_' + DateToText(DateTime.Now) : "") + _extension; }

        public string ContentType { get => ContentTypeHelper.ByFileExtension(FileName); }

        protected abstract Task<IEnumerable<T>> DataAsync();

        public abstract string PrefixFileName();

        protected ExcelExport(bool incDateFileName)
        {
            _incDateFileName = incDateFileName;
            PropertiesInfo = typeof(T).GetProperties().
                Where(p => p.GetCustomAttributes<ExcelAttribute>(false).Any()).OrderBy(x => x.DeclaringType.Name).
                ToDictionary(p => p, p => p.GetCustomAttributes<ExcelAttribute>(false).First());
        }

        private static string DateToText(DateTime? date)
        {
            if (!date.HasValue)
                return string.Empty;

            return date.Value.ToString("dd-MM-yyyy");
        }

        private void WriteColumn(OpenXmlWriter openXmlWriter)
        {
            for (int i = 1; i <= PropertiesInfo.Count; i++)
                openXmlWriter.WriteElement(new Column() { Min = (UInt32)i, Max = (UInt32)i, CustomWidth = true, Width = 0 });
        }

        private void ResizeColumns(Worksheet worksheet)
        {
            var columns = worksheet.Elements<Columns>().First();

            foreach (var item in _columnsSize)
            {
                var width = Math.Truncate((item.Value / 10) * _fontSize);
                columns.Append(new Column() { BestFit = true, Min = (UInt32)(item.Key + 1), Max = (UInt32)(item.Key + 1), CustomWidth = true, Width = (DoubleValue)width });
            }
        }

        private static string HiddenValue(string value)
        {
            var options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            var regex = new Regex(@"[a-b0-9]", options);
            return regex.Replace(value, @"*");
        }

        private static string FormatValue(string value, bool hidden)
        {
            if ((string.IsNullOrEmpty(value)) || (!hidden))
                return value;

            return HiddenValue(value);
        }

        private void CalcColumnWidth(int indexColumn, string text, CellValues cellValues, UInt32Value cellStyle)
        {
            var textSize = text.Length;
            if (cellStyle == _cellDate)
                textSize = 10;
            else if (cellStyle == _cellTime)
                textSize = 5;
            else if (cellStyle == _cellDateTime)
                textSize = 16;
            else if (cellValues == CellValues.Number)
            {
                textSize = string.IsNullOrEmpty(text) ? 1 : Math.Truncate(Convert.ToDecimal(text)).ToString().Length;
                var attribute = PropertiesInfo.Values.ElementAt(indexColumn);
                textSize += attribute.Decimals;
            }

            textSize = textSize > 50 ? 50 : textSize + (cellStyle == _cellTitle ? 2 : 0);
            if (!_columnsSize.ContainsKey(indexColumn))
                _columnsSize.Add(indexColumn, textSize);
            else if (textSize > _columnsSize[indexColumn])
                _columnsSize[indexColumn] = textSize;
        }

        #region [ Cells ]
        private Cell GenericCell(int indexColumn, CellValues cellValues, UInt32Value styleIndex, string value, bool hidden)
        {
            var cellValue = new CellValue(FormatValue(value, hidden));

            CalcColumnWidth(indexColumn, cellValue == null ? string.Empty : cellValue.InnerText, cellValues, styleIndex);

            return new Cell
            {
                DataType = new EnumValue<CellValues>(cellValues),
                StyleIndex = styleIndex,
                CellValue = cellValue
            };
        }

        private Cell TitleCell(int indexColumn, ExcelAttribute excelAttribute)
        {
            return GenericCell(indexColumn, CellValues.String, _cellTitle, excelAttribute.ColumnTitle, false);
        }

        private Cell StringCell(int indexColumn, UInt32Value styleIndex, object value, bool hidden)
        {
            return GenericCell(indexColumn, CellValues.String, styleIndex, value == null ? string.Empty : (value as string).Replace("\0", string.Empty).ToString(CultureInfo.InvariantCulture), hidden);
        }

        private Cell DateTimeCell(int indexColum, UInt32Value styleIndex, object value, bool hidden)
        {
            return GenericCell(indexColum, CellValues.Number, styleIndex, value == null ? string.Empty : (value as DateTime?).Value.ToOADate().ToString(CultureInfo.InvariantCulture), hidden);
        }

        private Cell DecimalCell(int indexColumn, UInt32Value styleIndex, object value, bool hidden)
        {
            return GenericCell(indexColumn, CellValues.Number, styleIndex, value == null ? string.Empty : ((value as decimal?).Value).ToString(CultureInfo.InvariantCulture), hidden);
        }

        private Cell IntegerCell(int indexColum, UInt32Value styleIndex, object value, bool hidden)
        {
            return GenericCell(indexColum, CellValues.Number, styleIndex, value == null ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture), hidden);
        }

        private Cell BoolCell(int indexColum, UInt32Value styleIndex, object value, string trueValue, string falseValue, bool hidden)
        {
            return GenericCell(indexColum, CellValues.String, styleIndex, value == null ? string.Empty : ((value as bool?).Value ? trueValue : falseValue).ToString(CultureInfo.InvariantCulture), hidden);
        }

        private Cell ExcelAttributeCell(int indexColumn, ExcelAttribute attribute, object item, IList<ExcelAttribute> numericAttributes)
        {
            if (item is DateTime)
            {
                if (attribute.OnlyDate)
                    return DateTimeCell(indexColumn, _cellDate, item, attribute.HiddenValue);
                else if (attribute.OnlyTime)
                    return DateTimeCell(indexColumn, _cellTime, item, attribute.HiddenValue);
                else
                    return DateTimeCell(indexColumn, _cellDateTime, item, attribute.HiddenValue);
            }
            else if ((item is decimal) || (item is double))
                return DecimalCell(indexColumn, _cellNumber + Convert.ToUInt32(numericAttributes.IndexOf(attribute)), item, attribute.HiddenValue);
            else if ((item is int) || (item is long))
                return IntegerCell(indexColumn, _cellNumber + Convert.ToUInt32(numericAttributes.IndexOf(attribute)), item, attribute.HiddenValue);
            else if (item is bool)
                return BoolCell(indexColumn, _cellString, item, attribute.TrueValue, attribute.FalseValue, attribute.HiddenValue);
            else
                return StringCell(indexColumn, _cellString, item, attribute.HiddenValue);
        }
        #endregion

        private string ColumnLetter(uint index)
        {
            var quotient = index / 26;
            if (quotient > 0)
                return ColumnLetter(quotient) + _chars[index % 26].ToString();
            else
                return _chars[index % 26].ToString();
        }

        private void AutoFilter(OpenXmlWriter openXmlWriter)
        {
            openXmlWriter.WriteElement(new AutoFilter() { Reference = $"{ColumnLetter(0)}:{ColumnLetter((uint)PropertiesInfo.Count - 1)}" });
        }

        private Font Font(Color color, OpenXmlElement fontStyle = null)
        {
            var font = new Font();
            font.Append(new FontSize() { Val = _fontSize });
            font.Append(new FontName() { Val = "Calibri" });
            font.Append(new FontFamilyNumbering() { Val = 2 });
            font.Append(new FontScheme() { Val = FontSchemeValues.Minor });
            font.Append(color);
            if (fontStyle != null)
                font.Append(fontStyle);

            return font;
        }

        private Fonts Fonts()
        {
            var fonts = new Fonts() { KnownFonts = true };
            fonts.Append(Font(new Color() { Theme = 1U }));
            fonts.Append(Font(new Color() { Theme = 0U }, new Bold()));

            return fonts;
        }

        private static Fills Fills()
        {
            var fills = new Fills();

            var fill = new Fill();
            fill.Append(new PatternFill() { PatternType = PatternValues.None });
            fills.Append(fill);

            fill = new Fill();
            fill.Append(new PatternFill() { PatternType = PatternValues.Gray125 });
            fills.Append(fill);

            fill = new Fill();
            var patternFill = new PatternFill() { PatternType = PatternValues.Solid };
            patternFill.Append(new ForegroundColor() { Theme = 9U });
            patternFill.Append(new BackgroundColor() { Indexed = 64U });
            fill.Append(patternFill);
            fills.Append(fill);

            return fills;
        }

        private static Borders Borders()
        {
            var borders = new Borders();
            borders.Append(new Border());

            var border = new Border();
            border.Append(new LeftBorder() { Style = BorderStyleValues.Thin });
            border.Append(new RightBorder() { Style = BorderStyleValues.Thin });
            border.Append(new TopBorder() { Style = BorderStyleValues.Thin });
            border.Append(new BottomBorder() { Style = BorderStyleValues.Thin });
            border.Append(new DiagonalBorder());

            borders.Append(border);

            return borders;
        }

        private IList<ExcelAttribute> NumericAttributes()
        {
            var types = new List<Type> { typeof(int), typeof(long), typeof(decimal), typeof(double), typeof(Nullable<int>), typeof(Nullable<long>), typeof(Nullable<decimal>), typeof(Nullable<double>) };
            var numerics = PropertiesInfo.Where(p => types.Contains(p.Key.PropertyType)).ToDictionary(p => p.Key, p => p.Value);
            return numerics.Values.ToList();
        }

        private NumberingFormats NumberingFormats()
        {
            var numberingFormats = new NumberingFormats();
            numberingFormats.Append(new NumberingFormat() { NumberFormatId = _formatDateTime, FormatCode = "dd/mm/yyyy\\ hh:mm;@" });

            var numericAttributes = NumericAttributes();
            for (var i = 0; i < numericAttributes.Count; i++)
            {
                var attribute = numericAttributes.ElementAt(i);
                var mask = string.Empty;
                if (attribute.IsCurrency)
                    mask += "\"R$\"\\ ";

                if ((attribute.IsCurrency) && (attribute.Decimals == -1))
                    attribute.Decimals = 2;

                mask += (attribute.ThousandSeparator || attribute.IsCurrency) ? "#,##0" : "0";
                if (attribute.Decimals > 0)
                    mask += "." + string.Empty.PadLeft(attribute.Decimals, '0');

                if (!string.IsNullOrEmpty(mask))
                    numberingFormats.Append(new NumberingFormat() { NumberFormatId = _formatNumber + Convert.ToUInt32(i), FormatCode = mask });
            }

            return numberingFormats;
        }

        private CellFormats CellFormats()
        {
            var cellFormats = new CellFormats();
            cellFormats.Append(new CellFormat());
            cellFormats.Append(new CellFormat() { NumberFormatId = _formatString, FontId = 1U, FillId = 2U, BorderId = 1U, FormatId = 0U, ApplyBorder = true, ApplyFont = true, ApplyAlignment = true, ApplyFill = true });
            cellFormats.Append(new CellFormat() { NumberFormatId = _formatString, FontId = 0U, FillId = 0U, BorderId = 1U, FormatId = 0U, ApplyBorder = true, ApplyFont = true, ApplyAlignment = true });
            cellFormats.Append(new CellFormat() { NumberFormatId = _formatDate, FontId = 0U, FillId = 0U, BorderId = 1U, FormatId = 0U, ApplyBorder = true, ApplyFont = true, ApplyAlignment = true, ApplyNumberFormat = true });
            cellFormats.Append(new CellFormat() { NumberFormatId = _formatTime, FontId = 0U, FillId = 0U, BorderId = 1U, FormatId = 0U, ApplyBorder = true, ApplyFont = true, ApplyAlignment = true, ApplyNumberFormat = true });
            cellFormats.Append(new CellFormat() { NumberFormatId = _formatDateTime, FontId = 0U, FillId = 0U, BorderId = 1U, FormatId = 0U, ApplyBorder = true, ApplyFont = true, ApplyAlignment = true, ApplyNumberFormat = true });

            var attributes = NumericAttributes();
            for (var i = 0; i < attributes.Count; i++)
                cellFormats.Append(new CellFormat() { NumberFormatId = new UInt32Value(Convert.ToUInt32(_formatNumber + i)), FontId = 0U, FillId = 0U, BorderId = 1U, FormatId = 0U, ApplyBorder = true, ApplyFont = true, ApplyAlignment = true, ApplyNumberFormat = true });

            return cellFormats;
        }

        private Stylesheet Stylesheet()
        {
            var stylesheet = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            stylesheet.Append(NumberingFormats());
            stylesheet.Append(Fonts());
            stylesheet.Append(Fills());
            stylesheet.Append(Borders());
            stylesheet.Append(CellFormats());

            return stylesheet;
        }

        private void Row(OpenXmlWriter openXmlWriter)
        {
            _currentRow += 1;
            openXmlWriter.WriteStartElement(new Row(), new List<OpenXmlAttribute>
                {
                    new OpenXmlAttribute("r", null, _currentRow.ToString())
                });
        }

        private string TempFile()
        {
            var template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports", "Excel", "Template", $"template{FileExtension}");
            var temp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Guid.NewGuid():D}{FileExtension}");
            File.Copy(template, temp);
            return temp;
        }

        private static void PrepareColumns(WorksheetPart worksheetPart)
        {
            var columns = worksheetPart.RootElement.Descendants<Columns>().FirstOrDefault();
            if (columns == null)
            {
                var sheetData = worksheetPart.RootElement.Descendants<SheetData>().FirstOrDefault();
                columns = worksheetPart.RootElement.InsertBefore<Columns>(new Columns(), sheetData);
            }

            columns.RemoveAllChildren<Column>();
        }

        private WorksheetPart PrepareWorksheetRead(WorkbookPart workbookPart)
        {
            var workbookStylesPart = workbookPart.WorkbookStylesPart;
            workbookStylesPart.Stylesheet = Stylesheet();

            var worksheetPart = workbookPart.WorksheetParts.First();

            PrepareColumns(worksheetPart);
            worksheetPart.Worksheet.Save();

            return worksheetPart;
        }

        private static void PopulateProperties(SpreadsheetDocument spreadsheetDocument)
        {
            spreadsheetDocument.PackageProperties.Creator = "Clube Agro";
            spreadsheetDocument.PackageProperties.Created = DateTime.Now;

            var extendedFilePropertiesPart = spreadsheetDocument.ExtendedFilePropertiesPart;

            var properties1 = new Ap.Properties();
            properties1.AddNamespaceDeclaration("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");

            var headingPairs1 = new Ap.HeadingPairs();

            var vTVector1 = new Vt.VTVector() { BaseType = Vt.VectorBaseValues.Variant, Size = 2U };
            var variant1 = new Vt.Variant();
            var vTLPSTR1 = new Vt.VTLPSTR { Text = "Relatório Excel" };

            variant1.Append(vTLPSTR1);
            var variant2 = new Vt.Variant();
            var vTInt321 = new Vt.VTInt32 { Text = "1" };

            variant2.Append(vTInt321);
            vTVector1.Append(variant1);
            vTVector1.Append(variant2);

            headingPairs1.Append(vTVector1);

            var titlesOfParts1 = new Ap.TitlesOfParts();
            var vTVector2 = new Vt.VTVector() { BaseType = Vt.VectorBaseValues.Lpstr, Size = 1U };
            var vTLPSTR2 = new Vt.VTLPSTR { Text = "Dados" };

            vTVector2.Append(vTLPSTR2);
            titlesOfParts1.Append(vTVector2);

            properties1.Append(new Ap.Application { Text = "Clube Agro Brasil" });
            properties1.Append(new Ap.DocumentSecurity { Text = "0" });
            properties1.Append(new Ap.ScaleCrop { Text = "false" });
            properties1.Append(headingPairs1);
            properties1.Append(titlesOfParts1);
            properties1.Append(new Ap.Company { Text = "Clube Agro" });
            properties1.Append(new Ap.LinksUpToDate { Text = "false" });
            properties1.Append(new Ap.SharedDocument { Text = "false" });
            properties1.Append(new Ap.HyperlinksChanged { Text = "false" });
            properties1.Append(new Ap.ApplicationVersion { Text = "1.0.0" });

            extendedFilePropertiesPart.Properties = properties1;
        }

        private void PopulateTitle(OpenXmlWriter openXmlWriter)
        {
            Row(openXmlWriter);

            for (var i = 0; i < PropertiesInfo.Count; i++)
                openXmlWriter.WriteElement(TitleCell(i, PropertiesInfo.ElementAt(i).Value));

            openXmlWriter.WriteEndElement();
        }

        private async Task PopulateData(OpenXmlWriter openXmlWriter)
        {
            var numericAttributes = NumericAttributes();
            try
            {
                var data = await DataAsync();
                foreach (var item in data)
                {
                    Row(openXmlWriter);

                    for (var i = 0; i < PropertiesInfo.Count; i++)
                    {
                        var propertyInfo = PropertiesInfo.ElementAt(i);
                        openXmlWriter.WriteElement(ExcelAttributeCell(i, propertyInfo.Value, propertyInfo.Key.GetValue(item), numericAttributes));
                    }

                    openXmlWriter.WriteEndElement();
                }
            }
            catch
            {
                throw new ValidateException(Messages.FailureConvertDataToExcel);
            }
        }

        private async Task WriteFile(string filename, string sheetname)
        {
            using var spreadsheetDocument = SpreadsheetDocument.Open(filename, true);
            PopulateProperties(spreadsheetDocument);

            var workbookPart = spreadsheetDocument.WorkbookPart;
            var worksheetRead = PrepareWorksheetRead(workbookPart);
            var worksheetWrite = workbookPart.AddNewPart<WorksheetPart>();

            var openXmlReader = OpenXmlReader.Create(worksheetRead);
            var openXmlWriter = OpenXmlWriter.Create(worksheetWrite);
            try
            {
                while (openXmlReader.Read())
                {
                    if (openXmlReader.ElementType == typeof(SheetData))
                    {
                        if (openXmlReader.IsEndElement)
                            continue;

                        openXmlWriter.WriteStartElement(new SheetData());

                        PopulateTitle(openXmlWriter);

                        await PopulateData(openXmlWriter);

                        openXmlWriter.WriteEndElement();

                        AutoFilter(openXmlWriter);
                    }
                    else if (openXmlReader.ElementType == typeof(Columns))
                    {
                        if (openXmlReader.IsStartElement)
                        {
                            openXmlWriter.WriteStartElement(openXmlReader);

                            WriteColumn(openXmlWriter);
                        }
                        else
                            openXmlWriter.WriteEndElement();
                    }
                    else if (openXmlReader.IsStartElement)
                        openXmlWriter.WriteStartElement(openXmlReader);
                    else if (openXmlReader.IsEndElement)
                        openXmlWriter.WriteEndElement();
                }
            }
            finally
            {
                openXmlReader.Close();
                openXmlWriter.Close();
            }

            var sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Id.Value == workbookPart.GetIdOfPart(worksheetRead)).First();
            sheet.Name = sheetname;
            sheet.Id.Value = workbookPart.GetIdOfPart(worksheetWrite);
            workbookPart.DeletePart(worksheetRead);

            ResizeColumns(worksheetWrite.Worksheet);
            worksheetWrite.Worksheet.Save();
            spreadsheetDocument.Close();
        }

        public async Task<MemoryStream> Export()
        {
            var filename = TempFile();
            try
            {
                await WriteFile(filename, "Dados");

                using var stream = File.OpenRead(filename);
                var memory = new MemoryStream();
                stream.CopyTo(memory);
                memory.Position = 0;

                return memory;
            }
            finally
            {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }
    }
}
