using System;
using System.Collections.Generic;
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
        public string Name { get; set; }
        public virtual ICollection<Tickets> Tickets { get; set; }
    }
}