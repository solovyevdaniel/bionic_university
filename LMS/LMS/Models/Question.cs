using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    using System.ComponentModel;

    public class Question
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Запитання")]
        public string Issue { get; set; }
        
        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
        public int TestId { get; set; }
        
        public virtual ICollection<Answer> Answers { get; set; }

        [DisplayName("Відповідей")]
        public int AnswerId { get; set; }

        public Question()
        {
            Answers = new List<Answer>();
        }
    }
}
