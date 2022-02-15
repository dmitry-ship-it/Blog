using Blog.Data.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Blog.Models
{
    public class ArticleViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        public ICollection<CommentViewModel> Comments { get; set; }

        public static explicit operator ArticleViewModel(Article article)
        {
            return new ArticleViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Text = article.Text,
                Date = article.Date,
                Username = article.Username,
                Comments = article.Comments?.Select(item => (CommentViewModel)item).ToList()
            };
        }
    }
}
