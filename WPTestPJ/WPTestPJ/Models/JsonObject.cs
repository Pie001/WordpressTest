using System.Collections.Generic;

namespace WPTestPJ.Web.Models
{
    public class JsonObject
    {
        public JsonObject()
        {
            this.Result = false;
            this.ResponseData = new Dictionary<string, string>();
            this.ErrorList = new List<Error>();
        }

        public bool Result { get; set; }

        public Dictionary<string, string> ResponseData { get; set; }

        public List<Error> ErrorList { get; set; }

        public void AddError(string type, string errorMessage)
        {
            this.ErrorList.Add(new Error { Type = type, Message = errorMessage });
        }

        public void AddData(string key, string value)
        {
            this.ResponseData[key] = value;
        }

        public void SetResult(bool result)
        {
            this.Result = result;
        }

        public class Error
        {
            public string Type { get; set; }

            public string Message { get; set; }
        }
    }
}