using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class Course
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Назва")]
        public string Title { get; set; }

        [Required, MaxLength(1000)]
        [Display(Name = "Опис")]
        public string Description { get; set; }


        public string Image { get; set; }

        public virtual ICollection<CourseTag> CourseTags { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        public Course()
        {
            Topics = new List<Topic>();
            Groups = new List<Group>();
            CourseTags = new HashSet<CourseTag>();
        }
    }
}
