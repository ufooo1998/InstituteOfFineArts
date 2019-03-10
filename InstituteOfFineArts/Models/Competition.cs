using InstituteOfFineArts.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InstituteOfFineArts.Models
{
    public class Competition
    {
        public Competition()
        {
        }
        public int ID { get; set; }

        [Required]
        [Display(Name ="Competition Name")]
        public string CompetitionName { get; set; }

        [Required]
        public string Decription { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd, dd/MM/yyyy}")]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd, dd/MM/yyyy}")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd, dd/MM/yyyy}")]
        [Display(Name = "Award Date")]
        public DateTime AwardDate { get; set; }

        [Display(Name = "Create Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd - dd/MM/yyyy - HH:mm}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Update")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dddd - dd/MM/yyyy - HH:mm}")]
        public DateTime UpdatedAt { get; set; }

        public DateTime DeletedAt { get; set; }

        public bool Available { get; set; }

        public CompetitonStatus Status { get; set; }

        public string UserID { get; set; }

        public CustomUser User { get; set; }

        public List<CompetitionPost> CompetitionPosts { get; set; }
    }

    public enum CompetitonStatus
    {
        Ended = 0,
        During = 1,
        Examining = 2,
        ComingUp = 3
    }
}
