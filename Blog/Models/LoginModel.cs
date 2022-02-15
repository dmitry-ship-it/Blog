using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        [StringLength(15, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
