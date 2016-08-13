using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        db.SaveChanges();
    }

    public void RemoveUserFromTicket(int ticketId)
    {
        var ticket = db.Tickets.Find(ticketId);
        ticket.AssignedUserId = null;
        db.SaveChanges();
    }

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
}