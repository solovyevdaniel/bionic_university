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
using LMS.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LMS.Controllers
{
    [Authorize(Roles = "Admin, CourseManager")]
    public class GroupController : AsyncController
    {
        private ApplicationDbContext appContext = new ApplicationDbContext();
        private CourseManagerActions cMa = new CourseManagerActions();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        /*
         * Returns JSON values of current users
         * user - unapplied user
         */
        public async Task<ActionResult> GetUsersForGroup(Int32? id, String role)
        {
            if (id != null)
            {
                Group group = await appContext.Groups.FindAsync(id);
                if (Request.IsAjaxRequest())
                {
                    CourseUsers cUser;
                    /*if null then search by learners and students*/
                    List<ApplicationUser> users = new List<ApplicationUser>();
                    if (role != null && Enum.TryParse(role, out cUser))
                    {
                        Course groupCourse = await appContext.Courses.FindAsync(group.Course_ID);
                        users.AddRange(await cMa.GetUserWithDefRoleAndCurrCourse(appContext, UserManager, groupCourse, group.Name, role, role, "Learner"));
                        ViewBag.Names = "allowedTeachers";
                    }
                    else if (role == null)
                    {
                        users.AddRange(await cMa.GetUsersFromMainGroupAsync(appContext, group, CourseUsers.Learner));
                        users.AddRange(await cMa.GetUsersFromMainGroupAsync(appContext, group, CourseUsers.Student, true));
                        users = new List<ApplicationUser>(await cMa.FilterUsersByRole(users, "Learner", UserManager));
                        role = Enum.GetName(typeof(CourseUsers), CourseUsers.Student);
                        ViewBag.Names = "allowedLearners";
                    }
                    if (users.Count != 0)
                    {
                        Dictionary<ApplicationUser, Boolean> usersStatuses = new Dictionary<ApplicationUser, Boolean>();
                        users.ForEach(u => usersStatuses.Add(u, cMa.CheckUserForRoleInMainGroup(u, group, role)));
                        return PartialView("GroupShared/CoursesUsersPartial", usersStatuses);
                    }
                }
            }
            return PartialView("GroupShared/CoursesUsersPartial", null);
        }
        public async Task<ActionResult> Index()
        {
            var groups = appContext.Groups.Include(g => g.Course).Include(g => g.Topic).Include(g => g.UserState);
            return View(await groups.Where(g => g.UserState_ID == (Int32)CourseUsers.Main).ToListAsync());
        }

        // GET: /Group/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id != null)
            {
                Group group = await appContext.Groups.FindAsync(id);
                if (group != null)
                {
                    Dictionary<ApplicationUser, Boolean> learners = new Dictionary<ApplicationUser, Boolean>();
                    Dictionary<ApplicationUser, Boolean> teachers = new Dictionary<ApplicationUser, Boolean>();
                    (await cMa.GetUsersFromMainGroupAsync(appContext, group, CourseUsers.Student, true))
                                    .ToList()
                                    .ForEach(u => learners.Add(u, cMa.CheckUserForRoleInMainGroup(u, group, "Student")));
                    (await cMa.GetUsersFromMainGroupAsync(appContext, group, CourseUsers.Teacher, true))
                                    .ToList()
                                    .ForEach(u => teachers.Add(u, cMa.CheckUserForRoleInMainGroup(u, group, "Teacher")));
                    GroupViewModel gVm = new GroupViewModel
                    {
                        GroupData = group,
                        Learners = learners,
                        Teachers = teachers
                    };

                    return View(gVm);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Can't find requested group");
        }
        // GET: /Group/Create
        public async Task<ActionResult> Create(Int32? id, String error = null)
        {
            if (id != null)
            {
                Course course = await appContext.Courses.FindAsync(id);
                if (course != null)
                {
                    ViewBag.Course_ID = id;
                    ViewBag.Error = error;
                    ViewBag.UserState_ID = (Int32)CourseUsers.Main;
                    return View();
                }
            }
            return RedirectToAction("Index", "Courses", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,Start,Finish,Visible,Course_ID,UserState_ID")] Group group)
        {
            if (ModelState.IsValid)
            {
                if (group.Course_ID != null && !await appContext.Groups.AnyAsync(g => String.Compare(g.Name, group.Name, false) == 0))
                {
                    if (group.Start > group.Finish)
                    {
                        DateTime swapper = group.Start;
                        group.Start = group.Finish;
                        group.Finish = swapper;
                    }
                    group.Duration = DateTime.Now;
                    appContext.Groups.Add(group);
                    await appContext.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = group.ID });
                }
                return RedirectToAction("Create", new { id = group.Course_ID, error = "Группу з таким ім'ям вже створено!" });
            }
            return RedirectToAction("Create", new { id = group.Course_ID, error = "Деякі поля заповнені неправильно" });
        }

        // GET: /Group/Edit/5
        public async Task<ActionResult> Edit(Int32? id, String error = null)
        {
            if (id != null)
            {
                Group group = await appContext.Groups.FindAsync(id);
                if (group != null && group.UserState_ID == (Int32)CourseUsers.Main)
                {
                    ViewBag.Error = error;
                    TempData["oldName"] = group.Name;
                    return View(group);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Start,Finish,Visible,Course_ID,UserState_ID")] Group group,
            IEnumerable<String> allowedLearners, IEnumerable<String> allowedTeachers, Boolean lWatched, Boolean tWatched)
        {
            if (ModelState.IsValid)
            {
                if (String.Compare(TempData["oldName"] as String, group.Name, false) == 0 || !await appContext.Groups.AnyAsync(g => String.Compare(g.Name, group.Name, false) == 0))
                {
                    appContext.Entry(group).State = EntityState.Modified;
                    if (group.Start > group.Finish)
                    {
                        DateTime swapper = group.Start;
                        group.Start = group.Finish;
                        group.Finish = swapper;
                    }
                    group.Duration = DateTime.Now;
                    await cMa.RefreshAllGroupsDataAsync(TempData["oldName"] as String, group, appContext);
                    if (lWatched)
                        await cMa.RefreshGroupOfUsersTypeAsync(allowedLearners, group, appContext, CourseUsers.Student, CourseUsers.Learner);
                    if (tWatched)
                        await cMa.RefreshGroupOfUsersTypeAsync(allowedTeachers, group, appContext, CourseUsers.Teacher, CourseUsers.Learner);
                    await appContext.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = group.ID });
                }
                return RedirectToAction("Edit", new { id = group.ID, error = "Группу з таким ім'ям вже створено! " });
            }
            return RedirectToAction("Edit", new { id = group.ID, error = "Деякі поля заповнені неправильно" });
        }

        // GET: /Group/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = await appContext.Groups.FindAsync(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: /Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Group group = await appContext.Groups.FindAsync(id);
            await appContext.Groups
                .Where(g => String.Compare(g.Name, group.Name, false) == 0)
                .ForEachAsync(g => { g.UserState_ID = (Int32)CourseUsers.Learner; g.Name = null; appContext.Entry(g).State = EntityState.Modified; });
            appContext.Groups.Remove(group);
            await appContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                appContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

