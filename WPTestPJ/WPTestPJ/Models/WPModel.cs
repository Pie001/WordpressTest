using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WPTestPJ.Common;
using WPTestPJ.Common.Models;

namespace WPTestPJ.Web.Models
{
    public class WPModel
    {
        public List<WPUserModel> UserList { get; set; }

        public List<WPPostModel> PostList { get; set; }
    }
}