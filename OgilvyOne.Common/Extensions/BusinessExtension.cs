using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Configuration;
using System.Web.Configuration;
using System.Globalization;
using System.Threading;

namespace OgilvyOne.Common.Extensions
{
    public class BusinessExtension
    {
        public static bool IsValidInternalReturnURL(string returnURL)
        {
            if (returnURL.Contains("http:") || returnURL.Contains("https:")
                || returnURL.Contains("www.") || returnURL.Contains(".com")
                || returnURL.Contains("prompt")
                || returnURL.Contains("onmouseover")
                || IsValidSQLInjection(returnURL) == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string GetUniqueNumber()
        {
            return String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);
        }
        public static string get_data_from_url(string url)
        {
            string response_text = "";
            try
            {
                // Ignore certificate errors
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
                objRequest.Method = "GET";
                objRequest.ContentType = "application/x-www-form-urlencoded";

                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    response_text = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception) { }

            return response_text;
        }
        /// <summary>
        /// Post Data to Specific URL
        /// </summary>
        /// <param name="url_post"></param>
        /// <param name="dataString"></param>
        /// <returns></returns>
        public static string Post(string url_post, string dataString)
        {
            string response_text = "";

            byte[] data = Encoding.UTF8.GetBytes(dataString);

            // Ignore certificate errors
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url_post);
            objRequest.Method = "POST";
            objRequest.ContentLength = data.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            Stream dataStream = objRequest.GetRequestStream();
            try
            {
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
            }
            catch (Exception) { }
            finally
            {
                dataStream.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                response_text = sr.ReadToEnd();
                sr.Close();
            }

            return response_text;
        }

        /// <summary>
        /// Encrypt Password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string DoEncryptPassword(string password)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            System.Text.UTF8Encoding encode = new System.Text.UTF8Encoding();
            byte[] result1 = md5.ComputeHash(encode.GetBytes(password));
            string sResult2 = ToHexa(result1);
            return sResult2;
        }
       
        /// <summary>
        /// Count month betweeen 2 months
        /// </summary>
        /// <param name="dtDateSoon"></param>
        /// <param name="dtDateLater"></param>
        /// <returns></returns>
        public static int CountMonthAge(DateTime dtDateSoon, DateTime dtDateLater)
        {
            int month = 0;
            try
            {

                month = (dtDateLater.Year - dtDateSoon.Year) * 12 + dtDateLater.Month - dtDateSoon.Month;

            }
            catch (Exception e)
            {
                return 0;
                throw;
            }
            return month;
        }
        public static int CountAge(DateTime DOB)
        {
            DateTime now = getNow();
            int age = now.Year - DOB.Year;
            if (now < DOB.AddYears(age)) age--;

            return age;
        }

