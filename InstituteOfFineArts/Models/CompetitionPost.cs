using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InstituteOfFineArts.Models
{
    public class CompetitionPost
    {
        public int ID { get; set; }

        [Required]
        public int CompetitionID { get; set; }

        public Competition Competition { get; set; }

        [Required]
        public int PostID { get; set; }

        public Post Post { get; set; }

        [Required]
        [Display(Name = "Post Point")]
        public int PostPoint { get; set; }

        [Required]
        [Display(Name = "Author")]
        public string UserID { get; set; }

        [Required]
        [Display(Name = "Submit Date")]
        public DateTime SubmitDate { get; set; }

        public bool Available { get; set; }

        public DateTime DeletedAt { get; set; }

    }
}
