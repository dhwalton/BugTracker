﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketComments
        public ActionResult Index()
        {
            var ticketComments = db.TicketComments.Include(t => t.Tickets).Include(t => t.User);
            return View(ticketComments.ToList());
        }

        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComments ticketComments = db.TicketComments.Find(id);
            if (ticketComments == null)
            {
                return HttpNotFound();
            }
            return View(ticketComments);
        }

        // GET: TicketComments/Create
        public ActionResult Create()
        {
            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Comment,Created,TicketsId,UserId")] TicketComments ticketComments)
        {
            if (ModelState.IsValid)
            {
                db.TicketComments.Add(ticketComments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "Title", ticketComments.TicketsId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComments.UserId);
            return View(ticketComments);
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult LeaveComment(string commentText, int ticketId)
        {
            var helper = new UserRolesHelper();
            var tHelper = new TicketsHelper();
            var userId = User.Identity.GetUserId();

            var user = db.Users.Find(User.Identity.GetUserId());
            var ticket = db.Tickets.Find(ticketId);

            // Check to see if this user meets the criteria to leave a comment
            if (!tHelper.CanEditTicket(user.Id,ticketId) && user.Id != ticket.OwnerUserId)
            {
                return RedirectToAction("Details", "Tickets", new { id = ticketId });
            }

            var comment = new TicketComments();
            comment.Comment = commentText;
            comment.User = user;
            comment.Tickets = ticket;
            comment.Created = DateTimeOffset.Now;
            db.TicketComments.Add(comment);
            db.SaveChanges();

            // notify project manager
            string message = "Comment has been added";
            if (userId != ticket.Project.ManagerId)
            {
                tHelper.CreateNotification(ticketId, ticket.Project.ManagerId, message);
            }

            // notify assigned user
            if (userId != ticket.AssignedUserId && ticket.AssignedUserId != ticket.Project.ManagerId)
            {
                tHelper.CreateNotification(ticketId, ticket.AssignedUserId, message);
            }

            // notify submitter
            if (userId != ticket.OwnerUserId)
            {
                tHelper.CreateNotification(ticketId, ticket.OwnerUserId, message);
            }


            if (User.IsInRole("Admin") || 
                ticket.AssignedUserId == user.Id || 
                (User.IsInRole("Project Manager") && ticket.Project.Users.Contains(user)))
            {
                return RedirectToAction("Edit", "Tickets", new { id = ticketId });
            }
            else
            {
                return RedirectToAction("Details", "Tickets", new { id = ticketId });
            }
            
        }


        // GET: TicketComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComments ticketComments = db.TicketComments.Find(id);
            if (ticketComments == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "Title", ticketComments.TicketsId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComments.UserId);
            return View(ticketComments);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Comment,Created,TicketsId,UserId")] TicketComments ticketComments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "Title", ticketComments.TicketsId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComments.UserId);
            return View(ticketComments);
        }

        // GET: TicketComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComments ticketComments = db.TicketComments.Find(id);
            if (ticketComments == null)
            {
                return HttpNotFound();
            }
            return View(ticketComments);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComments ticketComments = db.TicketComments.Find(id);
            db.TicketComments.Remove(ticketComments);
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
