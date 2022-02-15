using Blog.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Blog.Data.DbModels
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

        [Required]
        public string Username { get; set; }

        [ForeignKey("ArticleId")]
        public virtual ICollection<Comment> Comments { get; set; }

        public static explicit operator Article(ArticleViewModel articleViewModel)
        {
            return new Article()
            {
                Id = articleViewModel.Id,
                Title = articleViewModel.Title,
                Text = articleViewModel.Text,
                Date = articleViewModel.Date,
                Username = articleViewModel.Username,
                Comments = articleViewModel.Comments?.Select(item => (Comment)item).ToList()
            };
        }
    }
}
