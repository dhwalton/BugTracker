using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
// TEST!
namespace BugTracker.Models
{
    public class Tickets
    {
        public Tickets()
        {
            // hashset holds all the comments for a particular Ticket
            TicketComments = new HashSet<TicketComments>();
            TicketNotifications = new HashSet<TicketNotifications>();
            TicketHistories = new HashSet<TicketHistories>();
            TicketAttachments = new HashSet<TicketAttachments>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        public string Description { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public string OwnerUserId { get; set; } 
        public string AssignedUserId { get; set; }

        public virtual ICollection<TicketNotifications> TicketNotifications { get; set; }
        public virtual ICollection<TicketAttachments> TicketAttachments { get; set; }
        public virtual ICollection<TicketHistories> TicketHistories { get; set; }
        public virtual ICollection<TicketComments> TicketComments { get; set; }

        public virtual Projects Project { get; set; }
        public virtual TicketTypes TicketType { get; set; }
        public virtual TicketPriorities TicketPriority { get; set; }
        public virtual TicketStatuses TicketStatus { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
    }
}