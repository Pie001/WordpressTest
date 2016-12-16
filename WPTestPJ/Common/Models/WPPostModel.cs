using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPTestPJ.Common.Models
{
    public class WPPostModel
    {
        public decimal PostAuthor { get; set; }
        public string PostAuthorName { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime PostDateGmt { get; set; }
        public string PostContent { get; set; }
        public string PostTitle { get; set; }
        public string PostExcerpt { get; set; }
        public string PostStatus { get; set; }
        public string PostStatusName { get; set; }
        public string CommentStatus { get; set; }
        public string CommentStatusName { get; set; }
        public string PingStatus { get; set; }
        public string PingStatusName { get; set; }
        public string PostPassword { get; set; }
        public string PostName { get; set; }
        public string ToPing { get; set; }
        public string Pinged { get; set; }
        public DateTime PostModified { get; set; }
        public DateTime postModified_gmt { get; set; }
        public string PostContentFiltered { get; set; }
        public decimal postParent { get; set; }
        public string Guid { get; set; }
        public int MenuOrder { get; set; }
        public string PostType { get; set; }
        public string PostMimeType { get; set; }
        public long CommentCount { get; set; }
    }
}
