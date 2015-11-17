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

namespace LMS.Controllers
{
    public class ManagersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private String imageMapSource = "~/Content/img/User";

        // GET: Managers
        public ActionResult Index(string id)
        {
            var applicationUserInfo = db.ApplicationUserInfos.Where(t => t.Id == id).FirstOrDefault();
            return View(applicationUserInfo);
        }

        // GET: Managers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUserInfo applicationUserInfo = await db.ApplicationUserInfos.FindAsync(id);
            if (applicationUserInfo == null)
            {
                return HttpNotFound();
            }
            return View(applicationUserInfo);
        }

      
        

        // GET: Managers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUserInfo applicationUserInfo = await db.ApplicationUserInfos.FindAsync(id);
            if (applicationUserInfo == null)
            {
                return HttpNotFound();
            }
            return View(applicationUserInfo);
        }

        // POST: Managers/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Image,FirstName,LastName,Birthday")] ApplicationUserInfo applicationUserInfo,
            HttpPostedFileBase newFile, String fileSite)
        {
            if (ModelState.IsValid)
            {
                if (UserHelper.SavePersonalFile(newFile, fileSite, applicationUserInfo, Server.MapPath(imageMapSource)))
                {
                    db.Entry(applicationUserInfo).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Managers", new { id = applicationUserInfo.Id });
                }
            }          
            return View(applicationUserInfo);
        }

       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
