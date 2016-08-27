using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();

        [Authorize]
        public ActionResult Dashboard()
        {
            var helper = new TicketsHelper();
            var user = db.Users.Find(User.Identity.GetUserId());

            var userTickets = helper.TicketsAssignedToUser(user.Id)
                               .Union(db.Tickets.Where(t => t.Project.ManagerId == user.Id))
                               .Union(db.Tickets.Where(t => t.OwnerUserId == user.Id))
                               .OrderByDescending(t => t.Updated);
            var userTicketComments = db.TicketComments
                                       .Where(c => c.Tickets.AssignedUserId == user.Id)
                                       .Union(db.TicketComments.Where(c => c.Tickets.Project.ManagerId == user.Id))
                                       .Union(db.TicketComments.Where(c => c.Tickets.OwnerUserId == user.Id))
                                       .OrderByDescending(c => c.Created);

            user.Notifications = user.Notifications.OrderBy(n => n.IsRead).ToList();
            ViewBag.UserTickets = userTickets.ToList();
            ViewBag.TicketComments = helper.AssignedTicketComments(user.Id).OrderByDescending(c => c.Created).ToList();
            //ViewBag.TicketComments = userTicketComments;
            return View(user);
        }

        public ActionResult Index()
        {
            // if the user is logged in, redirect to the dashboard
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        public ActionResult _NotificationsMenuItem()
        {
            var userId = User.Identity.GetUserId();
            var notifications = db.TicketNotifications.Where(n => n.UserId == userId).Where(n => n.IsRead == false);
            return PartialView(notifications.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}