using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstituteOfFineArts.Models
{
    public class CompetitionPost
    {
        public int ID { get; set; }
        public int CompetitionID { get; set; }
        public Competition Competition { get; set; }
        public int PostID { get; set; }
        public Post Post { get; set; }
        public int PostPoint { get; set; }
    }
}
