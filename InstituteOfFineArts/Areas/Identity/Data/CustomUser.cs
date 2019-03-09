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
        }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        public Gender Gender { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AccountStatus Status { get; set; }
        public List<Competition> Competitions { get; set; }
        public List<Post> Posts { get; set; }
    }

    public enum Gender
    {
        Male = 1,
        Female = 0,
        Other = 2
    }

    public enum AccountStatus
    {
        Inactivate = 0,
        Activate = 1
    }
}
