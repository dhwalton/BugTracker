using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using System.Web.Security;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper userHelper = new UserRolesHelper();


        // GET: Projects
        [Authorize(Roles="Admin,Developer,Project Manager")]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.Projects.ToList());
            }
            else if (User.IsInRole("Project Manager"))
            {
                return RedirectToAction("ProjectManager");
            }
            else if (User.IsInRole("Developer"))
            {
                return RedirectToAction("Developer");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UserRoles()
        {
            var users = db.Users.ToList().OrderBy(u => u.LastName);
            return View(users);
        }

        // ******* THIS IS AN ALTERNATIVE TO THE UserRoles() METHOD ********* 
        [Authorize(Roles = "Admin")]
        public ActionResult UserManager()
        {
            var users = db.Users.ToList().OrderBy(u => u.LastName);
            return View(users);
        }

        public ActionResult EditUserRoles(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            UserRolesHelper helper = new UserRolesHelper();
            var selected = helper.ListUserRoles(id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.User = user;

            return View(AdminModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUserRoles(AdminUserViewModel model, string id)
        {
            //var user = User.Identity.GetUserId();
            UserRolesHelper helper = new UserRolesHelper();
            //var currentRoles = helper.ListUserRoles(id);
            if (model.SelectedRoles == null)
            {
                model.SelectedRoles = new string[] { "" };
            }
            foreach (var role in db.Roles.Select(r => r.Name))
            {
                if (model.SelectedRoles.Contains(role))
                {
                    helper.AddUserToRole(id, role);
                }
                else
                {
                    helper.RemoveUserFromRole(id, role);
                }
            }

            return RedirectToAction("UserRoles");
        }

        [Authorize(Roles = "Admin,Developer")]
        public ActionResult Developer()
        {
            //var ph = new ProjectsHelper(db);
            //var projects = ph.ProjectsAssignedToUser(User.Identity.Name);
            var uh = new UserRolesHelper();
            ViewBag.CurrentUser = uh.GetUserByName(User.Identity.Name);
            
            return View(db.Projects.ToList());
        }

        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult ProjectManager()
        {
            var uh = new UserRolesHelper();
            ViewBag.CurrentUser = uh.GetUserByName(User.Identity.Name);

            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                projects.StartDate = DateTimeOffset.Now;
                db.Projects.Add(projects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projects);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var projHelper = new ProjectsHelper(db);

            AdminProjectEditModel p = new AdminProjectEditModel();
            p.Project = db.Projects.Find(id);
            
            p.Users = projHelper.UsersInProject(id);

            //Projects projects = db.Projects.Find(id);
            if (p == null)
            {
                return HttpNotFound();
            }
            return View(p);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StartDate")] Projects projects)
        {
            
            if (ModelState.IsValid)
            {
                db.Entry(projects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projects);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Projects projects = db.Projects.Find(id);
            db.Projects.Remove(projects);
            db.SaveChanges();
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
