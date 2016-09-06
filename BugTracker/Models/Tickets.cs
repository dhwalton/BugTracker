using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        [Display(Name = "Ticket Title")]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Created On")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy, h:mm tt}")]
        public DateTimeOffset Created { get; set; }

        [Display(Name = "Updated On")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy, h:mm tt}")]
        public DateTimeOffset? Updated { get; set; }

        public int ProjectId { get; set; }

        [Display(Name = "Type")]
        public int TicketTypeId { get; set; }
        [Display(Name = "Priority")]
        public int TicketPriorityId { get; set; }
        [Display(Name = "Status")]
        public int TicketStatusId { get; set; }
        [Display(Name = "Submitted by")]
        public string OwnerUserId { get; set; } 
        [Display(Name = "Assigned to")]
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