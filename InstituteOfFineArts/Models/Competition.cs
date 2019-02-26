using InstituteOfFineArts.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstituteOfFineArts.Models
{
    public class Competition
    {
        public Competition()
        {
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;
            this.AwardDate = this.EndDate.AddDays(2);
            if (this.EndDate < DateTime.Now)
            {
                this.Status = CompetitonStatus.Ended;
            }

            if (this.StartDate < DateTime.Now && DateTime.Now < this.EndDate)
            {
                this.Status = CompetitonStatus.During;
            }

            if (this.StartDate > DateTime.Now)
            {
                this.Status = CompetitonStatus.InComming;
            }
        }
        public int ID { get; set; }
        public string CompetitionName { get; set; }
        public string Decription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
