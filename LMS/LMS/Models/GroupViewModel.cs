using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class GroupViewModel
    {
        public Group GroupData { get; set; }
        public Dictionary<ApplicationUser, Boolean> Learners { get; set; }
        public Dictionary<ApplicationUser,Boolean> Teachers { get; set;}
    }
}