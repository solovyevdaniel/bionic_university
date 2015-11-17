using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LMS.Models;
using System.Net;
using LMS.Infrastructure;
using System.Data.Entity;

namespace LMS.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

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

        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index","Managers", new { id = user.Id });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/Index
        //public async Task<ActionResult> Index(string id)
        //{

        //    var user = await UserManager.FindByIdAsync(id);

        //    return View(user);
        //}

        //public async Task<ActionResult> Edit(string id)
        //{

        //    var user = await UserManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(user.UserInfo);
        //}

        ////
        //// POST: /Users/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,Image,FirstName,LastName,Birthday")] ApplicationUserInfo personalmodel, 
        //    HttpPostedFileBase newFile, String fileSite)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (ApplicationDbContext appContext = new ApplicationDbContext()) { 

        //        var applicationuser = await UserManager.FindByIdAsync(personalmodel.Id);
        //            if (applicationuser != null)
        //            {
        //                appContext.Entry(applicationuser).State = EntityState.Modified;
        //                if (UserHelper.SavePersonalFile(newFile, fileSite, personalmodel, Server.MapPath(imageMapSource), save: false))
        //                {
        //                    await appContext.SaveChangesAsync();
        //                    return RedirectToAction("Index", applicationuser.Id);
        //                }
        //                await appContext.SaveChangesAsync();
        //                return RedirectToAction("Index", applicationuser.Id);


        //            }
        //        }

        //    }
        //    ModelState.AddModelError("", "Something failed.");
        //    //return RedirectToAction("Edit", new { id = personalmodel.Id });
        //    return View();
        //}








        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        //private bool HasPhoneNumber()
        //{
        //    var user = UserManager.FindById(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        return user.PhoneNumber != null;
        //    }
        //    return false;
        //}

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}