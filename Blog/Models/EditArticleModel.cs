using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class EditArticleModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Article body is required.")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        public ICollection<CommentViewModel> Comments { get; set; }
    }
}
