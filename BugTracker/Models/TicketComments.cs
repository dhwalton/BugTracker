using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class TicketComments
    {
        public int Id { get; set; }

        [AllowHtml]
        [Required]
        public string Comment { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTimeOffset Created { get; set; }
        public int TicketsId { get; set; }
        public string UserId { get; set; }
        public virtual Tickets Tickets { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}