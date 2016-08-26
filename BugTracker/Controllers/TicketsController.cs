using System;
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
using System.IO;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin, Project Manager, Developer, Submitter, Demo Admin, Demo Project Manager, Demo Developer, Demo Submitter")]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int? page, string searchStr)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.UserId = user.Id;

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

            if (user.isDemoUser())
            {
                tickets = tickets.Where(t => t.Project.DemoProject == true).OrderByDescending(t => t.TicketPriorityId);
            }

            return View(tickets.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult OldIndex(int? page, string orderby)
        {
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.User = db.Users.Find(User.Identity.GetUserId());

            // all tickets with relevant fields from related tables
            var tickets = db.Tickets.Include(t => t.AssignedUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType);

            // sort the ticket list
            //tickets = SortTicketsBy(tickets, orderby);

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
            //tickets = SortTicketsBy(tickets, orderby);

            return View(tickets.ToList());
        }

        [HttpGet]
        [Authorize (Roles="Admin, Project Manager, Developer, Demo Admin, Demo Developer, Demo Project Manager")]
        public ActionResult ProjectTickets()
        {
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.User = db.Users.Find(User.Identity.GetUserId());

            var userId = User.Identity.GetUserId();

            // all tickets that are in the user's projects
            var tickets = db.Tickets
                .Where(t => t.Project.Users.Any(u => u.Id == userId))
                .Include(t => t.AssignedUser)
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
        [Authorize(Roles = "Admin, Project Manager, Developer, Demo Admin, Demo Developer, Demo Project Manager")]
        public ActionResult AssignedTickets(string orderby)
        {
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.User = db.Users.Find(User.Identity.GetUserId());

            // instantiate an ApplicationUser from this user's Id
            //var user = db.Users.Find(User.Identity.GetUserId());
            var userId = User.Identity.GetUserId();

            //all tickets with relevant fields from related tables
            var tickets = db.Tickets
                .Where(t => t.AssignedUser.Id == userId)
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
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.User = user;

            // start with a list of all tickets
            var tickets = db.Tickets
                .Include(t => t.AssignedUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .OrderByDescending(t => t.TicketPriorityId);

            // demo users will see different tickets than regular users
            // admins can see every ticket
            if (!User.IsInRole("Admin")) {
                if (user.isDemoUser())
                {
                    // demo users can only see demo tickets
                    tickets = tickets.Where(t => t.Project.DemoProject == true).OrderByDescending(t => t.TicketPriorityId);
                }
                else
                {
                    // "real" non-admin users can only see non-demo tickets
                    tickets = tickets.Where(t => t.Project.DemoProject == false).OrderByDescending(t => t.TicketPriorityId);
                }
            }

            return View(tickets.ToList());
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
            helper.CreateNotification(ticketId, userId, "You have been assigned to this ticket");
            return RedirectToAction("Edit", new { id = ticketId });
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            Tickets tickets = db.Tickets.Find(id);

            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.CanEditTicket(id ?? 1) || user.OwnsTicket(id ?? 1))
            {
                ViewBag.CommentAttachmentRights = true;
            }
            else
            {
                ViewBag.CommentAttachmentRights = false;
            }

            // keep demo users out of "real" tickets and vice versa
            if ((tickets.Project.DemoProject && !user.isDemoUser())
                || tickets.Project.DemoProject && !user.isDemoUser())
            {
                return RedirectToAction("Index");
            }

            if (tickets == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Index");
            }
            return View(tickets);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Admin, Developer, Project Manager, Submitter, Demo Admin, Demo Developer, Demo Project Manager, Demo Submitter")]
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
            var user = db.Users.Find(userId);
            var ticket = db.Tickets.Find(id);
            var canEditTicket = tHelper.CanEditTicket(userId, id ?? 1);

            // keep demo users out of "real" tickets and vice versa
            if ((ticket.Project.DemoProject && !user.isDemoUser())
                || !ticket.Project.DemoProject && user.isDemoUser())
            {
                return RedirectToAction("Index");
            }


            if (!canEditTicket)
            {
                return RedirectToAction("Details", new { id = id });
                //return RedirectToAction("Index");
            }

            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }

            if (canEditTicket || tHelper.UserOwnsTicket(id ?? 1, userId))
            {
                ViewBag.CommentAttachmentRights = true;
            }
            else
            {
                ViewBag.CommentAttachmentRights = false;
            }

            if (canEditTicket && !User.IsInRole("Admin") && !User.IsInRole("Project Manager") && !User.IsInRole("Demo Admin"))
            {
                ViewBag.FullEditPermission = false;
            }
            else
            {
                ViewBag.FullEditPermission = true;
            }

            // clear any notifications that the user might have for this ticket
            tHelper.clearNotifications(id ?? 1, userId);


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
                var id = tickets.Id;
                var userId = User.Identity.GetUserId();
                var helper = new TicketsHelper();
                
                // No tracking method keeps the system from throwing a primary key conflict exception - also necessitates using Where() instead of Find()
                var oldTicket = db.Tickets.AsNoTracking().Where(t => t.Id == tickets.Id).First();

                //log any changes to the ticket
                if (oldTicket.Title != tickets.Title) helper.LogTicketActivity(tickets.Id, "Title", oldTicket.Title, tickets.Title);
                if (oldTicket.Description != tickets.Description) helper.LogTicketActivity(tickets.Id, "Description", oldTicket.Description, tickets.Description);
                if (oldTicket.ProjectId != tickets.ProjectId) helper.LogTicketActivity(tickets.Id, "Project", oldTicket.Project.Name, tickets.Project.Name);
                if (oldTicket.TicketPriorityId != tickets.TicketPriorityId) helper.LogTicketActivity(tickets.Id, "TicketPriorityId", oldTicket.TicketPriorityId.ToString(), tickets.TicketPriorityId.ToString());
                if (oldTicket.TicketTypeId != tickets.TicketTypeId) helper.LogTicketActivity(tickets.Id, "TicketTypeId", oldTicket.TicketTypeId.ToString(), tickets.TicketTypeId.ToString());
                if (oldTicket.TicketStatusId != tickets.TicketStatusId) helper.LogTicketActivity(tickets.Id, "TicketStatusId", oldTicket.TicketStatusId.ToString(), tickets.TicketStatusId.ToString());
                
                tickets.Updated = DateTimeOffset.Now;

                db.Entry(tickets).State = EntityState.Modified;
                db.SaveChanges();


                // notify user(s) that the ticket has been updated
                // notify project manager
                var managerId = db.Projects.Find(tickets.ProjectId).ManagerId;
                string message = "Ticket has been modified";
                if (userId != managerId)
                {
                    helper.CreateNotification(tickets.Id, managerId, message);
                }

                // notify assigned user
                if (userId != tickets.AssignedUserId && tickets.AssignedUserId != managerId)
                {
                    helper.CreateNotification(tickets.Id, tickets.AssignedUserId, message);
                }

                // notify submitter
                //if (userId != tickets.OwnerUserId)
                //{
                //    helper.CreateNotification(tickets.Id, tickets.OwnerUserId, message);
                //}



                return RedirectToAction("Edit", new { id = tickets.Id });
            }

            

            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", tickets.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", tickets.OwnerUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Developer, Project Manager, Submitter, Demo Admin, Demo Submitter, Demo Project Manager, Demo Developer")]
        public ActionResult UploadTicketAttachment(int ticketId, string description, HttpPostedFileBase attachment)
        {
            var helper = new TicketsHelper();
            var ticket = db.Tickets.Find(ticketId);
            var userId = User.Identity.GetUserId();

            // make sure this user actually meets the criteria to upload
            if (helper.CanEditTicket(userId, ticketId) || userId == ticket.OwnerUserId)
            {
                // where the magic happens
                if (attachment != null && attachment.ContentLength > 0)
                {
                    //check the file name to make sure its an image
                    var ext = Path.GetExtension(attachment.FileName).ToLower();
                    if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp" && ext != ".txt")
                        ModelState.AddModelError("file", "Invalid Format.");

                    if (attachment != null)
                    { 
                        //relative server path
                        var filePath = "/Uploads/";
                        // path on physical drive on server
                        var absPath = Server.MapPath("~" + filePath);
                        //save image
                        attachment.SaveAs(Path.Combine(absPath, attachment.FileName));
                        // update ticket
                        var ticketAttachment = new TicketAttachments();

                        // if this is an image, set the IsImage flag to true
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp")
                        {
                            ticketAttachment.IsImage = true;
                        }
                        else
                        {
                            ticketAttachment.IsImage = false;
                        }
                        
                        ticketAttachment.Created = DateTimeOffset.Now;
                        ticketAttachment.FilePath = filePath;
                        ticketAttachment.FileUrl = Path.Combine(filePath, attachment.FileName);
                        ticketAttachment.Description = description;
                        ticketAttachment.UserId = userId;

                        ticket.TicketAttachments.Add(ticketAttachment);

                        helper.LogTicketActivity(ticketId, "Attachment Added", "", ticketAttachment.FileUrl);


                        // notify project manager
                        string message = "Attachment has been added";
                        if (userId != ticket.Project.ManagerId)
                        {
                            helper.CreateNotification(ticketId, ticket.Project.ManagerId, message);
                        }

                        // notify assigned user
                        if (userId != ticket.AssignedUserId && ticket.AssignedUserId != ticket.Project.ManagerId)
                        {
                            helper.CreateNotification(ticketId, ticket.AssignedUserId, message);
                        }


                        db.Entry(ticket).State = EntityState.Modified;
                        db.SaveChanges(); 
                    }
                }
                
            }

            // return to the appropriate view based on user's role and ticket permissions
            if (helper.CanEditTicket(userId, ticketId)) 
            {
                return RedirectToAction("Edit", new { id = ticketId });
            }
            else
            {
                return RedirectToAction("Details", new { id = ticketId });
            }
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize (Roles = "Admin")]
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
