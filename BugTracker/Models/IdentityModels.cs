using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Projects = new HashSet<Projects>();
            //Tickets = new HashSet<Tickets>();
            Comments = new HashSet<TicketComments>();
            Attachments = new HashSet<TicketAttachments>();
            Histories = new HashSet<TicketHistories>();
            Notifications = new HashSet<TicketNotifications>();

            // attachments, comments, histories, notifications, projects
        }

        // is user a PM?
        public bool IsPM()
        {
            var h = new UserRolesHelper();
            return h.IsUserInRole(Id, "Project Manager");
        }

        // is user a Developer?
        public bool IsDev()
        {
            var h = new UserRolesHelper();
            return h.IsUserInRole(Id, "Developer");
        }

        // is user in a specified role?
        public bool inRole(string roleName)
        {
            var h = new UserRolesHelper();
            return h.IsUserInRole(Id, roleName);
        }

        // is this user a manager of a specified project?
        public bool ManagerOfProject(int projectId)
        {
            var h = new ProjectsHelper();
            if (h.ManagerOfProject(projectId) == Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // is this user part of a specified project?
        public bool OnProject(int projectId)
        {
            var h = new ProjectsHelper();
            return h.IsUserInProject(Id, projectId);
        }

        // Does this user own a specified ticket?
        public bool OwnsTicket(int ticketId)
        {
            var h = new TicketsHelper();
            return h.UserOwnsTicket(ticketId, Id);
        }

        // Is this user assigned to a specified ticket?
        public bool AssignedTicket(int ticketId)
        {
            var h = new TicketsHelper();
            return h.UserIsAssignedTicket(ticketId, Id);
        }

        public bool CanEditTicket(int ticketId)
        {
            var h = new TicketsHelper();
            return h.CanEditTicket(Id, ticketId);
        }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Display Name")]
        public string Displayname { get; set; }

        //public virtual ICollection<Tickets> Tickets { get; set; }
        public virtual ICollection<TicketComments> Comments { get; set; }
        public virtual ICollection<Projects> Projects { get; set; }
        public virtual ICollection<TicketAttachments> Attachments { get; set; }
        public virtual ICollection<TicketHistories> Histories { get; set; }
        public virtual ICollection<TicketNotifications> Notifications { get; set; }

        public virtual TicketPriorities TicketPriority { get; set; }
        public virtual TicketStatuses TicketStatus { get; set; }
        public virtual TicketTypes TicketType { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<TicketComments> TicketComments { get; set; }
        public DbSet<TicketHistories> TicketHistories { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<TicketAttachments> TicketAttachments { get; set; }
        public DbSet<TicketNotifications> TicketNotifications { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketPriorities> TicketPriorities { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketStatuses> TicketStatuses { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketTypes> TicketTypes { get; set; }
    }
}