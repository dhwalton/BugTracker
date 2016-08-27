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
    [RequireHttps]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper userHelper = new UserRolesHelper();


        // GET: Projects
        [Authorize(Roles = "Admin, Developer, Project Manager, Demo Admin, Demo Developer, Demo Project Manager")]
        public ActionResult Index()
        {
            // start with a list of all projects
            var projects = db.Projects.ToList();
            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.isDemoUser())
            {
                projects = projects.Where(p => p.DemoProject == true).ToList();
            }


            // check to see if this user isn't an admin
            if (!User.IsInRole("Admin") && !User.IsInRole("Demo Admin"))
            {
                // only show the projects to which this user belongs
                
                projects = projects.Where(p => p.Users.Contains(user)).ToList();

                

                //// get this user's id
                //var userId = User.Identity.GetUserId();


                //// filter the project list to only projects managed by this user
                //projects = projects.Where(p => p.ManagerId == userId).ToList();
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
        [Authorize(Roles = "Admin, Demo Admin")]
        public ActionResult UserManager()
        {
            UsersAndRolesModel model = null;
            if (User.IsInRole("Admin"))
            {
                model = new UsersAndRolesModel(false);
                return View(model);
            }
            else
            {
                ViewBag.UserId = User.Identity.GetUserId();
                model = new UsersAndRolesModel(true);
                return View("UserManagerDemo", model);
            }
            
        }

        // partial view for listing all users for the purpose of adding to a project
        [Authorize(Roles = "Admin, Demo Admin")]
        public ActionResult _AddUserToProject(ListUsersRolesModel model)
        {
            
            //model.Roles = userHelper.ListAllRoles();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Demo Admin, Demo Project Manager")]
        public ActionResult AddUserToProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var helper = new ProjectsHelper();

            // assigns a manager to a project if one isn't there already
            if (helper.ManagerOfProject(projectId) == null && (user.IsPM() || user.inRole("Demo Project Manager")))
            {
                helper.AssignManagerToProject(projectId, userId);
            }

            helper.AddUserToProject(projectId, userId);
            
            return RedirectToAction("Edit", new { id = projectId});
        }

        [Authorize(Roles = "Admin, Project Manager, Demo Admin, Demo Project Manager")]
        [HttpGet]
        public ActionResult RemoveProjectUser([Bind(Include = "ProjectId,UserId")] RemoveUserFromProjectModel model)
        {
            var helper = new ProjectsHelper();
            helper.RemoveUserFromProject(model.ProjectId, model.UserId);
            return RedirectToAction("Edit", new { id = model.ProjectId });
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


        [Authorize (Roles = "Admin")]
        public ActionResult ChangeProjectManager(string userId, int projectId)
        {
            var h = new ProjectsHelper();
            
            // a null userId means that the PM is removed from the project
            if (userId == null)
            {
                // remove the user from the project list of users
                h.RemoveUserFromProject(projectId, h.GetManagerForProject(projectId).Id);
            }

            // assign (or remove) the manager
            h.AssignManagerToProject(projectId, userId);

            return RedirectToAction("Edit", new { id = projectId });
        }

        [Authorize(Roles = "Admin, Demo Admin")]
        public void ChangeUserRole(string userId, string roleName, bool addRole)
        {
            // It's not paranoia if they're really out to get you
            if (!User.IsInRole("Admin") && (!roleName.Contains("Demo") || roleName.Contains("Admin")))
            {
                return;
            }

            UserRolesHelper helper = new UserRolesHelper();
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

        //[Authorize(Roles = "Admin,Developer")]
        //public ActionResult Developer()
        //{
        //    //var ph = new ProjectsHelper(db);
        //    //var projects = ph.ProjectsAssignedToUser(User.Identity.Name);
        //    var uh = new UserRolesHelper();
        //    ViewBag.CurrentUser = uh.GetUserByName(User.Identity.Name);
            
        //    return View(db.Projects.ToList());
        //}

        //[Authorize(Roles = "Admin,Project Manager")]
        //public ActionResult ProjectManager()
        //{
        //    var uh = new UserRolesHelper();
        //    ViewBag.CurrentUser = uh.GetUserByName(User.Identity.Name);

        //    return View(db.Projects.ToList());
        //}

        // GET: Projects/Details/5
        [Authorize (Roles = "Admin, Project Manager, Developer, Demo Admin, Demo Project Manager, Demo Developer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User.Identity.GetUserId();
            var viewModel = new ProjectDetailViewModel(id, userId);

            if (User.IsInRole("Demo Admin") || User.IsInRole("Demo Project Manager") || User.IsInRole("Demo Developer"))
            {
                if (!viewModel.Project.DemoProject)
                {
                    return RedirectToAction("Index");
                }
            }

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
        [Authorize(Roles = "Admin, Project Manager, Demo Admin, Demo Project Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Project Manager, Demo Admin, Demo Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                if (User.IsInRole("Project Manager") || User.IsInRole("Demo Project Manager"))
                {
                    // project creators in the PM role are auto-assigned to the project
                    projects.ManagerId = user.Id;
                }
                if (user.isDemoUser()) projects.DemoProject = true;
                projects.StartDate = DateTimeOffset.Now;
                db.Projects.Add(projects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projects);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Project Manager, Demo Admin, Demo Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // instantiate projects helper class
            var projHelper = new ProjectsHelper();
            var urHelper = new UserRolesHelper();
            var userId = User.Identity.GetUserId();

            // kick this user back to the index if they aren't assigned to this project and don't have admin rights
            if (!projHelper.IsUserInProject(userId,id ?? 1) && !urHelper.IsUserInRole(userId,"Admin") && !urHelper.IsUserInRole(userId,"Demo Admin"))
            {
                return RedirectToAction("Index");
            }

            // build the view model (see the constructor for this model)
            var model = new ProjectDetailViewModel(id, userId);

            // prevent demo users from editing "real" projects
            if (User.IsInRole("Demo Admin") || User.IsInRole("Demo Project Manager"))
            {
                if (!model.Project.DemoProject)
                {
                    return RedirectToAction("Index");
                }
            }

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Project Manager, Demo Project Manager, Demo Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string name)
        {
            if (ModelState.IsValid)
            {
                var project = db.Projects.Find(id);
                project.Name = name;
                if (User.IsInRole("Demo Admin") || User.IsInRole("Demo Project Manager")) project.DemoProject = true;

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
