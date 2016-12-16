using System;
using System.Data.Entity.Validation;
using System.Text;
using System.Web.Mvc;
using WPTestPJ.Common.BusinessLogics;
using WPTestPJ.Web.Models;
using System.Net;
using System.IO;
using WPTestPJ.Web.Common;

namespace WPTestPJ.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            WPModel wpmodel = new WPModel();

            using (var db = new SQLServerProxy())
            {
                try
                {
                    var wordPressBL = new WordPressBL(db._wplocalEntities);
                    wpmodel.UserList = wordPressBL.GetUserList();
                    //ViewBag.wpPostList = wordPressBL.GetPostList();
                    wpmodel.PostList = wordPressBL.GetPostList();
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(DbEntityValidationException))
                    {
                        DbEntityValidationException ex = (DbEntityValidationException)e;

                        foreach (var failure in ex.EntityValidationErrors)
                        {
                            foreach (var error in failure.ValidationErrors)
                            {
                                string aa = string.Format("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                            }
                        }
                    }
                }
            }

            // test s

            string response = string.Empty;
            //http://localhost/wordpress/wp-json/posts
            string url = "http://localhost/wordpress/wp-json/posts";
            object parameters = new
            {
                //cp_user_id = controlPanelUserID,
                //service_id = serviceId
            };

            try
            {
                //response = RestApi.GetApiResponse(url, "text/json", "GET", null);// RestApi.ObjectToJson(parameters)

                string responseResult = string.Empty;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        responseResult = streamReader.ReadToEnd();
                    }
                }

                //return JsonConvert.DeserializeObject<ListWPSitesSnapShotContainer>(response);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new RestfulAPIExeption(string.Format(
                //        "ErrorSource=({0}), ErrorMessage=({1}), StackTrace=({2})",
                //        ex.Source,
                //        ex.Message,
                //        ex.StackTrace));
            }

            // test e


            return View(wpmodel);
        }

        //public string GET(string url)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    try
        //    {
        //        WebResponse response = request.GetResponse();
        //        using (Stream responseStream = response.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
        //            return reader.ReadToEnd();
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        WebResponse errorResponse = ex.Response;
        //        using (Stream responseStream = errorResponse.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
        //            String errorText = reader.ReadToEnd();
        //            // log errorText
        //        }
        //        throw;
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddWPUser(WPUserAddModel model)
        {
            var result = new JsonObject();

            try
            {
                using (var db = new SQLServerProxy())
                {
                    var wordPressBL = new WordPressBL(db._wplocalEntities);
                    wordPressBL.InsertWPUser(model.LoginID, model.LastName, model.FirstName, model.Email);

                    db.Dispose();
                }
                result.SetResult(true);
            }
            catch (Exception)
            {
                result.AddError("errorMessage", "サーバーエラーが発生しました。");
            }

            return Json(result, "text/json", Encoding.UTF8, JsonRequestBehavior.DenyGet);
        }

        public ActionResult ViewWPUserList()
        {
            WPModel wpmodel = new WPModel();

            using (var db = new SQLServerProxy())
            {
                var wordPressBL = new WordPressBL(db._wplocalEntities);
                wpmodel.UserList = wordPressBL.GetUserList();

                db.Dispose();
            }

            return PartialView("_wpUserList", wpmodel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}