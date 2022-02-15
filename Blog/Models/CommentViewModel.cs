using Blog.Data.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Blog.Models
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        public int ArticleId { get; set; }

        public int? OutsideCommentId { get; set; }

        public ICollection<CommentViewModel> Replies { get; set; }

        public static explicit operator CommentViewModel(Comment comment)
        {
            return new CommentViewModel
            {
                Id = comment.Id,
                Text = comment.Text,
                DateCreated = comment.DateCreated,
                Username = comment.Username,
                ArticleId = comment.ArticleId,
                OutsideCommentId = comment.OutsideCommentId,
                Replies = comment.Replies?.Select(item => (CommentViewModel)item).ToList()
            };
        }
    }
}
