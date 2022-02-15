using Blog.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Blog.Data.DbModels
{
    public class Comment
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public int ArticleId { get; set; }

        [ForeignKey("OutsideCommentId")]
        public virtual ICollection<Comment> Replies { get; set; }

        public int? OutsideCommentId { get; set; }

        public static explicit operator Comment(CommentViewModel commentViewModel)
        {
            return new Comment()
            {
                Id = commentViewModel.Id,
                Username = commentViewModel.Username,
                Text = commentViewModel.Text,
                DateCreated = commentViewModel.DateCreated,
                ArticleId = commentViewModel.ArticleId,
                Replies = commentViewModel.Replies?.Select(item => (Comment)item).ToList(),
                OutsideCommentId = commentViewModel.OutsideCommentId
            };
        }
    }
}

