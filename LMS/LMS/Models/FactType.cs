using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class FactType
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        //[ForeignKey("FactID")]
        //public virtual Fact Fact { get; set; }
        //public int FactID { get; set; }

        //public virtual Fact Fact { get; set; }
        public virtual List<Fact> Facts { get; set; }
    }
}
