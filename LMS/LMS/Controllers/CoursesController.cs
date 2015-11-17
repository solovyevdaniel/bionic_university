using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using LMS.Infrastructure;

namespace LMS.Controllers
{
    public class CoursesController : AsyncController
    {
        private ApplicationDbContext appContext = new ApplicationDbContext();

        CourseSearchHelper coursesSearch = new CourseSearchHelper();
        private String imageMapSource = "~/Content/img/Courses";
        /*
         * Returns courses to user
         * CourseSearchViewModel:
         * 1.PagesInfo:
         * currentPage - current coursees page - first = 0,
         * ItemsPerPage - count of courses on one page
         * TotalItems - total avaible courses in DB
         * 2.Courses - all
         */
        public async Task<ActionResult> Index(Int32 currentPage = 0)
        {
            appContext.Courses
                .Include(c => c.CourseTags)
                .Include(c => c.Topics);
                CourseSearchViewModel csVm = new CourseSearchViewModel
                {
                    Courses = await CourseSearchHelper.GetCoursesOnPageAsync(appContext, currentPage),
                    PInfo = new PagesInfo
                    {
                        CurrentPage = currentPage,
                        ItemsPerPage = CourseSearchHelper.ItemsOnPageCount,
                        TotalItems = await appContext.Courses.CountAsync()
                    }
                };
                return View(csVm);
        }

        /*
         * returns course details - Main course page
         * Uses CourseViewModel
         */
        public async Task<ActionResult> Details(Int32? id, String join = null)
        {
            if (id != null)
            {
                Course course = await appContext.Courses.FindAsync(id);
                if (course != null)
                {
                    ViewBag.Join = join;
                    await CourseHelper.InitializeCourseDetailsComponentsAsync(course, appContext);
                    CourseViewModel cVm = new CourseViewModel
                    {
                        CourseElement = course,
                        Tags = course.CourseTags
                    };
                    return View(cVm);
                }
            }
            return RedirectToAction("Index");
        }

        /*
         * Creates a new course
         * CourseTags - all course tags from DB
         */
        [Authorize(Roles = "Admin, CourseManager")]
        [ValidateInput(false)]
        public async Task<ActionResult> Create()
        {
            if (TempData["error"] != null)
                ViewBag.ErrorMessage = TempData["error"];
            CourseViewModel cVm = new CourseViewModel
            {
                CourseElement = TempData["course"] != null ? TempData["course"] as Course : new Course(),
                Tags = await appContext.CourseTags.ToArrayAsync()
            };
            return View(cVm);
        }
   
        
        /*
         * creates course with image and tags
         * if smth wrong redirects to create
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Admin, CourseManager")]
        public async Task<ActionResult> Create([Bind(Include="ID,Title,Description,Formula,Image")] Course course, 
            HttpPostedFileBase file, String fileSite, String tags)
        {
            if (ModelState.IsValid)
            {
                IList<CourseTag> cTags = null;

                if (CourseHelper.GetTagsFromStringOrCreate(tags, appContext, ref cTags))
                    await appContext.SaveChangesAsync();
                
                if (cTags != null && cTags.Count != 0)
                {
                    /*can call an exception if someone will change CourseTags collection type*/
                    (course.CourseTags as HashSet<CourseTag>).UnionWith(cTags);

