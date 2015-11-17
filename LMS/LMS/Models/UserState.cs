using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class UserState
    {
        [Key]
        public int ID { get; set; }

        public int UState { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        public UserState()
        {
            Groups = new List<Group>();
        }
    }
}
