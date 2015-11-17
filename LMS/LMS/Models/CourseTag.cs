using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LMS.Models
{
    public class CourseTag
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(15)]
        public string Tag { get; set; }

        public virtual ICollection<Course> Courses { get; set; }

    }
}
