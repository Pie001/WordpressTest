using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPTestPJ.Common.Models
{
    public class WPUserModel
    {
        public string UserLogin { get; set; }
        public string UserNicename { get; set; }
        public string UserEmail { get; set; }
        public DateTime UserRegistered { get; set; }
        public string UserStatus { get; set; }
        public string DisplayName { get; set; }
        public DateTime ExpireDate { get; set; }
        public string ExpireDateTimestamp { get; set; }
    }
}
