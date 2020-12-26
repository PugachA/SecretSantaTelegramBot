using System;
using System.Collections.Generic;
using System.Text;

namespace Cryptography.Wrappers
{
    internal static class ArgumentValidationExtensions
    {
        public static string StringNullOrEmptyValidate(this string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"Parameter {parameterName} cannot be null or empty");

            return value;
        }

        public static T NullValidate<T>(this T value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException($"Parameter {parameterName} cannot be null");

            return value;
        }

        public static T[] ArrayNullOrEmptyValidate<T>(this T[] value, string parameterName)
        {
            if (value == null || value.Length <= 0)
                throw new ArgumentException($"Parameter {parameterName} cannot be null or empty");

            return value;
        }

        public static T EnumValidate<T>(this T value, Type enumType) where T : Enum
        {
            if (!Enum.IsDefined(enumType, value))
                throw new ArgumentException($"This enum value={value} not defined in {enumType}");

            return value;
        }
    }
}
