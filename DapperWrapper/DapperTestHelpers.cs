namespace DapperWrapper
{
    public static class DapperTestHelpers
    {
        public static bool HasPropertyWithValue<T>(string propertyName, object actual, T expectedValue)
        {
            var property = actual.GetType().GetProperty(propertyName);

            if (property == null) return false;

            var actualValue = (T)property.GetValue(actual, null);

            return expectedValue.Equals(actualValue);
        }
    }
}
