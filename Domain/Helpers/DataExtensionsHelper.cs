using System;

namespace BenMais.Domain.Helpers
{
    public static class DataExtensionsHelper
    {
        private static string SufixStartDate()
        {
            return " 00:00:01";
        }

        private static string SufixEndDate()
        {
            return " 23:59:59";
        }

        public static DateTime? StartOfDay(this DateTime? date)
        {
            if (!date.HasValue)
                return null;

            return DateTime.Parse(date.Value.ToShortDateString().Trim() + SufixStartDate());
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return DateTime.Parse(date.ToShortDateString().Trim() + SufixEndDate());
        }

        public static DateTime? EndOfDay(this DateTime? date)
        {
            if (!date.HasValue)
                return null;

            return DateTime.Parse(date.Value.ToShortDateString().Trim() + SufixEndDate());
        }

        public static DateTime? EndOfDay(this DateTime? date, DateTime defaultDate)
        {
            return DateTime.Parse(date.GetValueOrDefault(defaultDate).ToShortDateString().Trim() + SufixEndDate());
        }
    }
}