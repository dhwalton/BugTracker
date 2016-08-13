using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

public class UserRolesHelper
{
    private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
    private ApplicationDbContext db = new ApplicationDbContext();

    public bool IsUserInRole(string userId, string roleName)
    {
        return manager.IsInRole(userId, roleName);
    }

    public ICollection<string> ListUserRoles(string userId)
    {
        return manager.GetRoles(userId);
    }

    public bool AddUserToRole(string userId, string roleName)
    {
        var result = manager.AddToRole(userId, roleName);
        return result.Succeeded;
    }

    public bool AddUserToRoles(string userId, string[] roleNames)
    {
        var result = manager.AddToRoles(userId, roleNames);
        return result.Succeeded;
    }

    public bool RemoveUserFromRole(string userId, string roleName)
    {
        var result = manager.RemoveFromRole(userId, roleName);
        return result.Succeeded;
    }

    public ICollection<ApplicationUser> UsersInRole(string roleName)
    {
        var resultList = new List<ApplicationUser>();
        var List = manager.Users.ToList();
        foreach (var user in List)
        {
            if (IsUserInRole(user.Id, roleName))
                resultList.Add(user);
        }
        //var roleId = db.Roles.FirstOrDefault(r => r.Name == roleName).Id;
        //resultList = manager.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId)).ToList();
        return resultList;
    }

    public ICollection<ApplicationUser> UsersNotInRole(string roleName)
    {
        var resultList = new List<ApplicationUser>();
        var List = manager.Users.ToList();
        foreach (var user in List)
        {
            if (!IsUserInRole(user.Id, roleName))
                resultList.Add(user);
        }
        //var roleId = db.Roles.FirstOrDefault(r => r.Name == roleName).Id;
        //resultList = manager.Users.Where(u => u.Roles.Any(r => r.RoleId != roleId)).ToList();
        return resultList;
    }



        // Some methods to help with getting/setting user fields
        public string GetUserFirstName(string userId)
        {
            return manager.FindById(userId).FirstName;
        }

        public string GetUserLastName(string userId)
        {
            return manager.FindById(userId).LastName;
        }

        public string GetUserDisplayName(string userId)
        {
            return manager.FindById(userId).Displayname;   
        }

        public IdentityResult SetUserFirstName(string userId, string newFirstName)
        {
            var user = manager.FindById(userId);
            user.FirstName = newFirstName;
            return manager.Update(user);

           // db.Entry(user).State = EntityState.Modified;
           // return db.SaveChanges();
        }

        public IdentityResult SetUserLastName(string userId, string newLastName)
        {
            var user = manager.FindById(userId);
            user.LastName = newLastName;
            //db.Entry(user).State = EntityState.Modified;
            return manager.Update(user);
        }

        public IdentityResult SetUserDisplayName(string userId, string newDisplayName)
        {
            var user = manager.FindById(userId);
            user.Displayname = newDisplayName;
           // db.Entry(user).State = EntityState.Modified;
            //return db.SaveChanges();
            return manager.Update(user);
        }

        public IList<ApplicationUser> AllUsers()
        {
            return manager.Users.ToList();
        }

        public ApplicationUser GetUserById(string Id)
        {
            return manager.FindById(Id);
        }

        public ApplicationUser GetUserByName(string Name)
        {
            return manager.FindByName(Name);
        }

}
