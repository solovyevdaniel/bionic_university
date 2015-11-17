using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class AnswersByUser
    {
        [Key]
        public int Id { get; set; }
        
        public int TotalScoreForTest { get; set; }
        
        public DateTime DeliveryDate { get; set; }

        public virtual Test Test {get; set; }
        public int TestId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
