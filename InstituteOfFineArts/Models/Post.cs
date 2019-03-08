using InstituteOfFineArts.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InstituteOfFineArts.Models
{
    public class Post
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Post Name")]
        public string PostName { get; set; }

        [Display(Name = "Image")]
        public byte[] Image { get; set; }

        [Required]
        [Display(Name = "Decription")]
        public string Decription { get; set; }

        [Display(Name = "Start Date")]
        public int Price { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd - dd/MM/yyyy - HH:mm}")]
        [Display(Name = "Create Date")]
        public DateTime CreatedAt { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd - dd/MM/yyyy - HH:mm}")]
        [Display(Name = "Last Update")]
        public DateTime UpdatedAt { get; set; }

        [Display(Name = "Author")]
        public string UserID { get; set; }
        public CustomUser User { get; set; }
        public List<CompetitionPost> CompetitionPosts { get; set; }
    }
}
