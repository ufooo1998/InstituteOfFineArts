using InstituteOfFineArts.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstituteOfFineArts.Models
{
    public class Post
    {
        public int ID { get; set; }
        public string PostName { get; set; }
        public string Decription { get; set; }
        public int Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PostStatus Status  { get; set; }
        public string UserID { get; set; }
        public CustomUser User { get; set; }
        public List<CompetitionPost> CompetitionPosts { get; set; }
    }
    public enum PostStatus
    {
        Activate = 1,
        Inactivate = 0
    }
}
