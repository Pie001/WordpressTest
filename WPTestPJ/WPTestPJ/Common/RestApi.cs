using System;
using System.IO;
using System.Net;
//using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Dynamic;

namespace WPTestPJ.Web.Common
{

    /// <summary>
    /// REST APIライブラリ
    /// </summary>
    public class RestApi
    {
        /// <summary>
        /// REST APIのレスポンスをjson形式のstring文字列で返す。
        /// </summary>
        /// <param name="requestUri">API URL</param>
        /// <param name="contentType">contentType(ex)"text/json")</param>
        /// <param name="method">method(ex)"POST")</param>
        /// <param name="parameters">リクエストパラメータ(json形式)(ex){"name":"wp-user"})</param>
        /// <returns></returns>
        public static string GetApiResponse(string requestUri, string contentType, string method, string parameters)
        {
            string responseResult = string.Empty;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = method;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                // パラメータが存在する場合、パラメータを設定
                if (parameters != null && parameters != string.Empty)
                {
                    streamWriter.Write(parameters); // json形式のdata
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseResult = streamReader.ReadToEnd();
                }
            }
            return responseResult;
        }

        /// <summary>
        /// リクエストデータ作成用
        /// object -> JSON(string)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJson(object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }

        /// <summary>
        /// ダイナミック型での対応
        /// </summary>
        /// <param name="expando"></param>
        /// <returns></returns>
        public static string ObjectToJson(ExpandoObject expando)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StringBuilder json = new StringBuilder();
            List<string> keyPairs = new List<string>();
            IDictionary<string, object> dictionary = expando as IDictionary<string, object>;
            json.Append("{");

            foreach (KeyValuePair<string, object> pair in dictionary)
            {
                if (pair.Value is ExpandoObject)
                {
                    keyPairs.Add(String.Format(@"""{0}"": {1}", pair.Key, ObjectToJson((pair.Value as ExpandoObject))));
                }
                else
                {
                    keyPairs.Add(String.Format(@"""{0}"": {1}", pair.Key, serializer.Serialize(pair.Value)));
                }
            }

            json.Append(String.Join(",", keyPairs.ToArray()));
            json.Append("}");

            return json.ToString();
        }



        // ## サンプル
        // json形式のstring文字列を指定したクラス型で返す

        // json -> クラス型
        // HelloList hellList = JsonConvert.DeserializeObject<HelloList>(responseList);

        // json -> クラス・匿名型
        // var HelloList2 = JsonConvert.DeserializeAnonymousType(responseList, new HelloList());

        // /api/service/users/hello コンテナ
        public class Hello
        {
            public int status
            {
                get;
                set;
            }
            public Users users
            {
                get;
                set;
            }

            public class Users
            {
                public string message
                {
                    get;
                    set;
                }
            }
        }

        // /api/service/users/hello_list コンテナ

        public class HelloList
        {
            public int status
            {
                get;
                set;
            }
            public List<User> users
            {
                get;
                set;
            }

            public class User
            {
                public int id
                {
                    get;
                    set;
                }
                public string message
                {
                    get;
                    set;
                }
                public Obj obj
                {
                    get;
                    set;
                }
                public List<List> list
                {
                    get;
                    set;
                }
            }

            public class Obj
            {
                public string type
                {
                    get;
                    set;
                }
            }

            public class List
            {
                public int list_id
                {
                    get;
                    set;
                }
                public Val val
                {
                    get;
                    set;
                }
            }

            public class Val
            {
                public string type
                {
                    get;
                    set;
                }
            }
        }

    }
}