using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Helpers
{
    public static class FormatHelper
    {
        private const string MASK_DOCUMENT = @"000\.000\.000\-00";
        public static string RemoveSpecialCharacter(string value)
        {
            if (value == null)
                return null;

            return Regex.Replace(value, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
        }

        public static string OnlyNumber(string value)
        {
            if (value == null)
                return null;

            return Regex.Replace(value, "[^0-9]+", "", RegexOptions.Compiled);
        }
        public static string Document(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            value = OnlyNumber(value);
            return Convert.ToUInt64(RemoveSpecialCharacter(value).PadLeft(14, '0')).ToString(MASK_DOCUMENT);
        }

        public static string ListToText<T>(IEnumerable<T> lista, string separador = ", ", string ultimoSeparador = null)
        {
            if ((lista == null) || (lista.Count() == 0))
                return string.Empty;

            if (typeof(T).IsEnum)
                return ListToText(lista.Select(i => Convert.ToInt64(i)), separador, ultimoSeparador);

            var texto = string.Join(separador, lista.ToArray());

            var indice = texto.LastIndexOf(separador);
            if (!string.IsNullOrEmpty(ultimoSeparador) && (indice >= 0))
                texto = texto.Substring(0, indice) + ultimoSeparador + texto[(indice + separador.Length)..];

            return texto;
        }
    }
}