                    if (CourseHelper.SaveCourseFile(file, fileSite, course, Server.MapPath(imageMapSource), save: false))
                    {
                        appContext.Courses.Add(course);
                        await appContext.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = course.ID });
                    }
                }
            }
            TempData["error"] = "not all fields are filled";
            TempData["course"] = course;
            return RedirectToAction("Create");
        }

        /*
         * edit course info
         */
        [Authorize(Roles = "Admin, CourseManager,Teacher")]
        public async Task<ActionResult> Edit(Int32? id)
        {
            if (id != null)
            {
                Course course = await appContext.Courses.FindAsync(id);
                if (course != null)
                {
                    CourseViewModel cVm = new CourseViewModel
                    {
                        Tags = await appContext.CourseTags.ToArrayAsync(), 
                        CourseElement = course
                    };
                    return View(cVm);
                }
            }
            return RedirectToAction("Index");
        }

            /*
             * post edit:
             * newFile - users file form localPC
             * fileSite - url
             * tags - new course tags
             */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Admin, CourseManager,Teacher")]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Title,Description,Formula, Image")] Course CourseElement,
            HttpPostedFileBase newFile, String fileSite, String tags)
        {
            if (ModelState.IsValid && tags.Length != 0)
            {
                IList<CourseTag> foundTags = null;

                if (CourseHelper.GetTagsFromStringOrCreate(tags, appContext, ref foundTags))
                    await appContext.SaveChangesAsync();

                if (foundTags.Count != 0)
                {
                    appContext.Entry(CourseElement).State = EntityState.Modified;
                    await appContext.Entry(CourseElement).Collection(c => c.CourseTags).LoadAsync();

                    CourseHelper.RemoveTagsFromCourse(CourseElement);
                    await appContext.SaveChangesAsync();

                    CourseElement.CourseTags = foundTags;

                    if (CourseHelper.SaveCourseFile(newFile, fileSite, CourseElement, Server.MapPath(imageMapSource), save: false))
                    {
                        await appContext.SaveChangesAsync();
                        return RedirectToAction("Details", new { Id = CourseElement.ID });
                    }
                }
            }
            return RedirectToAction("Edit", new { id = CourseElement.ID });
        }
        /*delete course with current ID*/

        [Authorize(Roles = "Admin, CourseManager")]
        public async Task<ActionResult> Delete(Int32? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await appContext.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        /*confirm delete action*/

        [Authorize(Roles = "Admin, CourseManager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Int32 id)
        {
            Course course = await appContext.Courses.FindAsync(id);
            appContext.Courses.Remove(course);
            await appContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
 
        /*
         * Child action - returns form with search input
         */
        [HttpGet]
        [ChildActionOnly]
        public ActionResult SearchIndex()
        {
            return PartialView("SearchTools");
        }
        /*
         * search view to search course by three categories : all, tags, names
         */
        public async Task<ActionResult> Search(String query = "all", String search = "all", Boolean showAll = true, Int32 currentPage = 0) // default: search by all
        {
            ViewBag.Query = query;
            ViewBag.ShowAll = showAll;
            ViewBag.Search = search;
            IEnumerable<Course> courses = showAll ? await coursesSearch.SearchCoursesAsync(search, query, appContext)
                : await coursesSearch.SearchCoursesAsync(search, query, appContext, currentPage);
            if (courses != null)
            {
                CourseSearchViewModel csVm = new CourseSearchViewModel
                {
                    Courses = courses,
                    PInfo = showAll ? null : new PagesInfo
                    {
                        CurrentPage = currentPage,
                        ItemsPerPage = CourseSearchHelper.ItemsOnPageCount,
                        TotalItems = coursesSearch.ItemsCount
                    }
                };
                return View(csVm);
            }
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Enter(String userName, Int32? id)
        {
            try
            {
                if (id != null)
                {
                    Course course = await appContext.Courses.FindAsync(id);
                    await appContext.Users.LoadAsync();
                    ApplicationUser user = await appContext.Users.Where(u => String.Compare(u.UserName, userName, true) == 0).FirstAsync();
                    if (user != null)
                    {
                        if (user.Groups.Any(g => g.Course_ID == id && g.Name != null))
                            return RedirectToAction("Index","Topics",new { id = course.ID });
                        else
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Sorry, but you can't enter this course");
                    }
                }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Can't find user, please try to log in");
            }
            catch (InvalidOperationException)
            {
                //first error
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Can't find user, please try to log in");
            }
        }
        public async Task<ActionResult> Join(String userName, Int32? id)
        {
            try
            {
                if (id != null)
                {
                    Course course = await appContext.Courses.FindAsync(id);
                    await appContext.Users.LoadAsync();
                    ApplicationUser user = await appContext.Users.Where(u => String.Compare(u.UserName, userName, false) == 0).FirstAsync();
                    if (user != null)
                    {
                        if (!await appContext.Groups.AnyAsync(g => g.Course_ID == id && String.Compare(g.User.Id, user.Id, false) == 0))
                        {
                            Group group = new Group
                            {
                                Course_ID = course.ID,
                                Course = course,
                                UserState_ID = (Int32)CourseUsers.Learner,
                                User = user,
                                Duration = DateTime.Now,
                                Start = DateTime.Now,
                                Finish = DateTime.Now,
                                TotalScore = 0
                            };
                            appContext.Groups.Add(group);
                            await appContext.SaveChangesAsync();
                            return RedirectToAction("Details", new { id = course.ID, join = "Вашу заявку прийнято." });
                        }
                        else if (user.Groups.Any(g => g.Course_ID == id && g.Name != null))
                            return RedirectToAction("Index", "Topics", new { id = course.ID });
                    }
                    return RedirectToAction("Details", new { id = course.ID, join = "Ви вже подавали заявку." });
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Can't find this course , please, try again later!");
            }
            catch (InvalidOperationException)
            {
                //first error
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Can't find user, please try to log in");
            }
        }
        [HttpGet]
        public async Task<ActionResult> ApplyUsers(Int32? courseId)
        {
            ViewBag.UserStats = new SelectList(await appContext.UserStates.ToArrayAsync());
            if (courseId != null)
            {
                Course course = await appContext.Courses.FindAsync(courseId);
                if (course != null)
                {
                    appContext.Groups.Include(g => g.User);
                    IEnumerable<ApplicationUser> users = await appContext.Groups
                        .Where(g => g.Course_ID == course.ID && g.UserState_ID == 1)
                        .Select(g=>g.User)
                        .ToArrayAsync();
                    if (users != null)
                        return View(users);
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                appContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
