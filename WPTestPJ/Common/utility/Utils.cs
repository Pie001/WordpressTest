using WPTestPJ.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WPTestPJ.Common.utility
{
    public class Utils
    {
        /// <summary>
        /// メール送信(お客様宛)
        /// </summary>
        /// <param name="mailFrom">送信元アドレス</param>
        /// <param name="mailTo">送信先アドレス</param>
        /// <param name="subject">件名</param>
        /// <param name="body">本文</param>
        /// <param name="mailCC">送信先CC</param>
        /// <param name="mailBcc">送信先Bcc</param>
        /// <returns></returns>
        public static bool SendMail(string mailFrom, string mailTo, string subject, string body, string mailCC = null, string mailBcc = null)
        {
            if (string.IsNullOrEmpty(mailFrom) || string.IsNullOrEmpty(mailTo))
            {
                return false;
            }

            // 設定情報読込み
            string smtp = ConfigurationManager.AppSettings["Smtp"];
            if (string.IsNullOrEmpty(smtp))
            {
                return false;
            }
            string[] smtpSettings = smtp.Split('/');

            using (System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient())
            {
                // SMTP設定
                smtpClient.Host = smtpSettings[0];
                smtpClient.Port = Int32.Parse(smtpSettings[1]);
                smtpClient.Credentials = new System.Net.NetworkCredential(smtpSettings[2], smtpSettings[3]);

                using (System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage())
                {
                    mailMessage.From = new System.Net.Mail.MailAddress(mailFrom);
                    mailMessage.To.Add(mailTo);
                    if (mailCC != null)
                    {
                        mailMessage.CC.Add(mailCC);
                    };
                    if (mailBcc != null)
                    {
                        mailMessage.Bcc.Add(mailBcc);
                    };
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    smtpClient.Send(mailMessage);
                }
            }
            return true;
        }

        public static string EncryptBase64StringDES(string plainText, string password)
        {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

            //パスワードから共有キーと初期化ベクタを作成
            // saltは8bit以上に設定する
            byte[] salt = Encoding.UTF8.GetBytes("LAGDLAlkasdgklq31354");
            //Rfc2898DeriveBytesオブジェクトを作成する
            var deriveBytes = new Rfc2898DeriveBytes(password, salt);
            deriveBytes.IterationCount = 1000;

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateEncryptor(deriveBytes.GetBytes(cryptoProvider.KeySize / 8), deriveBytes.GetBytes(cryptoProvider.BlockSize / 8)), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(plainText);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            string encrypt = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length).Replace("+", ".").Replace("/", "_").Replace("=", "-");

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(encrypt));
        }

        public static string DecryptDES(string cipherText, string password)
        {
            cipherText = cipherText.Replace("-", "=").Replace("_", "/").Replace(".", "+");

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            //パスワードから共有キーと初期化ベクタを作成
            // saltは8bit以上に設定する
            byte[] salt = Encoding.UTF8.GetBytes("LAGDLAlkasdgklq31354");
            //Rfc2898DeriveBytesオブジェクトを作成する
            var deriveBytes = new Rfc2898DeriveBytes(password, salt);
            deriveBytes.IterationCount = 1000;

            MemoryStream memoryStream = new MemoryStream
                    (Convert.FromBase64String(cipherText));
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateDecryptor(deriveBytes.GetBytes(cryptoProvider.KeySize / 8), deriveBytes.GetBytes(cryptoProvider.BlockSize / 8)), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

        public static string DecryptBase64StringDES(string cipherText, string password)
        {
            cipherText = Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));

            cipherText = cipherText.Replace("-", "=").Replace("_", "/").Replace(".", "+");

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            //パスワードから共有キーと初期化ベクタを作成
            // saltは8bit以上に設定する
            byte[] salt = Encoding.UTF8.GetBytes("LAGDLAlkasdgklq31354");
            //Rfc2898DeriveBytesオブジェクトを作成する
            var deriveBytes = new Rfc2898DeriveBytes(password, salt);
            deriveBytes.IterationCount = 1000;

            MemoryStream memoryStream = new MemoryStream
                    (Convert.FromBase64String(cipherText));
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateDecryptor(deriveBytes.GetBytes(cryptoProvider.KeySize / 8), deriveBytes.GetBytes(cryptoProvider.BlockSize / 8)), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// 簡易パスワード生成
        /// </summary>
        /// <param name="len">パスワードの長さ</param>
        /// <param name="validChars">使用できる文字のリスト</param>
        /// <returns></returns>
        public static string GenerateSimplePassword(int len, string validChars)
        {
            Random rnd = new Random(DateTime.Now.Ticks.GetHashCode());
            int rndMax = validChars.Length - 1;
            StringBuilder pw = new StringBuilder(len);
            int pos;
            for (int ii = 0; ii <= len - 1; ii++)
            {
                pos = rnd.Next(0, rndMax);
                pw.Append(validChars[pos]);
            }
            return pw.ToString();
        }

        /// <summary>
        /// MD5ハッシュ値を取得
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Hash(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// MD5ハッシュ値を検証する
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyMd5Hash(string input, string hash)
        {
            string hashOfInput = GetMd5Hash(input);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
        }

        /// <summary>
        /// オブジェクトをクエリストリングに変換
        /// </summary>
        /// <param name="request"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToQueryString(object request, string separator = ",")
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            // Get all properties on the object
            var properties = request.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null)
                .ToDictionary(x => x.Name, x => x.GetValue(request, null));

            // Get names for all IEnumerable properties (excl. string)
            var propertyNames = properties
                .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                .Select(x => x.Key)
                .ToList();

            // Concat all IEnumerable properties into a comma separated string
            foreach (var key in propertyNames)
            {
                var valueType = properties[key].GetType();
                var valueElemType = valueType.IsGenericType
                                        ? valueType.GetGenericArguments()[0]
                                        : valueType.GetElementType();
                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                {
                    var enumerable = properties[key] as IEnumerable;
                    properties[key] = string.Join(separator, enumerable.Cast<object>());
                }
            }

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties
                .Select(x => string.Concat(
                    Uri.EscapeDataString(x.Key), "=",
                    Uri.EscapeDataString(x.Value.ToString()))));
        }

        /// <summary>
        /// クエリストリングからオブジェクトへ変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static T ConvertFromQueryString<T>(string queryString) where T : new()
        {
            var obj = new T();
            var properties = typeof(T).GetProperties();

            System.Collections.Specialized.NameValueCollection nvp = System.Web.HttpUtility.ParseQueryString(queryString);

            foreach (var property in properties)
            {
                object value = Parse(property.PropertyType, nvp[property.Name]);
                if (value == null)
                {
                    continue;
                }

                property.SetValue(obj, value, null);
            }
            return obj;
        }

        private static object Parse(Type dataType, string val)
        {
            TypeConverter obj = TypeDescriptor.GetConverter(dataType);
            return obj.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, val);
        }

        public static void DeleteLocalFileList(List<string> deleteFilePathList)
        {
            foreach (var deleteFilePath in deleteFilePathList)
            {
                FileInfo deleteFile = new FileInfo(deleteFilePath);

                if (deleteFile.Exists)
                {
                    if ((deleteFile.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        deleteFile.Attributes = FileAttributes.Normal;
                    }
                    deleteFile.Delete();
                }
            }
        }

        public static List<string> GetLocalFileList(string path, string searchPattern)
        {
            var fileList = new List<string>();

            try
            {
                foreach (string filePath in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly))
                {
                    fileList.Add(filePath);
                }
            }
            catch (Exception)
            {
            }

            return fileList;
        }

        //public static List<SelectObject> GetMonthList()
        //{
        //    var monthList = new List<SelectObject>();

        //    for (int i = 1; i <= 12; i++)
        //    {
        //        monthList.Add(new SelectObject(i.ToString(), i.ToString()));
        //    }

        //    return monthList;
        //}

        public static int RemoveComma(string amountValue)
        {
            return Convert.ToInt32(amountValue.Replace("円", "").Replace(",", ""));
        }
    }

    /// <summary>
    /// メールクラス(社内向けメール用)
    /// </summary>
    public class CMail
    {
        public string Smtp { get; set; }

        public int Port { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string SubjectPrefix { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsTest { get; set; }

        public EMailType MailType { get; set; }

        public string ErrMsg { get; set; }

        public enum EMailType
        {
            /// <summary>
            /// 情報メール
            /// </summary>
            Info = 0,

            /// <summary>
            /// アラートメール
            /// </summary>
            Alert = 1,

            /// <summary>
            /// 注意メール
            /// </summary>
            Warning = 2
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        public CMail()
        {
            Smtp = "";
            Port = 25;
            From = "";
            To = "";
            Cc = "";
            Bcc = "";
            SubjectPrefix = "";
            Subject = "";
            Body = "";
            IsTest = false;
            MailType = EMailType.Info;
            ErrMsg = "";
        }

        /// <summary>
        /// メール送信
        /// </summary>
        /// <param name="from">送信者メールアドレス(プロパティでの設定可)</param>
        /// <param name="to">宛先メールアドレス(カンマ「,」で区切って複数指定可能 プロパティでの設定可)</param>
        /// <param name="subject">件名</param>
        /// <param name="body">本文</param>
        /// <param name="cc">CCメールアドレス(カンマ「,」で区切って複数指定可能 プロパティでの設定可)</param>
        /// <param name="bcc">BCCメールアドレス(カンマ「,」で区切って複数指定可能 プロパティでの設定可)</param>
        /// <returns></returns>
        public bool SendMail(string smtp = "", int port = 25, string from = null, string to = "", string subject = "", string body = "", string cc = "", string bcc = "")
        {
            try
            {
                // プロパティ設定値取得
                if (string.IsNullOrEmpty(smtp))
                {
                    smtp = Smtp;
                }
                if (Port != 25)
                {
                    port = Port;
                }
                if (from == null)
                {
                    from = From;
                }
                if (string.IsNullOrEmpty(to))
                {
                    to = To;
                }
                if (string.IsNullOrEmpty(cc))
                {
                    cc = Cc;
                }
                if (string.IsNullOrEmpty(bcc))
                {
                    bcc = Bcc;
                }
                if (string.IsNullOrEmpty(subject))
                {
                    subject = Subject;
                }
                if (string.IsNullOrEmpty(body))
                {
                    body = Body;
                }

                if (string.IsNullOrEmpty(smtp))
                {
                    ErrMsg = "[SMTP] is not exists.";
                    return false;
                }
                else if (string.IsNullOrEmpty(from))
                {
                    ErrMsg = "[From] is not exists.";
                    return false;
                }
                else if (string.IsNullOrEmpty(to))
                {
                    ErrMsg = "[To] is not exists.";
                    return false;
                }

                using (System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient())
                {
                    smtpClient.Host = smtp;
                    smtpClient.Port = port;

                    using (System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage())
                    {
                        mailMessage.From = new System.Net.Mail.MailAddress(from);
                        mailMessage.To.Add(to);

                        if (string.IsNullOrEmpty(cc) == false)
                        {
                            mailMessage.CC.Add(cc);
                        }

                        if (string.IsNullOrEmpty(bcc) == false)
                        {
                            mailMessage.Bcc.Add(bcc);
                        }

                        // 件名設定
                        string makeSubject = "";
                        if (IsTest)
                        {
                            makeSubject = "[TEST]";
                        }

                        makeSubject += SubjectPrefix;

                        switch (MailType)
                        {
                            case EMailType.Alert:
                                makeSubject += "Alert! " + subject;
                                break;

                            case EMailType.Warning:
                                makeSubject += "Warning! " + subject;
                                break;

                            default:
                                makeSubject += subject;
                                break;
                        }

                        mailMessage.Subject = makeSubject;
                        mailMessage.Body = body;
                        smtpClient.Send(mailMessage);
                    }
                }
                return true;
            }
            catch (System.Net.Mail.SmtpFailedRecipientsException ex)
            {
                ErrMsg = ex.Message;
                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return false;
            }
        }
    }
}