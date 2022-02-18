using System.ComponentModel.DataAnnotations;

namespace Blog.Data.DbModels
{
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Salt { get; set; }

        [Required]
        public Role Role { get; set; }
    }

    public enum Role
    {
        User,
        Admin
    }
}
