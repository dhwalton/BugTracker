using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class AdminUserViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }
    }

    public class AdminProjectEditModel
    {
        public Projects Project { get; set; }
        public IList<ApplicationUser> Users { get; set; }
    }

    public class RemoveUserFromProjectModel
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; }
    }
}