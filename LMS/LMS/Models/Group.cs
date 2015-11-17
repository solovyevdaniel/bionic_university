using System;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class Group
    {
        [Key]
        public int ID { get; set; }
        
        public string Name { get; set; }

        public int TotalScore { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime Start { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Finish { get; set; }

        [DataType(DataType.Duration)]
        public DateTime Duration { get; set; }

        [HiddenInput(DisplayValue = false)]
        public bool Visible { get; set; }

        [ForeignKey("Course_ID")]
        public virtual Course Course { get; set; }
        public int? Course_ID { get; set; }

        [ForeignKey("Topic_ID")]
        public virtual Topic Topic { get; set; }
        public int? Topic_ID { get; set; }

        [ForeignKey("UserState_ID")]
        public virtual UserState UserState { get; set; }
        public int UserState_ID { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
