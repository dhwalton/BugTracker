namespace BugTracker.Migrations
{
    using Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));

            var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            if (!context.Users.Any(u => u.Email == "donny77walton@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "donny77walton@gmail.com",
                    Email = "donny77walton@gmail.com",
                    FirstName = "Donny",
                    LastName = "Walton",
                    Displayname = "Donny Walton"
                }, "MaxGunch03!");
            }


            if (!context.Users.Any(u => u.Email == "logan@jontz.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "logan@jontz.com",
                    Email = "logan@jontz.com",
                    FirstName = "Logan",
                    LastName = "Walton",
                    Displayname = "Logan Walton"
                }, "Testing123!");
            }

            if (!context.Users.Any(u => u.Email == "henry@jontz.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "henry@jontz.com",
                    Email = "henry@jontz.com",
                    FirstName = "Henry",
                    LastName = "Walton",
                    Displayname = "Henry Walton"
                }, "Testing123!");
            }

            if (!context.Users.Any(u => u.Email == "maggie@jontz.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "maggie@jontz.com",
                    Email = "maggie@jontz.com",
                    FirstName = "Maggie",
                    LastName = "Walton",
                    Displayname = "Maggie Walton"
                }, "Testing123!");
            }


            var userId = userManager.FindByEmail("donny77walton@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            var lUserId = userManager.FindByEmail("logan@jontz.com").Id;
            userManager.AddToRole(lUserId, "Project Manager");

            var hUserId = userManager.FindByEmail("henry@jontz.com").Id;
            userManager.AddToRole(hUserId, "Developer");

            var mUserId = userManager.FindByEmail("maggie@jontz.com").Id;
            userManager.AddToRole(mUserId, "Developer");

            // Seed some ticket priorities
            //if (!context.TicketPriorities.Any(t => t.Name == "Default"))
            //{
            //    var ticketPriorityDefault = new TicketPriorities();
            //    ticketPriorityDefault.Name = "Default";
            //    context.TicketPriorities.Add(ticketPriorityDefault);
            //}

            if (!context.TicketPriorities.Any(t => t.Name == "Default"))
            {
                var ticketPriorityDefault = new TicketPriorities();
                ticketPriorityDefault.Name = "Default";
                context.TicketPriorities.Add(ticketPriorityDefault);
            }


            if (!context.TicketPriorities.Any(t => t.Name == "Low"))
            {
                var ticketPriorityLow = new TicketPriorities();
                ticketPriorityLow.Name = "Low";
                context.TicketPriorities.Add(ticketPriorityLow);
            }

            if (!context.TicketPriorities.Any(t => t.Name == "Medium"))
            {
                var ticketPriorityMedium = new TicketPriorities();
                ticketPriorityMedium.Name = "Medium";
                context.TicketPriorities.Add(ticketPriorityMedium);
            }

            if (!context.TicketPriorities.Any(t => t.Name == "High"))
            {
                var ticketPriorityHigh = new TicketPriorities();
                ticketPriorityHigh.Name = "High";
                context.TicketPriorities.Add(ticketPriorityHigh);
            }

            if (!context.TicketPriorities.Any(t => t.Name == "Urgent"))
            {
                var ticketPriorityUrgent = new TicketPriorities();
                ticketPriorityUrgent.Name = "Urgent";
                context.TicketPriorities.Add(ticketPriorityUrgent);
            }

            // seed ticket statuses
            if (!context.TicketStatuses.Any(t => t.Name == "Open"))
            {
                var ticketStatusOpen = new TicketStatuses();
                ticketStatusOpen.Name = "Open";
                context.TicketStatuses.Add(ticketStatusOpen);
            }

            if (!context.TicketStatuses.Any(t => t.Name == "Closed"))
            {
                var ticketStatusClosed = new TicketStatuses();
                ticketStatusClosed.Name = "Closed";
                context.TicketStatuses.Add(ticketStatusClosed);
            }

            // seed ticket types
            if (!context.TicketTypes.Any(t => t.Name == "Enhancement"))
            {
                var ticketTypeDefault = new TicketTypes();
                ticketTypeDefault.Name = "Enhancement";
                context.TicketTypes.Add(ticketTypeDefault);
            }

            if (!context.TicketTypes.Any(t => t.Name == "Software Bug"))
            {
                var ticketTypeBug = new TicketTypes();
                ticketTypeBug.Name = "Software Bug";
                context.TicketTypes.Add(ticketTypeBug);
            }
            
            //if (!context.TicketTypes.Any(t => t.Name == "Software Bug"))
            //{
            //    var ticketTypeBug = new TicketTypes();
            //    ticketTypeBug.Name = "Software Bug";
            //    context.TicketTypes.Add(ticketTypeDefault);
            //}
        }
    }
}
