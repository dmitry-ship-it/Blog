using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class AddCommentModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        public int ArticleId { get; set; }

        public int? OutsideCommentId { get; set; }
    }
}
