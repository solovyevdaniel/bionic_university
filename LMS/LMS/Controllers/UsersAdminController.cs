using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using LMS;
using LMS.Infrastructure;

namespace LMS.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class UsersAdminController : Controller
    {
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        ApplicationDbContext appContext = new ApplicationDbContext();
        //
        // GET: /Users/
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        //// GET: /Users/Create
        //public async Task<ActionResult> Create()
        //{
        //    //Get the list of Roles
        //    ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
        //    return View();
        //}

        ////
        //// POST: /Users/Create
        //[HttpPost]
        //public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
        //        var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

        //        //Add User to the selected Roles 
        //        if (adminresult.Succeeded)
        //        {

        //            if (selectedRoles != null)
        //            {
        //                var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
        //                if (!result.Succeeded)
        //                {
        //                    ModelState.AddModelError("", result.Errors.First());
        //                    ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
        //                    return View();
        //                }

        //            }
        //            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return View("DisplayEmail");

        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", adminresult.Errors.First());
        //            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
        //            return View();

        //        }

        //        //return RedirectToAction("Index");
        //    }
        //    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
        //    return View();
        //}

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                IList<String> roles = await UserManager.GetRolesAsync(editUser.Id);
                ApplicationUser appUser = appContext.Users.Where(u => String.Compare(u.Email, editUser.Email, false) == 0).FirstOrDefault();
                if (appUser != null)
                {
                    if (roles != null && roles.Count > 0)
                    {
                        Group[] groups = appUser.Groups.ToArray();
                        foreach (var gr in groups)
                        {
                            if (roles.Contains("Admin") || roles.Contains("CourseManager"))
                                appContext.Groups.Remove(gr);
                            else if ((roles.Contains("Teacher") && gr.UserState_ID == (Int32)CourseUsers.Student) ||
                                (roles.Contains("Learner") && gr.UserState_ID == (Int32)CourseUsers.Teacher))
                            {
                                gr.Name = null;
                                gr.UserState_ID = (Int32)CourseUsers.Learner;
                                appContext.Entry(gr).State = EntityState.Modified;
                            }
                        }
                        await appContext.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}