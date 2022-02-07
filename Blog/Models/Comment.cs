using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
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

        // Outside comment id (if comment is reply to other comment)
        // add-migration generates this column in db to handle ICollection<Comment> Replies
        public int? CommentId { get; set; }

        public virtual ICollection<Comment> Replies { get; set; }
    }
}
