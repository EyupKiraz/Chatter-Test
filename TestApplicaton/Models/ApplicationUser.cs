using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApplicaton.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int GroupId { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Likes { get; set; }
        public string Dislikes { get; set; }

        public ApplicationUser()
        {
            Likes = "";
            Dislikes = "";
        }

    }
}
