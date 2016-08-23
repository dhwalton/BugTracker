using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketHistories
    { 
        public string ToMessage(bool old)
        {
            var h = new TicketsHelper();
            return h.HistoryToString(Id, old);
        }
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public bool Changed { get; set; }
        public DateTimeOffset ChangedDate { get; set; }
        
        public virtual Tickets Ticket { get; set; } 
        public virtual ApplicationUser User { get; set; }
    }
}