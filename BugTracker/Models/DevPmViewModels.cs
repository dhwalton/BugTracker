using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ProjectDetailViewModel 
    {
        public Projects Project { get; set; }
        public IList<Tickets> AssignedTickets { get; set; }
        public IList<Tickets> UnassignedTickets { get; set; }
    }
}