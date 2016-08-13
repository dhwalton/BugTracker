using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ProjectDetailViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper helper = new ProjectsHelper();
        private UserRolesHelper urHelper = new UserRolesHelper();

        public ProjectDetailViewModel(int? projectId, string userId)
        {
            if (projectId != null) {
                // instantiate user from user Id
                var user = db.Users.Find(userId);

                // instantiate project from project Id
                Project = db.Projects.Find(projectId);

                // make a list of this user's assigned tickets for this project
                AssignedTickets = Project.Tickets.Where(t => t.AssignedUser == user).ToList();

                // make a list of all the other tickets for this project
                UnassignedTickets = Project.Tickets.Where(t => t.AssignedUser != user).ToList();

                //make a list of the users in the correct roles for project assignment
                IEnumerable<ApplicationUser> usersInProperRoles = null; // start at null, see below
                if (urHelper.IsUserInRole(userId, "Admin"))
                    {
                        // admins can assign developers and PMs
                        usersInProperRoles = urHelper.UsersInRole("Developer")
                            .Union(urHelper.UsersInRole("Project Manager"));
                    }
                    else if (urHelper.IsUserInRole(userId, "Project Manager"))
                    {
                        // PMs can assign developers
                        usersInProperRoles = urHelper.UsersInRole("Developer");
                    }

                // a null list means this user is not an Admin/PM, they will not have access to
                // the ability to add/remove users from projects
                if (usersInProperRoles != null) { 
                    // join the users not in project with those in proper roles to the users in proper roles
                    UsersNotInProject = from user1 in helper.UsersNotInProject(projectId ?? 1)
                                        join user2 in usersInProperRoles
                                        on user1.Id equals user2.Id
                                        select user1;
                }
                
            }
        }

        public Projects Project { get; set; }
        public IList<Tickets> AssignedTickets { get; set; }
        public IList<Tickets> UnassignedTickets { get; set; }
        public IEnumerable<ApplicationUser> UsersNotInProject { get; set; }
    }
}