using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();

        [Authorize]
        public ActionResult Dashboard()
        {
            var helper = new TicketsHelper();
            var user = db.Users.Find(User.Identity.GetUserId());
            user.Notifications = user.Notifications.OrderBy(n => n.IsRead).ToList();
            ViewBag.UserTickets = helper.TicketsAssignedToUser(user.Id).OrderByDescending(t => t.Updated).ToList();
            ViewBag.TicketComments = helper.AssignedTicketComments(user.Id).OrderByDescending(c => c.Created).ToList();
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