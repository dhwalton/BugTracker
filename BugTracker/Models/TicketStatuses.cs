using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketStatuses
    {
        public TicketStatuses()
        {
            Tickets = new HashSet<Tickets>();
        }
        public int Id { get; set; }
        [Display (Name="Status")]
        public string Name { get; set; }
        public virtual ICollection<Tickets> Tickets { get; set; }
    }
}