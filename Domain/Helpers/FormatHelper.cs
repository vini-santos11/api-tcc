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
    }
}
