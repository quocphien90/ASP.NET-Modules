using System.Globalization;

namespace OgilvyOne.Common.Extensions
{
    public static class NumberExtension
    {

//        public static string ToCurrencyString(this int number, string unit = "")
//        {
//            return ConvertUtility.FormatCurrency(number, unit);
//        }

        public static string ToCurrencyString(this decimal number, bool enableShorten = false, bool showUnit = true, bool enableRound = true)
        {
            string unit = "đ";
            
            string format = "N00";


            if (enableRound == false)
            {
                if (showUnit)
                {
                    return number.ToString("#,##0.00") + unit;
                }
                return number.ToString("#,##0.00");
            }

            if (enableShorten)
            {
                if (number >= 1000000)
                {
                    number = number / (decimal)1000000;
                    unit = "tr";
                    format = "N01";
                }
                else if (number >= 1000)
                {
                    number = number / (decimal)1000;
                    unit = "k";
                    format = "N00";
                }
            }
            
            var currency = number.ToString(format, new CultureInfo("vi-VN"));
          
            if (showUnit == false)
                return currency;

            return string.Format("{0}{1}", currency, unit);

        }

        public static string ToCurrencyString(this decimal? number, bool enableShorten = false, bool showUnit = true, bool enableRound = true)
        {
            return ToCurrencyString(number.GetValueOrDefault(), enableShorten, showUnit, enableRound);
        }

        public static string ToCurrencyString(this int number, bool enableShorten = false, bool showUnit = true)
        {
            return ToCurrencyString((decimal)number, enableShorten, showUnit);
        }

        public static string ToCurrencyString(this long number, bool enableShorten = false, bool showUnit = true)
        {
            return ToCurrencyString((decimal)number, enableShorten, showUnit);
        }
        
        public static string ToCurrencyString(this int? number, bool enableShorten = false, bool showUnit = true)
        {
            return ToCurrencyString((decimal)number.GetValueOrDefault(0), enableShorten, showUnit);
        }

        public static string ToCurrencyString(this long? number, bool enableShorten = false, bool showUnit = true)
        {
            return ToCurrencyString((decimal)number.GetValueOrDefault(0), enableShorten, showUnit);
        }
    }
}
