using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; }
        
        public bool IsCorect { get; set; }
        
        public virtual Question Question { get; set; }
        public int QuestionId { get; set; }
        
    }
}
