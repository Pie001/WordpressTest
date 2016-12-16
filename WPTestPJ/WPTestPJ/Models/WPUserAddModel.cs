using System.ComponentModel.DataAnnotations;


namespace WPTestPJ.Web.Models
{
    public class WPUserAddModel
    {
        [Display(Name = "ログインID")]
        [Required]
        public string LoginID { get; set; }

        [Display(Name = "姓")]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "名前")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "メールアドレス")]
        [Required]
        public string Email { get; set; }
    }
}