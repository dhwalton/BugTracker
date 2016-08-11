using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketPriorities
    {
        public TicketPriorities()
        {
            Tickets = new HashSet<Tickets>();
        }
        public int Id { get; set; }
        [Display (Name="Priority")]
        public string Name { get; set; }
        public virtual ICollection<Tickets> Tickets { get; set; }
    }
}