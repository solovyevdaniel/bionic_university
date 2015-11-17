using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class Comment
    {
        [Key]
        public int ID { get; set; }

        public string Commentary { get; set; }     //nullable

        [ForeignKey("TestID")]
        public virtual Test Test { get; set; }
        public int TestID { get; set; }
    }
}
