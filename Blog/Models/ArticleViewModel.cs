using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class ArticleViewModel
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        public ICollection<CommentViewModel> Comments { get; set; }
    }
}
