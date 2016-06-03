using System;
using System.Text.RegularExpressions;

namespace OgilvyOne.Common.Extensions
{
    public static class StringExtension
    {
        public static string FormatAsGroup(this string str, int numberOfPart = 4, string separator = ".")
        {
            str = Regex.Replace(str, string.Format("(?!^).{{{0}}}", numberOfPart), string.Format("{0}$0", separator), RegexOptions.RightToLeft);
            return str;

        }

        public static string MakeSortString(this string str, int length)
        {
            if (str.Length < length)
                length = str.Length;
            string s = str.Substring(0, length);
            if (s.Length < str.Length && s.LastIndexOf(' ') > 0)
                s = str.Substring(0, s.LastIndexOf(' '));
            if (str.Length > s.Length)
                s += "...";
            return s;
        }

        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }

        public static int ToInt(this string str, int defaultValue = 0)
        {
            int result;
            int.TryParse(str, out result);
            if (defaultValue != 0 && result == 0)
            {
                result = defaultValue;
            }

            return result;
        }

        public static long ToLong(this string str)
        {
            return long.Parse(str);
        }



        public static string ConverToNumberToString(string number)
        {
            string[] dv = { "", "mươi", "trăm", "nghìn", "triệu", "tỉ" };
            string[] cs = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string doc;
            int i, j, k, n, len, found, ddv, rd;

            len = number.Length;
            number += "ss";
            doc = "";
            found = 0;
            ddv = 0;
            rd = 0;

            i = 0;
            while (i < len)
            {
                //So chu so o hang dang duyet
                n = (len - i + 2) % 3 + 1;

                //Kiem tra so 0
                found = 0;
                for (j = 0; j < n; j++)
                {
                    if (number[i + j] != '0')
                    {
                        found = 1;
                        break;
                    }
                }

                //Duyet n chu so
                if (found == 1)
                {
                    rd = 1;
                    for (j = 0; j < n; j++)
                    {
                        ddv = 1;
                        switch (number[i + j])
                        {
                            case '0':
                                if (n - j == 3) doc += cs[0] + " ";
                                if (n - j == 2)
                                {
                                    if (number[i + j + 1] != '0') doc += "lẻ ";
                                    ddv = 0;
                                }
                                break;
                            case '1':
                                if (n - j == 3) doc += cs[1] + " ";
                                if (n - j == 2)
                                {
                                    doc += "mười ";
                                    ddv = 0;
                                }
                                if (n - j == 1)
                                {
                                    if (i + j == 0) k = 0;
                                    else k = i + j - 1;

                                    if (number[k] != '1' && number[k] != '0')
                                        doc += "mốt ";
                                    else
                                        doc += cs[1] + " ";
                                }
                                break;
                            case '5':
                                if (i + j == len - 1)
                                    doc += "lăm ";
                                else
                                    doc += cs[5] + " ";
                                break;
                            default:
                                doc += cs[(int)number[i + j] - 48] + " ";
                                break;
                        }

                        //Doc don vi nho
                        if (ddv == 1)
                        {
                            doc += dv[n - j - 1] + " ";
                        }
                    }
                }


                //Doc don vi lon
                if (len - i - n > 0)
                {
                    if ((len - i - n) % 9 == 0)
                    {
                        if (rd == 1)
                            for (k = 0; k < (len - i - n) / 9; k++)
                                doc += "tỉ ";
                        rd = 0;
                    }
                    else
                        if (found != 0) doc += dv[((len - i - n + 1) % 9) / 3 + 2] + " ";
                }

                i += n;
            }

            if (len == 1)
                if (number[0] == '0' || number[0] == '5') return cs[(int)number[0] - 48];

            var str = doc.ToCharArray();
            var result = "";
            for (int l = 0; l < str.Length; l++)
            {
                if (l == 0)
                {
                    result += str[l].ToString().ToUpper();
                }
                else
                {
                    result += str[l];
                }
            }
            return result;
        }

        public static string CleanUpForDownload(this string s)
        {
            return s.Replace("..", string.Empty).Replace("/", string.Empty).Replace("\\", string.Empty);
        }

        public static string FormatDateAsPostedAt(this DateTime postedAt)
        {
            var ts = new TimeSpan(DateTime.Now.Ticks - postedAt.Ticks); double delta = Math.Abs(ts.TotalSeconds);
            const int SECOND = 1; const int MINUTE = 60 * SECOND; const int HOUR = 60 * MINUTE; const int DAY = 24 * HOUR; const int MONTH = 30 * DAY;
            if (delta < 0) { return "chưa có"; } if (delta < 1 * MINUTE) { return ts.Seconds == 1 ? "1 giây trước" : ts.Seconds + 1 + " giây trước"; }
            if (delta < 2 * MINUTE) { return "1 phút trước"; } if (delta < 45 * MINUTE) { return ts.Minutes + " phút trước"; }
            if (delta < 90 * MINUTE) { return "1 giờ trước"; } if (delta < 24 * HOUR) { return ts.Hours + " giờ trước"; }
            if (delta < 48 * HOUR) { return "Hôm qua, " + postedAt.ToString("hh:mm"); } if (delta < 30 * DAY) { return ts.Days + " ngày trước"; }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "1 tháng trước" : months + " tháng trước";
            }
            else { int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365)); return years <= 1 ? "1 năm trước" : years + " năm trước"; }
        }
    }
}
