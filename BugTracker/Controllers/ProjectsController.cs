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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
            // start with a list of all projects
            var projects = db.Projects.ToList();

            // check to see if this user isn't an admin
            if (!User.IsInRole("Admin"))
            {
                // make an ApplicationUser from this user's Id
                var user = db.Users.Find(User.Identity.GetUserId());

                // filter the project list to only projects assigned to this user
                projects = projects.Where(p => p.Users.Contains(user)).ToList();
            }
                    
            return View(projects);
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
            var model = new UsersAndRolesModel();
            return View(model);
        }

        // partial view for listing all users for the purpose of adding to a project
        [Authorize(Roles = "Admin")]
        public ActionResult _AddUserToProject(ListUsersRolesModel model)
        {
            
            //model.Roles = userHelper.ListAllRoles();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AddUserToProject(string userId, int projectId)
        {
            var helper = new ProjectsHelper();
            helper.AddUserToProject(projectId, userId);
            return RedirectToAction("Details", new { id=projectId});
        }

        [Authorize(Roles = "Admin, Project Manager")]
        [HttpGet]
        public ActionResult RemoveProjectUser([Bind(Include = "ProjectId,UserId")] RemoveUserFromProjectModel model)
        {
            var helper = new ProjectsHelper();
            helper.RemoveUserFromProject(model.ProjectId, model.UserId);
            return RedirectToAction("Details", new { id = model.ProjectId });
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

        [Authorize(Roles = "Admin")]
        public void ChangeUserRole(string userId, string roleName, bool addRole)
        {
            var helper = new UserRolesHelper();
            if (addRole)
            { 
                helper.AddUserToRole(userId, roleName);
            }
            else
            {
                helper.RemoveUserFromRole(userId, roleName);
            }
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
        [Authorize (Roles = "Admin, Project Manager, Developer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User.Identity.GetUserId();
            var viewModel = new ProjectDetailViewModel(id, userId);

            if (viewModel.Project == null)
            {
                return HttpNotFound();
            }

            var helper = new ProjectsHelper();
            
            // deny access to non-admin users attempting to access projects to which they aren't assigned
            if (!User.IsInRole("Admin") && !helper.IsUserInProject(User.Identity.GetUserId(), id ?? 1))
            {
                return RedirectToAction("Index");
                
            }
            return View(viewModel);
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
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // testing something...
            var thisUser = new UserWithRoles(User.Identity.GetUserId());

            // instantiate projects helper class
            var projHelper = new ProjectsHelper();
            var urHelper = new UserRolesHelper();
            var userId = User.Identity.GetUserId();

            // kick this user back to the index if they aren't assigned to this project and don't have admin rights
            if (!projHelper.IsUserInProject(userId,id ?? 1) && !urHelper.IsUserInRole(userId,"Admin"))
            {
                return RedirectToAction("Index");
            }

            // build the view model (see the constructor for this model)
            var model = new ProjectDetailViewModel(id, userId);    

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string name)
        {
            if (ModelState.IsValid)
            {
                var project = db.Projects.Find(id);
                project.Name = name;
               
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit", new { id = id });
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