        public static int CountMomPregnantWeek(DateTime BirthDate, DateTime CurrentDate)
        {
            int weeks = 0;

            try
            {
                if (CurrentDate > BirthDate)
                {
                    weeks = 40;
                    return weeks;
                }
                else
                {
                    DateTime MomStartBirth = BirthDate.AddDays(-(42 * 7));

                    int days = (CurrentDate - MomStartBirth).Days;

                    weeks = days / 7;

                    if (weeks == 0)
                        weeks = 1;
                }
            }
            catch (Exception e)
            {
                return 0;
                throw;
            }
            return weeks;
        }

        
        public static String AppConfig(String paramName)
        {
            String tmpS = String.Empty;
            try
            {
                tmpS = ConfigurationSettings.AppSettings[paramName.ToLowerInvariant()].ToString();
            }
            catch
            {
                tmpS = String.Empty;
            }
            return tmpS;
        }
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == ' ' || c == '%')
                { sb.Append(c); }
            }
            return sb.ToString();
        }
        public static DateTime LocalDateTime()
        {
            string TimeZone = AppConfig("TimeZone");

            if (TimeZone != string.Empty)
            {
                return DateTime.UtcNow.AddHours(int.Parse(TimeZone));
            }
            else
            {
                return DateTime.UtcNow.AddHours(7);
            }
        }
        public static DateTime ConvertToLocalDateTime(DateTime dtDate)
        {
            string TimeZone = AppConfig("TimeZone");

            if (TimeZone != string.Empty)
            {
                return dtDate.AddHours(int.Parse(TimeZone));
            }
            else
            {
                return dtDate.AddHours(7);
            }
        }
        public static Int32 CheckBadWord(string file_bad_words, String Str)
        {
            List<string> badWords = BadWordList(file_bad_words);

            if (Str == "" || Str.Length == 0 || badWords.Count == 0) return -1;
            Str = Str.ToLower();

            Int32 indexOf = -1;
            for (Int32 i = 0; i < badWords.Count; i++)
            {
                indexOf = Str.IndexOf(badWords[i].ToLower());
                if (indexOf != -1)
                {
                    break;
                }
            }

            return indexOf;
        }



        public static string GetRemoteAddress()
        {
            //string ip_addr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string ip_addr = HttpContext.Current.Request.UserHostAddress;
            return ip_addr;
        }

        public static List<string> BadWordList(string file)
        {
            //create a new List(T) for holding the words
            List<string> words = new List<string>();

            //create a new XmlDocument, this will read our XML file
            XmlDocument xmlDoc = new XmlDocument();

            //here is where XPath comes into play, when we use
            //SelectNodes we will pass this XPath query into it
            //so we can navigate straight to the nodes we want
            string query = "/WordList/word";

            //now load the XML document
            xmlDoc.Load(file);

            //loop through all the XmlNodes that meet our XPath criteria
            foreach (XmlNode node in xmlDoc.SelectNodes(query))
            {
                //add the InnerText of each ChildNodes we find
                words.Add(node.ChildNodes[0].InnerText);
            }

            //return the populated List(T)
            return words;
        }

        public static string ToHexa(byte[] data)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sb.AppendFormat("{0:X2}", data[i]);
            return sb.ToString();
        }

        public static string DoMD5(string SData)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            System.Text.UTF8Encoding encode = new System.Text.UTF8Encoding();
            byte[] result1 = md5.ComputeHash(encode.GetBytes(SData));
            string sResult2 = ToHexa(result1);
            return sResult2;
        }

        public static void del_cookie(string cookie_name)
        {
            if (HttpContext.Current.Request.Cookies[cookie_name] != null)
            {
                HttpCookie myCookie = new HttpCookie(cookie_name);
                myCookie.Expires = DateTime.Now.AddDays(-100d);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
        }

        public static void send_email_v2(string to, string to_name, string from, string from_name, string subject, string body)
        {
            string domain = GetCurrentDomain();
            //do something to send email
           // string debugerEmails = Dsquare.Configs.Config.Current.EmailEngine.DEBUG_EMAILS;
           // bool isDebug = !string.IsNullOrWhiteSpace(debugerEmails);
         
                Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                MailSettingsSectionGroup mailSettings = configurationFile.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

                string UserName = mailSettings.Smtp.Network.UserName;
                string Password = mailSettings.Smtp.Network.Password;
                string Host = mailSettings.Smtp.Network.Host;
                int iPort = mailSettings.Smtp.Network.Port;

                try
                {
                    MailAddress to_ = new MailAddress(to, to_name);
                    MailAddress from_ = new MailAddress(from, from_name);

                    System.Net.Mail.MailMessage M = new System.Net.Mail.MailMessage(from_, to_);
                    System.Net.Mail.SmtpClient SMTPClient = new System.Net.Mail.SmtpClient(Host, iPort);
                    System.Net.NetworkCredential NTLMAuthentication = new System.Net.NetworkCredential(UserName, Password);

                    M.Subject = subject;
                    M.SubjectEncoding = System.Text.Encoding.UTF8;
                    M.Body = body;
                    M.BodyEncoding = System.Text.Encoding.UTF8;
                    M.IsBodyHtml = true;
                    SMTPClient.UseDefaultCredentials = false;
                    SMTPClient.Credentials = NTLMAuthentication;

                    SMTPClient.Send(M);
                }
                catch (Exception e)
                {
                   
                    System.Web.Mail.SmtpMail.SmtpServer = Host;
                    //System.Net.Mail.SmtpDeliveryMethod.Network. = Host;
                    System.Web.Mail.MailMessage mail = new System.Web.Mail.MailMessage();

                    mail.To = new MailAddress(to, to_name).ToString();
                    mail.From = new MailAddress(from, from_name).ToString();
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.BodyFormat = System.Web.Mail.MailFormat.Html;

                    try
                    {
                        System.Web.Mail.SmtpMail.Send(mail);
                    }
                    catch (Exception exp)
                    {
                        HttpContext.Current.Response.Write(string.Format("Message: {0} <br/> Source: {1} <br/> InnerException: {2} <br/>StackTrace: {3}", exp.Message, exp.Source, exp.InnerException, exp.StackTrace));

                    }
                }
            
           
        }

        public static string ReadFromFile(string filename)
        {
            filename = HttpContext.Current.Server.MapPath(filename);

            string s = "";
            try
            {
                FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(file);
                s = sr.ReadToEnd();
                sr.Close();
                file.Close();
            }
            catch
            {
                s = "";
            }
            return s;
        }

     

        public static string GetRandomUsingGUID(int length)
        {
            // Get the GUID
            string guidResult = System.Guid.NewGuid().ToString();

            // Remove the hyphens
            guidResult = guidResult.Replace("-", string.Empty);

            // Make sure length is valid
            if (length <= 0 || length > guidResult.Length)
                throw new ArgumentException("Length must be between 1 and " + guidResult.Length);

            // Return the first length bytes
            return guidResult.Substring(0, length);
        }

        public static void set_cookie(string param, string value)
        {
            if (Encrypt.Enable)
            {
                param = Encrypt.EncryptData(Encrypt.Key, param, true);
                value = Encrypt.EncryptData(Encrypt.Key, value);
            }
            HttpContext.Current.Response.Cookies[param].Value = value;
            //HttpContext.Current.Response.Cookies[param].Expires = DateTime.Now.AddMinutes(15);
            HttpContext.Current.Response.Cookies[param].HttpOnly = true;
            if (HttpContext.Current.Request.IsSecureConnection) {
                HttpContext.Current.Response.Cookies[param].Secure = true;
            }
            //if (common_logic.AppConfig("cookie_domain") != "")
            //{
            //    HttpContext.Current.Response.Cookies[param].Domain = common_logic.AppConfig("cookie_domain");
            //}
        }

        public static void set_cookie(string param, string value, DateTime expired)
        {
            if (Encrypt.Enable)
            {
                param = Encrypt.EncryptData(Encrypt.Key, param, true);
                value = Encrypt.EncryptData(Encrypt.Key, value);
            }
            HttpContext.Current.Response.Cookies[param].Value = value;
            HttpContext.Current.Response.Cookies[param].Expires = expired;

            HttpContext.Current.Response.Cookies[param].HttpOnly = true;
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                HttpContext.Current.Response.Cookies[param].Secure = true;
            }
            //if (common_logic.AppConfig("cookie_domain") != "")
            //{
            //    HttpContext.Current.Response.Cookies[param].Domain = common_logic.AppConfig("cookie_domain");
            //}
        }

        public static void remove_all_cookies()
        {
            DateTime expired = DateTime.Now.AddSeconds(-1);
            foreach (string key in HttpContext.Current.Request.Cookies.AllKeys)
            {
                HttpContext.Current.Request.Cookies[key].Expires = expired;
                //common_logic.set_cookie(key, "", expired);
            }
        }

        public static string cookie(String paramName)
        {
            String tmpS = String.Empty;
            try
            {
                if (Encrypt.Enable)
                {
                    paramName = Encrypt.EncryptData(Encrypt.Key, paramName, true);
                    tmpS = Encrypt.DecryptData(Encrypt.Key, HttpContext.Current.Request.Cookies[paramName].Value.ToString());
                }
                else
                {
                    tmpS = HttpContext.Current.Request.Cookies[paramName].Value.ToString();
                }
            }
            catch
            {
                tmpS = String.Empty;
            }
            return tmpS;
        }

        public static int cookie_int(string paramName)
        {
            int result = 0;
            try
            {
                if (Encrypt.Enable)
                {
                    paramName = Encrypt.EncryptData(Encrypt.Key, paramName, true);
                    result = int.Parse(Encrypt.DecryptData(Encrypt.Key, HttpContext.Current.Request.Cookies[paramName].Value.ToString()));
                }
                else
                {
                    result = int.Parse(HttpContext.Current.Request.Cookies[paramName].Value.ToString());
                }
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public static string query_string(string paramName)
        {
            String tmpS = String.Empty;
            try
            {
                tmpS = HttpContext.Current.Request.QueryString[paramName].ToString();
            }
            catch
            {
                tmpS = String.Empty + "";
            }
            return tmpS;
        }

        public static int query_string_int(string paramName)
        {
            int result = 0;
            try
            {
                result = int.Parse(HttpContext.Current.Request.QueryString[paramName].ToString());
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public static double query_string_double(string paramName)
        {
            double result = 0;
            try
            {
                result = double.Parse(HttpContext.Current.Request.QueryString[paramName].ToString());
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public static String form(String paramName)
        {
            String tmpS = String.Empty;
            try
            {
                tmpS = HttpContext.Current.Request.Form[paramName].ToString();
            }
            catch
            {
                tmpS = String.Empty;
            }
            return tmpS;
        }

        public static int form_int(string paramName)
        {
            int result = 0;
            try
            {
                result = int.Parse(HttpContext.Current.Request.Form[paramName].ToString());
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public static String new_guid()
        {
            return System.Guid.NewGuid().ToString().Replace("-", "");
        }

        public static bool session_check(string paramName)
        {
            if (HttpContext.Current.Session[paramName] == null)
                return false;
            return true;
        }

        public static String session_get(String paramName)
        {
            String tmpS = String.Empty;
            try
            {
                tmpS = HttpUtility.HtmlEncode(HttpContext.Current.Session[paramName].ToString());
            }
            catch
            {
                tmpS = String.Empty;
            }
            return tmpS;
        }

        public static String session_get(String paramName, string strDefault)
        {
            string str = "";
            if (HttpContext.Current.Session[paramName] != null)
                str = HttpContext.Current.Session[paramName].ToString();
            else
                str = strDefault;
            return str;
        }

        public static int session_get_int(String paramName)
        {
            int result = 0;
            try
            {
                result = int.Parse(HttpContext.Current.Session[paramName].ToString());
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public static double session_get_double(String paramName)
        {
            double result = 0;
            try
            {
                result = double.Parse(HttpContext.Current.Session[paramName].ToString());
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public static void session_set(string paramName, string val)
        {
            HttpContext.Current.Session[paramName] = val;
        }

        public static bool IsValidEmailAddress(string EmailAddress)
        {

            //Regex regEmail = new Regex(@"^[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            Regex regEmail = new Regex(@"[-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}");
            if (regEmail.IsMatch(EmailAddress))
                return true;
            return false;
        }

        public static int check_allow_host(string host, string allowed_domains)
        {
            string[] arr_allowed_domain = allowed_domains.Split(';');

            for (int i = 0; i < arr_allowed_domain.Length; i++)
            {
                if (host == arr_allowed_domain[i])
                {
                    return 1;
                }
            }

            return 0;
        }

        public static bool IsValidFormatDate(string date)
        {
            string strRegex = @"([0]?[1-9]|[12][0-9]|3[01])[- /.]([0]?[1-9]|1[012])[- /.](19|20)\d\d";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(strRegex);
            return regex.IsMatch(date);
        }

        /// <summary>
        /// Check vietnamese date of birth
        /// Alow formats dd/mm/yyyy | dd-mm-yyyy
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool isValidDOB(string date)
        {
            try
            {
                string[] dateParts = new string[3];
                if (date.IndexOf('-') >= 0)
                {
                    // for US, alter to suit if splitting on hyphen, comma, etc.
                    dateParts = date.Split('-');
                }
                else if (date.IndexOf('/') >= 0)
                {
                    dateParts = date.Split('/');
                }

                if (DateTime.Now.Year < int.Parse(dateParts[2]))
                {
                    return false;
                }

                // create new date from the parts; if this does not fail
                // the method will return true and the date is valid
                DateTime testDate = new
                    DateTime(Convert.ToInt32(dateParts[2]),
                    Convert.ToInt32(dateParts[1]),
                    Convert.ToInt32(dateParts[0]));

                return true;
            }
            catch
            {
                // if a test date cannot be created, the
                // method will return false
                return false;
            }
        }

        public static bool parseDOB(string date, ref DateTime outDate)
        {

            if (!isValidDOB(date))
            {
                return false;
            }

            string[] dateParts = new string[3];
            if (date.IndexOf('-') >= 0)
            {
                dateParts = date.Split('-');
            }
            else if (date.IndexOf('/') >= 0)
            {
                dateParts = date.Split('/');
            }
            // create new date from the parts; if this does not fail
            // the method will return true and the date is valid
            outDate = new
                DateTime(Convert.ToInt32(dateParts[2]),
                Convert.ToInt32(dateParts[1]),
                Convert.ToInt32(dateParts[0]));

            return true;

        }

        public static int CalculateAge(DateTime birthday) {

            DateTime Today = getNow();
            TimeSpan ts = Today - birthday;
            DateTime Age = DateTime.MinValue + ts;

            //(Math.Round(ts.Days / 365.0).ToString());
            int age = 0;
            age = int.Parse(Math.Round(ts.Days / 365.0).ToString());

            return age;
        }

        public static bool IsValidPhone(string Phone)
        {
            try
            {
                Regex reg = new Regex(@"^[0-9]{8,11}$");
                return reg.IsMatch(Phone);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidMobile(string Phone)
        {
            try
            {
                Regex reg = new Regex(@"^[0-9]{9,13}$");
                return reg.IsMatch(Phone);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidate(String Text)
        {
            if (Text.Contains("xp_") || Text.Contains("union") || Text.Contains("uni/**/on")
                || Text.Contains("'\"") || Text.Contains(");|]*")
                || Text.Contains("{%0d") || Text.Contains("|]*{")
                || Text.Contains("\";") || Text.Contains("';")
                || Text.Contains("' or") || Text.Contains("\" or")
                || Text.Contains("' and") || Text.Contains("\" and")
                || Text.Contains(";--") || Text.Contains("nvarchar")
                || Text.Contains("-1 or") || Text.Contains("1 or")
                || Text.Contains("-1 and") || Text.Contains("1 and")
                || Text.Contains("sleep") || Text.Contains("; waitfor delay")
                || Text.Contains("<script") || Text.Contains("</script>")
                || Text.Contains("1'1") || Text.Contains("():;")
                || Text.Contains("%3D%22")
                || Text.Contains("="))
            {
                return false;
            }

            return true;
        }

        public static bool IsValidPassword(String Text)
        {
            // Regex matching for password complex
            Match matchUpper = Regex.Match(Text, @"[A-Z]+");
            Match matchLower = Regex.Match(Text, @"[a-z]+");
            Match matchNumber = Regex.Match(Text, @"[0-9]+");
            Match matchSpecial = Regex.Match(Text, @"[^\w]+");

            int matchValidPass = 0;

            matchValidPass += matchUpper.Success ? 1 : 0;
            matchValidPass += matchLower.Success ? 1 : 0;
            matchValidPass += matchNumber.Success ? 1 : 0;
            matchValidPass += matchSpecial.Success ? 1 : 0;

            if (Text.Length < 6 || matchValidPass < 2)
            {
                return false;
            }
            else
                return true;
        }

        public static bool IsValidCharaters(String Text)
        {
            if (Text.Contains("xp_") || Text.Contains("union") || Text.Contains("uni/**/on")
                || Text.Contains("'\"") || Text.Contains(");|]*")
                || Text.Contains("{%0d") || Text.Contains("|]*{")
                || Text.Contains("\";") || Text.Contains("';")
                || Text.Contains("' or") || Text.Contains("\" or")
                || Text.Contains("' and") || Text.Contains("\" and")
                || Text.Contains(";--") || Text.Contains("nvarchar")
                || Text.Contains("-1 or") || Text.Contains("1 or")
                || Text.Contains("-1 and") || Text.Contains("1 and")
                || Text.Contains("sleep") || Text.Contains("; waitfor delay")
                || Text.Contains("<script") || Text.Contains("</script>")
                || Text.Contains("1'1") || Text.Contains("():;")
                || Text.Contains("%3D%22") || Text.Contains("\""))
            {
                return false;
            }
            else
            {
                //String VIETNAMESE_DIACRITIC_CHARACTERS = "ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴ";
                //Regex reg = new Regex(@"^[" + VIETNAMESE_DIACRITIC_CHARACTERS + "][A-Za-z0-9~!@#$%^&*\"\\|=+:;,.' _-/?]+$");
                Regex regNotAllow = new Regex(@"[\^\{\[\(\|\)\*\\&<>`']+");
                if (regNotAllow.IsMatch(Text))
                    return false;
                else
                    return true;
            }
        }

        

        public static void save_file_from_url(string file_name, string url)
        {
            byte[] content;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();
            //int i;
            using (BinaryReader br = new BinaryReader(stream))
            {
                content = br.ReadBytes(500000);
                br.Close();
            }
            response.Close();

            FileStream fs = new FileStream(file_name, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                bw.Write(content);
            }
            finally
            {
                fs.Close();
                bw.Close();
            }
        }

        public static void WriteBytesToFile(string fileName, byte[] content)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);
            try
            {
                w.Write(content);
            }
            finally
            {
                fs.Close();
                w.Close();
            }
        }

        public static string MakeFriendlyFileName(string name)
        {
            string invalidChars = Regex.Escape("^ .$%'`{}~*!#()&_^:’/,?");
            string invalidReStr = string.Format(@"[{0}]", invalidChars);
            return Regex.Replace(name, invalidReStr, "-");
        }

        public static string MakeFriendlyUrl(string name)
        {
            name = MakeFriendlyFileName(remove_vietnamese(name).ToLower());
            name = name.Replace("-–-", "-");
            return name;
        }

        public static string remove_vietnamese(string viet_string)
        {
            string[] viet_chars = new string[] { 
            "à","á","ạ","ả","ã","â","ầ","ấ","ậ","ẩ","ẫ","ă",
			"ằ","ắ","ặ","ẳ","ẵ","è","é","ẹ","ẻ","ẽ","ê","ề",
			"ế","ệ","ể","ễ",
			"ì","í","ị","ỉ","ĩ",
			"ò","ó","ọ","ỏ","õ","ô","ồ","ố","ộ","ổ","ỗ","ơ",
			"ờ","ớ","ợ","ở","ỡ",
			"ù","ú","ụ","ủ","ũ","ư","ừ","ứ","ự","ử","ữ",
			"ỳ","ý","ỵ","ỷ","ỹ",
			"đ",
			"À","Á","Ạ","Ả","Ã","Â","Ầ","Ấ","Ậ","Ẩ","Ẫ","Ă",
			"Ằ","Ắ","Ặ","Ẳ","Ẵ",
			"È","É","Ẹ","Ẻ","Ẽ","Ê","Ề","Ế","Ệ","Ể","Ễ",
			"Ì","Í","Ị","Ỉ","Ĩ",
			"Ò","Ó","Ọ","Ỏ","Õ","Ô","Ồ","Ố","Ộ","Ổ","Ỗ","Ơ",
			"Ờ","Ớ","Ợ","Ở","Ỡ",
			"Ù","Ú","Ụ","Ủ","Ũ","Ư","Ừ","Ứ","Ự","Ử","Ữ",
			"Ỳ","Ý","Ỵ","Ỷ","Ỹ",
			"Đ"
        };

            string[] alpha_chars = new string[] { 
            "a","a","a","a","a","a","a","a","a","a","a",
			"a","a","a","a","a","a",
			"e","e","e","e","e","e","e","e","e","e","e",
			"i","i","i","i","i",
			"o","o","o","o","o","o","o","o","o","o","o","o",
			"o","o","o","o","o",
			"u","u","u","u","u","u","u","u","u","u","u",
			"y","y","y","y","y",
			"d",
			"A","A","A","A","A","A","A","A","A","A","A","A",
			"A","A","A","A","A",
			"E","E","E","E","E","E","E","E","E","E","E",
			"I","I","I","I","I",
			"O","O","O","O","O","O","O","O","O","O","O","O",
			"O","O","O","O","O",
			"U","U","U","U","U","U","U","U","U","U","U",
			"Y","Y","Y","Y","Y",
			"D"
        };

            int i = 0;
            foreach (string viet_char in viet_chars)
            {
                viet_string = viet_string.Replace(viet_char, alpha_chars[i]);
                i++;
            }

            return viet_string;
        }

        public static string FormatDateAgo(object oDate, string language)
        {
           
           // HttpContext.Current.Response.Write(language);
            DateTime date = Convert.ToDateTime(oDate);
            DateTime dateNow = DateTime.Now;
            string timeline = "";
            if (dateNow >= date)
            {
                if (dateNow.Year != date.Year)
                {
                    // get number of months since posting
                    int iMonthCount = 0;
                    DateTime dateIndex = date;
                    while (dateIndex <= dateNow)
                    {
                        iMonthCount++;
                        dateIndex = dateIndex.AddMonths(1);
                    }

                    if (iMonthCount > 11)
                    {
                        return "Over a year ago";
                    }
                    else if (iMonthCount == 1)
                    {
                        if (language.Contains("en"))
                        {
                            timeline = "month ago";
                        }
                        else {
                            timeline = "tháng trước";
                        }
                        return "1 " + timeline;
                    }
                    else
                    {
                        if (language.Contains("en"))
                        {
                            timeline = "months ago";
                        }
                        else
                        {
                            timeline = "tháng trước";
                        }
                        return iMonthCount.ToString() + " " + timeline;
                    }
                }

                int iMonthDiff = dateNow.Month - date.Month;
                if (iMonthDiff > 0)
                {
                    if (iMonthDiff == 1)
                    {
                        if (language.Contains("en"))
                        {
                            timeline = "month ago";
                        }
                        else
                        {
                            timeline = "tháng trước";
                        }
                        return "1 " + timeline;
                    }
                    if (language.Contains("en"))
                    {
                        timeline = "months ago";
                    }
                    else
                    {
                        timeline = "tháng trước";
                    }
                    return iMonthDiff.ToString() + " " + timeline;
                }

                TimeSpan ts = dateNow.Subtract(date);
                if (ts.Days > 0)
                {
                    if (ts.Days == 1)
                    {
                        if (language.Contains("en"))
                        {
                            timeline = "day ago";
                        }
                        else
                        {
                            timeline = "ngày trước";
                        }
                        return "1 " + timeline;
                    }
                    if (language.Contains("en"))
                    {
                        timeline = "days ago";
                    }
                    else
                    {
                        timeline = "ngày trước";
                    }
                    return ts.Days.ToString() + " " + timeline;
                }

                if (ts.Hours > 0)
                {
                    if (ts.Hours == 1)
                    {
                        if (language.Contains("en"))
                        {
                            timeline = "hour ago";
                        }
                        else
                        {
                            timeline = "giờ trước";
                        }
                        return "1 "+ timeline;
                    }
                    if (language.Contains("en"))
                    {
                        timeline = "hours ago";
                    }
                    else
                    {
                        timeline = "giờ trước";
                    }
                    return ts.Hours.ToString() + " " + timeline;
                }

                if (ts.Minutes > 0)
                {
                    if (ts.Minutes == 1)
                    {
                        if (language.Contains("en"))
                        {
                            timeline = "minute ago";
                        }
                        else
                        {
                            timeline = "phút trước";
                        }
                        return "1 " + timeline;
                    }
                    if (language.Contains("en"))
                    {
                        timeline = "minutes ago";
                    }
                    else
                    {
                        timeline = "phút trước";
                    }
                    return ts.Minutes.ToString() + " " + timeline;
                }
            }
            if (language.Contains("en"))
            {
                timeline = "minute ago";
            }
            else
            {
                timeline = "phút trước";
            }
            return "0 " + timeline;
        }
        public static bool isValidateImageType(HttpPostedFile FileUpload)
        {
            string[] validFileTypes={"bmp","gif","png","jpg","jpeg"};
            string ext = System.IO.Path.GetExtension(FileUpload.FileName).ToLower();  
            bool isValidFile = false;
            for (int i = 0; i < validFileTypes.Length; i++)
            {
                if (ext == "." + validFileTypes[i] )
                {
                    isValidFile = true;
                    break;
                }
            }
            return isValidFile;
        }

        public static DateTime getNow()
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            //return new DateTime(2014, 1, 1);
            return TimeZoneInfo.ConvertTime(DateTime.Now, tz);

        }
        public static bool FindValueFromArray(object[] Values, object valueToSearch)
        {

            bool retVal = false;

            Array myArray = (Array)Values;

            int found = Array.BinarySearch(myArray, valueToSearch);

            if (found != -1)
            {

                retVal = true;

            }

            return retVal;

        }

        public static string GetCurrentDomain() 
        {
            Uri currentUri = HttpContext.Current.Request.Url;
            string currentDomain = currentUri.Scheme + System.Uri.SchemeDelimiter + currentUri.Host + (currentUri.IsDefaultPort ? "" : ":" + currentUri.Port);
            return currentDomain;
        }
        //public static bool IsValidInternalReturnURL(string returnURL)
        //{
        //    if (returnURL.Contains("http:") || returnURL.Contains("https:")
        //        || returnURL.Contains("www.") || returnURL.Contains(".com"))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
        public static bool IsValidSQLInjection(string Text)
        {
            Text = Text.ToLower();
            if (Text.Contains("xp_") || Text.Contains("union") || Text.Contains("uni/**/on")
                || Text.Contains("'\"") || Text.Contains(");|]*")
                || Text.Contains("{%0d") || Text.Contains("|]*{")
                || Text.Contains("\";") || Text.Contains("';")
                || Text.Contains("' or") || Text.Contains("\" or")
                || Text.Contains("' and") || Text.Contains("\" and")
                || Text.Contains(";--") || Text.Contains("nvarchar")
                || Text.Contains("-1 or") || Text.Contains("1 or")
                || Text.Contains("-1 and") || Text.Contains("1 and")
                || Text.Contains("sleep") || Text.Contains("; waitfor delay")
                || Text.Contains("<script") || Text.Contains("</script>")
                || Text.Contains("1'1") || Text.Contains("():;")
                || Text.Contains("javascript:")
                )
            {
                return true;
            }
            return false;
        }

     

        public static DateTime GetLocalDateTime(int TimeZone)
        {
            return DateTime.UtcNow.AddHours(TimeZone);
        }

       
        public static bool IsValidStringIDs(string strIDs)
        {
            Regex regEmail = new Regex(@"^[0-9,-/]+$");
            if (regEmail.IsMatch(strIDs))
                return true;
            return false;
        }

        public static bool IsValidFloat(string FloatNumber)
        {
            float tmpFloatNumber;

            return float.TryParse(FloatNumber, out tmpFloatNumber);
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.UTF8.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string DecodeFrom64(string encodedData)
        {
            try
            {
                byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
                return System.Text.ASCIIEncoding.UTF8.GetString(encodedDataAsBytes);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string GeneratePaging(int pageIndex, int pageSize, int rowSum, string linkPage)
        {
            string sHtml = "";
            int displayItemsCount = 11; // 3 5 7 9 ...
            int displayItemsLeft = displayItemsCount / 2;
            int totalPage = (int)Math.Ceiling((double)rowSum / pageSize);
            string link = linkPage;
            if (totalPage > 1)
            {
                sHtml += "<ul class='paging'>";
                if (pageIndex == 1)
                {
                    sHtml += "      <li><a rel='prev' href='" + link + "?p=" + (pageIndex - 1) + "' title='Previous'>Trang trước</a></li>";
                }
                else if (pageIndex <= totalPage)
                {
                    sHtml += "      <li><a rel='prev' href='" + link + "?p=" + (pageIndex - 1) + "' title='Previous'>Trang trước</a></li>";
                }

                int beginLoop = 1;
                int endLoop = displayItemsCount;
                if (totalPage <= displayItemsCount)
                {
                    beginLoop = 1;
                    endLoop = totalPage;
                }
                else
                {
                    if (pageIndex <= displayItemsLeft)
                    {
                        beginLoop = 1;
                        if (totalPage <= displayItemsCount)
                        {
                            endLoop = totalPage;
                        }
                        else
                        {
                            endLoop = displayItemsCount;
                        }
                    }
                    else if ((pageIndex + displayItemsLeft) >= totalPage)
                    {
                        beginLoop = totalPage - (displayItemsLeft * 2);
                        endLoop = totalPage;
                    }
                    else
                    {
                        beginLoop = pageIndex - displayItemsLeft;
                        endLoop = pageIndex + displayItemsLeft;
                    }

                }

                for (int i = beginLoop; i <= endLoop; i++)
                {
                    if (i == pageIndex)
                    {
                        sHtml += "<li class='active'><a href='" + link + "?p=" + i + "' title='" + i.ToString() + "' ><strong>" + i.ToString() + "</strong></a></li>";
                    }
                    else
                    {
                        sHtml += "<li><a " + (i > pageIndex ? "rel='next'" : "rel='prev'") + " href='" + link + "?p=" + i + "' title='" + i.ToString() + "'>" + i.ToString() + "</a></li>";
                    }
                }

                if (pageIndex == totalPage)
                {
                    sHtml += "      <li><a rel='next' href='" + link + "?p=" + (pageIndex + 1) + "' title='Next'>Trang sau</a></li>";

                }
                else
                {
                    sHtml += "      <li><a rel='next' href='" + link + "?p=" + (pageIndex + 1) + "' title='Next'>Trang sau</a></li>";

                }
                sHtml += "</ul>";
            }

            return sHtml;
        }
    }
}