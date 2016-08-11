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

        public ProjectDetailViewModel(int? projectId, string userId)
        {
            if (projectId != null) { 
                var user = db.Users.Find(userId);
                Project = db.Projects.Find(projectId);
                AssignedTickets = Project.Tickets.Where(t => t.AssignedUser == user).ToList();
                UnassignedTickets = Project.Tickets.Where(t => t.AssignedUser != user).ToList();
                UsersNotInProject = helper.UsersNotInProject(projectId ?? 1);
            }
        }

        public Projects Project { get; set; }
        public IList<Tickets> AssignedTickets { get; set; }
        public IList<Tickets> UnassignedTickets { get; set; }
        public IList<ApplicationUser> UsersNotInProject { get; set; }
    }
}