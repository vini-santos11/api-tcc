namespace Cross.Cutting.IoC
{
    public static class StringExtension
    {
        public static long? ToNullableInt32(this string value)
        {
            if (int.TryParse(value, out var newValue)) 
                return newValue;

            return null;
        }
    }
}
