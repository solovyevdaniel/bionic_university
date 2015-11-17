using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class FactLink
    {
        [Key]
        public int ID { get; set; }

        public string Link { get; set; }     //nullable

        [ForeignKey("FactID")]
        public virtual Fact Fact { get; set; }
        public int FactID { get; set; }
    }
}
