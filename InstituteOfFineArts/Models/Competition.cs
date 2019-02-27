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
        public string CompetitionName { get; set; }
        public string Decription { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EndDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime AwardDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CompetitonStatus Status { get; set; }
        public string UserID { get; set; }
        public CustomUser User { get; set; }
        public List<CompetitionPost> CompetitionPosts { get; set; }
    }

    public enum CompetitonStatus
    {
        Ended = 0,
        During = 1,
        InComming = 2
    }
}
