using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Data.DatabaseModels
{
    public class Article
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
