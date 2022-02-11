using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Data.DatabaseModels
{
    public class Comment
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Username")]
        public string User { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public int ArticleId { get; set; }

        [ForeignKey("OutsideCommentId")]
        public virtual ICollection<Comment> Replies { get; set; }

        public int? OutsideCommentId { get; set; }
    }
}
