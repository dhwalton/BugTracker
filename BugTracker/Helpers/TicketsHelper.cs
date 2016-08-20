using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

public class TicketsHelper
{
    
    private ApplicationDbContext db = new ApplicationDbContext();
    //private UserManager<ApplicationUser> userManager;

    public TicketsHelper()
    {
       // userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
    }

    public IQueryable<Tickets> UserTickets(string userId)
    {
        var user = db.Users.Find(userId);
        var tickets = db.Tickets.Where(t => t.AssignedUser == user);
        return tickets;
    }

    public bool CanEditTicket(string userId, int ticketId)
    {
        var ticket = db.Tickets.Find(ticketId);
        var user = db.Users.Find(userId);

        // admins can edit all tickets
        if (user.inRole("Admin")) return true;

        // managers of a ticket's project can edit
        if (userId == ticket.Project.ManagerId) return true;

        // any user that is assigned this ticket can edit
        if (userId == ticket.AssignedUserId) return true;

        // nobody else can edit!
        return false;
    }

    // generate a list of tickets assigned to a user
    public IList<Tickets>TicketsAssignedToUser(string userId)
    {
        var user = db.Users.Find(userId);
        var tickets = db.Tickets.Where(t => t.AssignedUser == user).ToList();
        return tickets;
    }

    // generate a list of tickets assigned to a user in a specific project
    public IList<Tickets>TicketsAssignedToUserInProject(string userId, int projectId)
    {
        var user = db.Users.Find(userId);
        var project = db.Projects.Find(projectId);
        var tickets = project.Tickets.Where(t => t.AssignedUser == user).ToList();
        return tickets;
    }

    // generate a list of tickets NOT assigned to a user in a specific project
    public IList<Tickets> TicketsNotAssignedToUserInProject(string userId, int projectId)
    {
        var user = db.Users.Find(userId);
        var project = db.Projects.Find(projectId);
        var tickets = project.Tickets.Where(t => t.AssignedUser != user).ToList();
        return tickets;
    }

    // generate a list of unassigned tickets for a project
    public IList<Tickets> UnassignedTicketsInProject(int projectId)
    {
        var project = db.Projects.Find(projectId);
        var tickets = project.Tickets.Where(t => t.AssignedUser == null).ToList();
        return tickets;
    }

    // assign a user to a ticket
    public void AddUserToTicket(int ticketId, string userId)
    {
        var user = db.Users.Find(userId);
        var ticket = db.Tickets.Find(ticketId);
        ticket.AssignedUser = user;
        ticket.Updated = DateTimeOffset.Now;
        db.SaveChanges();
    }

    public void RemoveUserFromTicket(int ticketId)
    {
        var ticket = db.Tickets.Find(ticketId);
        ticket.AssignedUserId = null;
        ticket.Updated = DateTimeOffset.Now;
        db.SaveChanges();
    }

    // is a specific user assigned to a specific ticket?
    public bool UserIsAssignedTicket(int ticketId, string userId)
    {
        var ticket = db.Tickets.Find(ticketId);
        if (ticket.AssignedUserId == userId)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // does a specific user own a specific ticket?
    public bool UserOwnsTicket(int ticketId, string userId)
    {
        var ticket = db.Tickets.Find(ticketId);
        if (ticket.OwnerUserId == userId)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}