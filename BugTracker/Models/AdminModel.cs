﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class UsersAndRolesModel
    {
        public UsersAndRolesModel()
        {
            var db = new ApplicationDbContext();
            var urHelper = new UserRolesHelper();
            Roles = urHelper.ListAllRoles();
            Users = db.Users.ToList();
        }

        public IList<ApplicationUser> Users { get; set; }
        public IList<IdentityRole> Roles { get; set; }
    }

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
        public ListUsersRolesModel UserList { get; set; }
    }

    public class RemoveUserFromProjectModel
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; }
    }

    public class ListUsersRolesModel
    {
        public int ProjectId { get; set; }
        public IOrderedEnumerable<ApplicationUser> Users { get; set; }
        public ICollection<IdentityUserRole> Roles { get; set; }

        public virtual Projects Project { get; set; }
    }
}