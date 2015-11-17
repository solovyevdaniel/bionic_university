using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class Fact
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(50)]
        [Display(Name = "Назва")]
        public string Title { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Опис")]
        public string Description { get; set; }

        //public string UploadedFilePath { get; set; }   //nullable

        //public DateTime? FactFinish { get; set; }      //nullable

        [ForeignKey("Topic_ID")]
        public virtual Topic Topic { get; set; }
        public int? Topic_ID { get; set; }

        [ForeignKey("FactType_ID")]
        public virtual FactType FactType { get; set; }
        public int FactType_ID { get; set; }
        //public virtual ICollection<FactType> FactTypes { get; set; }
        //public virtual FactType FactType { get; set; }

        public virtual ICollection<FactUploadFile> FactUploadFiles { get; set; }

        public virtual ICollection<FactLink> FactLinks { get; set; }

        public Fact()
        {
            FactUploadFiles = new List<FactUploadFile>();
            FactLinks = new List<FactLink>();
        }
    }
}
