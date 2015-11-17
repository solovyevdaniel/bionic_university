using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Text;
namespace LMS.Models
{
    /*
     * Course View model :
     * contains:
     * 1. Tags - collection of all tags in DB (for adding new tag to course)
     * 2. CourseElement - course to display on page
     */
    sealed public  class CourseViewModel
    {
        public IEnumerable<CourseTag> Tags { get; set; }
        public Course CourseElement { get; set; }
        
        /*
         * Return string of all course tags IDs separated with separator
         * Required for displaying course tags in selectize.js
         */
        public String TagsIDToString(Char separator = ',')
        {
            StringBuilder sBuild = new StringBuilder();
            if (CourseElement.CourseTags != null && CourseElement.CourseTags.Count != 0)
            {
                foreach (var i in CourseElement.CourseTags)
                    sBuild.Append(i.ID.ToString() + separator);
                String tags = sBuild.ToString();
                return tags.Remove(tags.Length - 1);
            }
            return String.Empty;
        }
        /*
         * Return string of all course tags Names separated with separator
         * Required for displaying course tags in selectize.js
         */
        public String TagsNamesToString(Char separator = ',')
        {
            StringBuilder sBuild = new StringBuilder();
            if (CourseElement != null && CourseElement.CourseTags != null && CourseElement.CourseTags.Count() != 0)
            {
                foreach (var i in CourseElement.CourseTags)
                    sBuild.Append(i.Tag.ToString() + separator);
                String tags = sBuild.ToString();
                return tags.Remove(tags.Length - 1);
            }
            return String.Empty;
        }
    }
}