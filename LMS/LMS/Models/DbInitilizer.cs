using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LMS.Models
{
    public class DbInitilizer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // создаем две роли
            var addAdminRole = new IdentityRole { Name = "Admin" };
            var addCourseManagerRole = new IdentityRole { Name = "CourseManager" };
            var addTeacherRole = new IdentityRole { Name = "Teacher" };
            var addLearnerRole = new IdentityRole { Name = "Learner" };

            // добавляем роли в бд
            roleManager.Create(addAdminRole);
            roleManager.Create(addCourseManagerRole);
            roleManager.Create(addTeacherRole);
            roleManager.Create(addLearnerRole);

            //context.Courses.Add(new Course { ID = 1, Formula = "asfasf", Title = "sdgsdg", Description = "sdgsdg" });
            //context.Courses.Add(new Course { ID = 2, Formula = "asfasf", Title = "sdgsdg", Description = "sdgsdg" });
            //context.Courses.Add(new Course { ID = 3, Formula = "asfasf", Title = "sdgsdg", Description = "sdgsdg" });
            //context.Courses.Add(new Course { ID = 4, Formula = "asfasf", Title = "sdgsdg", Description = "sdgsdg" });
            //context.Courses.Add(new Course { ID = 5, Formula = "asfasf", Title = "sdgsdg", Description = "sdgsdg" });
            //context.Courses.Add(new Course { ID = 6, Formula = "asfasf", Title = "sdgsdg", Description = "sdgsdg" });
            //context.Courses.Add(new Course { ID = 7, Formula = "asfasf", Title = "sdgsdg", Description = "sdgsdg" });

            context.CourseTags.Add(new CourseTag { ID = 1, Tag = "Math" });
            context.CourseTags.Add(new CourseTag { ID = 2, Tag = "Programming" });
            context.CourseTags.Add(new CourseTag { ID = 3, Tag = "Literature" });
            context.CourseTags.Add(new CourseTag { ID = 4, Tag = "Web" });


            context.UserStates.Add(new UserState { ID = 1, UState = 1 });
            context.UserStates.Add(new UserState { ID = 2, UState = 2 });
            context.UserStates.Add(new UserState { ID = 3, UState = 3 });
            context.UserStates.Add(new UserState { ID = 4, UState = 4 });

            context.FactTypes.Add(new FactType { ID = 1, Name = "Лабораторна робота" });
            context.FactTypes.Add(new FactType { ID = 2, Name = "Лекція" });
            context.FactTypes.Add(new FactType { ID = 3, Name = "Консультація" });
            //context.Topics.Add(new Topic { ID = 1, Course_ID = 1, Description = "tratata", Title = "Logarithm" });
            //context.CourseTags.Add(new CourseTag { ID = 5, Tag = "124124" });
            //context.CourseTags.Add(new CourseTag { ID = 6, Tag = "124124" });
            //context.CourseTags.Add(new CourseTag { ID = 7, Tag = "asfasfasf" });
            //context.CourseTags.Add(new CourseTag { ID = 8, Tag = "asfasfasf" });
            base.Seed(context);
        }
    }
}