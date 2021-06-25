using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApplicaton.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int GroupId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
