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
using PagedList;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // sorts tickets by column - might possibly add ascending/descending functionality later
        private IQueryable<Tickets> SortTicketsBy(IQueryable<Tickets> tickets, string orderby)
        {
            // I'm sure there's a better way to do this...
            switch (orderby)
            {
                case "priority":
                    // higher priority tickets are higher on the list
                    tickets = tickets.OrderByDescending(t => t.TicketPriorityId);
                    break;
                case "updated":
                    tickets = tickets.OrderBy(t => t.Updated);
                    break;
                case "status":
                    tickets = tickets.OrderBy(t => t.TicketStatusId);
                    break;
                case "project":
                    tickets = tickets.OrderBy(t => t.Project.Name);
                    break;
                case "title":
                    tickets = tickets.OrderBy(t => t.Title);
                    break;
                case "type":
                    tickets = tickets.OrderBy(t => t.TicketTypeId);
                    break;
                case "assigned":
                    tickets = tickets.OrderBy(t => t.AssignedUser.Displayname);
                    break;
                case "submitted":
                    tickets = tickets.OrderBy(t => t.OwnerUser.Displayname);
                    break;
                case "created":
                    tickets = tickets.OrderBy(t => t.Created);
                    break;
                default:
                    tickets = tickets.OrderByDescending(t => t.TicketPriorityId);
                    break;
            }
            return tickets;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.UserId = User.Identity.GetUserId();

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            // all tickets with relevant fields from related tables
            var tickets = db.Tickets.Where(t => t.Description.Contains(searchStr))
                .Union(db.Tickets.Where(t => t.Title.Contains(searchStr)))
                .Union(db.Tickets.Where(t => t.Title.Contains(searchStr)))
                .Union(db.Tickets.Where(t => t.Project.Name.Contains(searchStr)))
                .Union(db.Tickets.Where(t => t.TicketStatus.Name.Contains(searchStr)))
                .Union(db.Tickets.Where(t => t.TicketPriority.Name.Contains(searchStr)))
                .Union(db.Tickets.Where(t => t.TicketType.Name.Contains(searchStr)))
                .Union(db.Tickets.Where(t => t.AssignedUser.Displayname.Contains(searchStr)))
                .Union(db.Tickets.Where(t => t.OwnerUser.Displayname.Contains(searchStr)))
                .Include(t => t.AssignedUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .OrderByDescending(t => t.TicketPriorityId);

            return View(tickets.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult OldIndex(int? page, string orderby)
        {
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.User = db.Users.Find(User.Identity.GetUserId());

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            

            // all tickets with relevant fields from related tables
            var tickets = db.Tickets.Include(t => t.AssignedUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType);

            // sort the ticket list
            tickets = SortTicketsBy(tickets, orderby);

            return View(tickets);
        }

        [HttpGet]
        public ActionResult SubmittedTickets(string orderby)
        {
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.User = db.Users.Find(User.Identity.GetUserId());

            // instantiate an ApplicationUser from this user's Id
            var user = db.Users.Find(User.Identity.GetUserId());

            // all tickets submitted by this user with relevant fields from related tables 
            var tickets = db.Tickets.Include(t => t.AssignedUser)
                .Where(t => t.OwnerUser.Id == user.Id)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType);

            // sort the ticket list
            tickets = SortTicketsBy(tickets, orderby);

            return View(tickets.ToList());
        }

        [HttpGet]
        public ActionResult ProjectTickets()
        {
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.User = db.Users.Find(User.Identity.GetUserId());

            // instantiate an ApplicationUser from this user's Id
            var user = db.Users.Find(User.Identity.GetUserId());

            // all tickets submitted by this user with relevant fields from related tables 
            var tickets = db.Tickets.Include(t => t.AssignedUser)
                .Where(t => t.Project.Users.Any(u => u.Id == user.Id))
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .OrderByDescending(t => t.TicketPriorityId);

            // sort the ticket list
            //tickets = SortTicketsBy(tickets, orderby);

            return View(tickets.ToList());
        }

        [HttpGet]
        public ActionResult AssignedTickets(string orderby)
        {
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.User = db.Users.Find(User.Identity.GetUserId());

            // instantiate an ApplicationUser from this user's Id
            var user = db.Users.Find(User.Identity.GetUserId());

            //all tickets with relevant fields from related tables
            var tickets = db.Tickets
                .Where(t => t.AssignedUser.Id == user.Id)
                .Include(t => t.AssignedUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType);

            return View(tickets.ToList());
        }

        // GET: Tickets
        public ActionResult Index(int? page)
        {
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.User = db.Users.Find(User.Identity.GetUserId());

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            // start with a list of all tickets
            var tickets = db.Tickets
                .Include(t => t.AssignedUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .OrderByDescending(t => t.TicketPriorityId);

            return View(tickets.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult RemoveAssignedUserFromTicket(int ticketId)
        {
            var helper = new TicketsHelper();
            helper.RemoveUserFromTicket(ticketId);
            return RedirectToAction("Edit", new { id = ticketId });
        }

        public ActionResult AssignUserToTicket(int ticketId, string userId)
        {
            var helper = new TicketsHelper();
            helper.AddUserToTicket(ticketId, userId);
            return RedirectToAction("Edit", new { id = ticketId });
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Admin, Developer, Project Manager, Submitter")]
        public ActionResult Create()
        {
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedUserId")] Tickets tickets)
        {
            var helper = new UserRolesHelper();
            tickets.OwnerUserId = helper.GetUserByName(User.Identity.Name).Id;
            tickets.Created = DateTimeOffset.Now;
            tickets.AssignedUserId = null;
            tickets.TicketStatusId = 1;
            tickets.Updated = null;
            if (ModelState.IsValid)
            {
                db.Tickets.Add(tickets);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", tickets.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", tickets.OwnerUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var uHelper = new UserRolesHelper();
            var tHelper = new TicketsHelper();
            var pHelper = new ProjectsHelper();

            var userId = User.Identity.GetUserId();
            var ticket = db.Tickets.Find(id);
          

            // kick out devs and PMs who don't meet the criteria to edit this ticket
            if (!uHelper.IsUserInRole(userId,"Admin") && 
                ((!uHelper.IsUserInRole(userId, "Project Manager") && !tHelper.UserIsAssignedTicket(id ?? 1, userId))
                || uHelper.IsUserInRole(userId, "Project Manager") && !pHelper.IsUserOnProject(userId, ticket.Project.Id)))
            {
                return RedirectToAction("Index");
            }

            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }

            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", tickets.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", tickets.OwnerUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedUserId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {
                tickets.Updated = DateTimeOffset.Now;
                db.Entry(tickets).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", tickets.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", tickets.OwnerUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tickets tickets = db.Tickets.Find(id);
            db.Tickets.Remove(tickets);
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
