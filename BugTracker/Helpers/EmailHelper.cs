using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

public class EmailHelper
{
    //private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

    public static async Task SendMessage(TicketNotifications n)
    {
        UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        await manager.SendEmailAsync(n.UserId, "New Activity on Ticket '" + n.Ticket.Title + "'",
                "Ticket '" + n.Ticket.Title + "' has new activity: " + n.Message + "<p><a href='https://dhwalton-bugtracker.azurewebsites.net'>Click Here to Login.</a>");
        return;

        //using (var client = new SmtpClient("127.0.0.1", 25))
        //{
        //    await client.SendMailAsync(message, message);
        //}
    }
}
