using System;

namespace Prototype
{
    public class TypeUtils
    {
        public static T TryConvert<T>(string value, T defaultValue = default(T))
        {
            T result;

            if (null == value)
            {
                return defaultValue;
            }

            Type typeT = typeof(T);

            try
            {
                Type underlyingType = Nullable.GetUnderlyingType(typeT);
                
                result = (T)Convert.ChangeType(value, underlyingType ?? typeT);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
                result = defaultValue;
            }

            return result;
        }
    }
}
