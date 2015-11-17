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
    public class FactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private String fileMapSource = "~/Files/FactFiles";
        // GET: Facts
        public async Task<ActionResult> Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var facts = db.Facts.Include(f => f.FactType).Include(f => f.Topic).Where(t=> t.Topic_ID == id);
            return View(await facts.ToListAsync());
        }

        // GET: Facts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fact fact = await db.Facts.FindAsync(id);
            if (fact == null)
            {
                return HttpNotFound();
            }

            //name of file
            var stringFormDb = db.FacttUploadFiles.Where(f => f.ID == id).Select(f => f.UploadFile).FirstOrDefault();
            if (stringFormDb != null)
            {
                string[] splitedStr = stringFormDb.Split(new Char[] { '/', '\\', '.' });
                var len = splitedStr.Count();
                var str = splitedStr[len - 2] + "." + splitedStr[len - 1];
                ViewBag.STRING = str;
            }

            return View(fact);
        }

        // GET: Facts/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.FactType_ID = new SelectList(db.FactTypes, "ID", "Name");
            ViewBag.Topic_ID = id;
            return View();
        }

        // POST: Facts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin, CourseManager")]
        public async Task<ActionResult> Create(/*[Bind(Include = "ID,Title,Description,Topic_ID,FactType_ID")]*/ Fact fact, string nameOfNewFactType, HttpPostedFileBase file, String fileSite)
        {
            ViewBag.Message = "";
            /*  if (ModelState.IsValid)
              {*/
            
            db.Facts.Add(fact);

            //adding new FactType
            if (fact.FactType_ID == 0 || (fact.FactType_ID == 1 && nameOfNewFactType != ""))
            {

                if (fact.FactType_ID == 0 && (nameOfNewFactType == null || nameOfNewFactType == ""))
                {
                    ViewBag.Message = "Введiть тип події";
                    ViewBag.FactType_ID = new SelectList(db.FactTypes, "ID", "Name", fact.FactType_ID);
                    ViewBag.Topic_ID = fact.Topic_ID;

                    return View(fact);
                }
                if (fact.FactType_ID != 0)
                {
                    //var allFactTypes = from element in db.FactTypes
                    //                   select db.FactTypes;

                    foreach (var item in db.FactTypes)
                    {
                        if (nameOfNewFactType == item.Name)
                        {
                            ViewBag.Message = "Дана подiя вже iснує";
                            ViewBag.FactType_ID = new SelectList(db.FactTypes, "ID", "Name", fact.FactType_ID);
                            ViewBag.Topic_ID = fact.Topic_ID;

                            return View(fact);
                        }
                    }
                }

                FactType newFactType = new FactType();
                newFactType.ID = fact.FactType_ID;
                newFactType.Name = nameOfNewFactType;
                db.FactTypes.Add(newFactType);
                db.SaveChanges();

            }

            //uploading files to server
            if (file != null)
            {
                fact.FactUploadFiles.Add(new FactUploadFile
                {
                    FactID = fact.ID,
                    UploadFile = Saver.SaveFileToFolder(file.FileName, file, Server.MapPath(fileMapSource))
                });
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Courses");
            /* }*/

            /*ViewBag.FactType_ID = new SelectList(db.FactTypes, "ID", "Name", fact.FactType_ID);
            ViewBag.Topic_ID = new SelectList(db.Topics, "ID", "Title", fact.Topic_ID);
            return View(fact);*/
        }

        // GET: Facts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fact fact = await db.Facts.FindAsync(id);
            if (fact == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = id;
            ViewBag.FactType_ID = new SelectList(db.FactTypes, "ID", "Name", fact.FactType_ID);
            ViewBag.Topic_ID = fact.Topic_ID;
            return View(fact);
        }

        // POST: Facts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(/*[Bind(Include = "ID,Title,Description,Topic_ID,FactType_ID")] */Fact fact, int id) 
        {

            /* if (ModelState.IsValid)
             {*/
            UpdateModel(fact.Description);
            //db.Entry(fact.Description).State = EntityState.Modified;
            //db.Facts.Add(fact);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Topics", new { id = fact.Topic_ID });
            /* }*/
            //ViewBag.FactType_ID = new SelectList(db.FactTypes, "ID", "Name", fact.FactType_ID);
            //ViewBag.Topic_ID = new SelectList(db.Topics, "ID", "Title", fact.Topic_ID);
            //return View(fact);
        }

        // GET: Facts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fact fact = await db.Facts.FindAsync(id);
            if (fact == null)
            {
                return HttpNotFound();
            }
            return View(fact);
        }

        // POST: Facts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Fact fact = await db.Facts.FindAsync(id);
            db.Facts.Remove(fact);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
