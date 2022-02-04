using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Comment
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
    }
}
