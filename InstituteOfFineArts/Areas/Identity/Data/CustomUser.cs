using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Identity;

namespace InstituteOfFineArts.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the CustomUser class
    public class CustomUser : IdentityUser
    {
        public CustomUser()
        {
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;
            this.Status = AccountStatus.Activate;
        }
        public string Address { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AccountStatus Status { get; set; }
        public List<Competition> Competitions { get; set; }
        public List<Post> Posts { get; set; }
    }

    public enum AccountStatus
    {
        Inactivate = 0,
        Activate = 1
    }
}
