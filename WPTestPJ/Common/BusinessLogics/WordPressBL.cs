using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPTestPJ.Common.Models;
using CryptSharp;
using WPTestPJ.Common.utility;
using System.Web;

namespace WPTestPJ.Common.BusinessLogics
{
    public class WordPressBL
    {
        private WPlocalEntities _wplocalEntities;
        //private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void Logger(string message)
        {
            //logger.Info(message);
        }

        public WordPressBL(WPlocalEntities entities)
        {
            this._wplocalEntities = entities;
        }

        public List<WPUserModel> GetUserList()
        {
            List<WPUserModel> wpuserList = new List<WPUserModel>();

            wpuserList = (from a in this._wplocalEntities.wp_users
                          join b in this._wplocalEntities.wp_usermeta on a.ID equals b.user_id
                          where b.meta_key == "_expire_user_date"
                          orderby a.ID
                          select new WPUserModel
                          {
                              UserLogin = a.user_login,
                              UserNicename = a.user_nicename,
                              UserEmail = a.user_email,
                              UserRegistered = a.user_registered,
                              UserStatus = a.user_status.ToString(),
                              DisplayName = a.display_name,
                              ExpireDateTimestamp = b.meta_value
                          }).ToList();


            wpuserList.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.ExpireDateTimestamp))
                {
                    x.ExpireDate = ConvertTimestampToDatetime(x.ExpireDateTimestamp);
                }
            });


            return wpuserList;
        }

        public List<WPPostModel> GetPostList()
        {
            List<WPPostModel> wpPostModelList = new List<WPPostModel>();
            wpPostModelList = (from a in this._wplocalEntities.wp_posts
                               join b in this._wplocalEntities.wp_users on a.post_author equals b.ID
                               where a.post_content != "" && (a.post_status == "publish" || a.post_status == "pending") && a.ping_status == "open"
                               orderby a.ID
                          select new WPPostModel
                          {
                              PostAuthor = a.post_author,
                              PostAuthorName = b.user_login,
                              PostDate = a.post_date,
                              PostDateGmt = a.post_date_gmt,
                              PostContent = a.post_content,
                              PostTitle = a.post_title,
                              PostExcerpt = a.post_excerpt,
                              PostStatus = a.post_status,
                              CommentStatus = a.comment_status,
                              PingStatus = a.ping_status,
                              Guid = a.guid,
                              PostType = a.post_type,
                              PostMimeType = a.post_mime_type,
                              CommentCount = a.comment_count
                          }).ToList();

            wpPostModelList.ForEach(x =>
            {
                if (x.PostStatus.Equals("publish"))
                {
                    x.PostStatusName = "公開済";
                }
                if (x.PostStatus.Equals("pending"))
                {
                    x.PostStatusName = "承認待ち";
                }

                if (x.CommentStatus.Equals("open")) 
                {
                    x.CommentStatusName = "許可";
                }
                if (x.CommentStatus.Equals("closed"))
                {
                    x.CommentStatusName = "不許可";
                }
                if (x.CommentStatus.Equals("registered_only"))
                {
                    x.CommentStatusName = "登録ユーザのみ";
                }

                if (x.PingStatus.Equals("open"))
                {
                    x.PingStatusName = "トラックバック・ピンバックを受け付ける";
                }
                if (x.PingStatus.Equals("closed"))
                {
                    x.PingStatusName = "受け付けない";
                }
            });

            return wpPostModelList;
        }

        public DateTime ConvertTimestampToDatetime(string timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddSeconds(Convert.ToUInt32(timestamp)).ToLocalTime();
        }

        public void InsertWPUser(string loginId, string lastName, string firstName, string email)
        {
            using (var tran = this._wplocalEntities.Database.BeginTransaction())
            {
                //SQL ログの出力設定 
                //db.Database.Log = Logger;

                var wpUser = new wp_users();

                string pass = "1234";

                wpUser.user_login = loginId;
                wpUser.user_pass = Crypter.Phpass.Crypt(pass);
                wpUser.user_nicename = loginId;
                wpUser.user_email = email;
                wpUser.user_registered = DateTime.Now;
                wpUser.display_name = lastName + " " + firstName;
                wpUser.user_url = "";
                wpUser.user_activation_key = "";

                this._wplocalEntities.wp_users.Add(wpUser);
                this._wplocalEntities.SaveChanges();

                decimal userId = wpUser.ID;
                // 有効期限を設定
                DateTime expireDate = DateTime.Now.AddDays(2);
                var timespan = expireDate - new DateTime(1970, 1, 1, 0, 0, 0);
                string timestamp = ((uint)timespan.TotalSeconds).ToString();

                List<Usermeta> usermetaList = new List<Usermeta>();
                usermetaList.Add(new Usermeta { MetaKey = "nickname", MetaValue = loginId });
                usermetaList.Add(new Usermeta { MetaKey = "first_name", MetaValue = firstName });
                usermetaList.Add(new Usermeta { MetaKey = "last_name", MetaValue = lastName });
                usermetaList.Add(new Usermeta { MetaKey = "description", MetaValue = "" });
                usermetaList.Add(new Usermeta { MetaKey = "rich_editing", MetaValue = "true" });
                usermetaList.Add(new Usermeta { MetaKey = "comment_shortcuts", MetaValue = "false" });
                usermetaList.Add(new Usermeta { MetaKey = "admin_color", MetaValue = "fresh" });
                usermetaList.Add(new Usermeta { MetaKey = "use_ssl", MetaValue = "0" });

                usermetaList.Add(new Usermeta { MetaKey = "show_admin_bar_front", MetaValue = "true" });
                usermetaList.Add(new Usermeta { MetaKey = "wp_capabilities", MetaValue = "a:1:{s:11:\"contributor\";b:1;}" });
                usermetaList.Add(new Usermeta { MetaKey = "wp_user_level", MetaValue = "1" });
                usermetaList.Add(new Usermeta { MetaKey = "_expire_user_date", MetaValue = timestamp });
                usermetaList.Add(new Usermeta { MetaKey = "_expire_user_settings", MetaValue = "a:5:{s:15:\"default_to_role\";s:0:\"\";s:14:\"reset_password\";b:0;s:5:\"email\";b:0;s:11:\"email_admin\";b:0;s:13:\"remove_expiry\";b:0;}" });
                usermetaList.Add(new Usermeta { MetaKey = "_expire_user_expired", MetaValue = "N" });
                usermetaList.Add(new Usermeta { MetaKey = "dismissed_wp_pointers", MetaValue = "" });

                foreach (var usermeta in usermetaList)
                {
                    var wpUsermeta = new wp_usermeta();
                    wpUsermeta.user_id = userId;
                    wpUsermeta.meta_key = usermeta.MetaKey;
                    wpUsermeta.meta_value = usermeta.MetaValue;

                    this._wplocalEntities.wp_usermeta.Add(wpUsermeta);
                }

                this._wplocalEntities.SaveChanges();

                tran.Commit();

                SendMail(lastName + " " + firstName, loginId, pass, expireDate, true);
            }
        }

        public void SendMail(string name, string loginId, string password, DateTime expireDate, bool isTest)
        {
            var cmail = new CMail();
            cmail.IsTest = isTest;
            string tempEmail = “mail@address.com”;
            string from = tempEmail;
            string to = tempEmail;
            string subject = string.Empty;
            string body = string.Empty;

            string mailTemplate = "AddWPUserSuccessful";

            subject = string.Format("【重要】Wordpress投稿アカウント発行のお知らせ：{0}様", name);
            using (var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/MailTemplate/" + mailTemplate + ".txt"), System.Text.Encoding.Default))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("#社員名#", name)
                .Replace("#ログインID#", loginId)
                .Replace("#パスワード#", password)
                .Replace("#有効期限#", expireDate.ToString("yyyy/MM/dd HH:mm:ss"));

            string[] smtpSettings = ConfigurationManager.AppSettings["Smtp"].Split('/');

            if (!cmail.SendMail(smtpSettings[0], Int32.Parse(smtpSettings[1]), from, to, subject, body, ""))
            {
                //logger.Error(string.Format("■SendMail Error! : {0}", cmail.ErrMsg));
            }
        }
    }
    
    public class Usermeta
    {
        public string MetaKey { get; set; }
        public string MetaValue { get; set; }
    }
}
