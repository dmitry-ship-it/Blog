using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class CommentViewModel
    {
        public string Username { get; set; }

        public string Text { get; set; }

        public DateTime DateCreated { get; set; }

        public int ArticleId { get; set; }

        public ICollection<CommentViewModel> Replies { get; set; }

        public int? OutsideCommentId { get; set; }
    }
}
