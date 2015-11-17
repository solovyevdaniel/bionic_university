using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    /*
     * course search view model
     * Courses - all found courses
     * PInfo - pages info
     */
    public class CourseSearchViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public PagesInfo PInfo { get; set; }
    }
}