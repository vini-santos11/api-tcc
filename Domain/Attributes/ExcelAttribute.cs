using System;

namespace Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelAttribute : Attribute
    {
        public string ColumnTitle { get; set; }
        public bool OnlyDate { get; set; }
        public bool OnlyTime { get; set; }
        public bool IsCurrency { get; set; }
        public bool ThousandSeparator { get; set; }
        public int Decimals { get; set; } = -1;
        public bool HiddenValue { get; set; }
        public string TrueValue { get; set; } = "Sim";
        public string FalseValue { get; set; } = "Não";
    }
}
