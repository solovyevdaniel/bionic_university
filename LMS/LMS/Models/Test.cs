using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LMS.Models
{
    public class Test
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

      
        [DisplayName("Назва")]
        public string TestTitle { get; set; }

    
        [DisplayName("Необхідно балів %")]
        public int MinScore { get; set; }

        
        [ForeignKey("Topic_ID")]
        public virtual Topic Topic { get; set; }
        public int? Topic_ID { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public int CommentId { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public int QuestionId { get; set; }

        public virtual ICollection<AnswersByUser> AnswersByUsers { get; set; }
        public int AnswersByUserId { get; set; }

        public Test()
        {
            Comments = new List<Comment>();
            Questions = new List<Question>();
            AnswersByUsers = new List<AnswersByUser>();
        }
    }
}