using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class Topic
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(50)]
        [Display(Name ="Назва")]
        public string Title { get; set; }

        [Required, MaxLength(1000)]
        [Display(Name = "Опис")]
        public string Description { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "Початок")]
        public DateTime? Start { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Кінець")]
        public DateTime? Finish { get; set; }


        [ForeignKey("Course_ID")]
        public virtual Course Course { get; set; }
        public int Course_ID { get; set; }

        public virtual ICollection<Fact> Facts { get; set; }

        public virtual ICollection<Test> Tests { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        public Topic()
        {
            Facts = new List<Fact>();
            Tests = new List<Test>();
            Groups = new List<Group>();
        }
    }
}
