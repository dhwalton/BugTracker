using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BugTracker.Models;

namespace BugTracker.Helpers
{
    public class UserRolesHelper
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        
        public UserRolesHelper(ApplicationDbContext context)
        {
            this.userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            this.roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            this.db = context;
        } 

        public bool IsUserInRole(string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }

        public IList<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var userIDs = roleManager.FindByName(roleName).Users.Select(r => r.UserId);
            return userManager.Users.Where(u => userIDs.Contains(u.Id)).ToList();
        }

        public IList<ApplicationUser> UsersNotInRole(string roleName)
        {
            var userIDs = System.Web.Security.Roles.GetUsersInRole(roleName);
            return userManager.Users.Where(u => !userIDs.Contains(u.Id)).ToList();
        }

        // Some methods to help with getting/setting user fields
        public string GetUserFirstName(string userId)
        {
            return userManager.FindById(userId).FirstName;
        }

        public string GetUserLastName(string userId)
        {
            return userManager.FindById(userId).LastName;
        }

        public string GetUserDisplayName(string userId)
        {
            return userManager.FindById(userId).Displayname;   
        }

        public IdentityResult SetUserFirstName(string userId, string newFirstName)
        {
            var user = userManager.FindById(userId);
            user.FirstName = newFirstName;
            return userManager.Update(user);

           // db.Entry(user).State = EntityState.Modified;
           // return db.SaveChanges();
        }

        public int SetUserLastName(string userId, string newLastName)
        {
            var user = userManager.FindById(userId);
            user.LastName = newLastName;
            db.Entry(user).State = EntityState.Modified;
            return db.SaveChanges();
        }

        public int SetUserDisplayName(string userId, string newDisplayName)
        {
            var user = userManager.FindById(userId);
            user.Displayname = newDisplayName;
            db.Entry(user).State = EntityState.Modified;
            return db.SaveChanges();
        }

    }
}