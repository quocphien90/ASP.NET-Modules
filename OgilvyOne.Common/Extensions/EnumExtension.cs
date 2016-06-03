using System;

namespace OgilvyOne.Common.Extensions
{
    public static class EnumExtension
    {
        public static int ToInt(this Enum enumValue)
        {
            //return (int)Convert.ChangeType(enumValue, enumValue.GetTypeCode());
            return Convert.ToInt32(enumValue);
            //return (int)Enum.Parse(enumValue.GetType(), enumValue.ToString());
        }

        /// <summary>
        /// Usage:
        /// Color colorEnum = "Red".ToEnum<Color>();
        /// Or
        /// string color = "Red";
        /// var colorEnum = color.ToEnum<Color>();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumString"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this object enumString)
        {
            return (T)Enum.Parse(typeof(T), enumString.ToString());
        }

        
    }
}
