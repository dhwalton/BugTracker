﻿using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

public class ProjectsHelper
{

    private ApplicationDbContext db = new ApplicationDbContext();
    private UserManager<ApplicationUser> userManager;

    public ProjectsHelper()
    {
        userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
    }

    public bool IsUserOnProject(string userId, int projectId)
    {
        var project = db.Projects.Find(projectId);
        var flag = project.Users.Any(u => u.Id == userId);
        return flag;
    }

    public IQueryable<Projects> ProjectsAssigned(string userId)
    {
        var user = db.Users.Find(userId);
        var projects = db.Projects.Where(p => p.Users.Contains(user));
        return projects;
    }

    // generate a list of projects assigned to a user
    public IList<Projects> ProjectsAssignedToUser(string userName)
    {
        
        var user = userManager.FindByName(userName);
        var projects = db.Projects.Where(p => p.Users.Contains(user)).ToList();
        return projects;
    }

    // determines whether a user is in a specific project
    public bool IsUserInProject(string userId, int projectId)
    {
        var project = db.Projects.Find(projectId);
        var isAssigned = project.Users.Any(u => u.Id == userId);

        return isAssigned;
    }

    // get a list of all users assigned to a project
    public IList<ApplicationUser> UsersInProject(int? projectId)
    {
        if (projectId == null) return null;
        var resultList = new List<ApplicationUser>();
        resultList = db.Users.Where(p => p.Projects.Any(n => n.Id == projectId)).ToList();

        return resultList;
    }

    // get a list of all users assigned to a project
    public IEnumerable<ApplicationUser> UsersNotInProject(int? projectId)
    {
        if (projectId == null) return null;
        var resultList = new List<ApplicationUser>();
        resultList = db.Users.Where(p => !p.Projects.Any(n => n.Id == projectId)).ToList();
        

        return resultList;
    }

    // remove a user from a project 
    public bool RemoveUserFromProject(int projectId, string userId)
    {
        var project = db.Projects.Find(projectId);
        var user = db.Users.Find(userId);
        var result = project.Users.Remove(user);
        if (result)
        {
            //var tickets = project.Tickets.Where(t => t.AssignedUserId == userId);
            
            //// unassign a project's tickets from this user (could this be done with linq?)
            //foreach (var ticket in tickets)
            //{
            //    ticket.AssignedUserId = null;
            //}


            db.SaveChanges();
        }
        return result;
    }

    // add a user to a project
    public void AddUserToProject(int projectId, string userId)
    {
        var project = db.Projects.Find(projectId);
        var newUser = db.Users.Find(userId);
        project.Users.Add(newUser);
        db.SaveChanges();
    }

    // assign a project manager to a project
    public void AssignManagerToProject(int projectId, string userId)
    {
        db.Projects.Find(projectId).ManagerId = userId;
        db.SaveChanges();
    }

    public ApplicationUser GetManagerForProject(int projectId)
    {
        var project = db.Projects.Find(projectId);
        if (!String.IsNullOrWhiteSpace(project.ManagerId))
        {
            var user = db.Users.Find(project.ManagerId);
            return user;
        }
        else
        {
            return null;
        }
    }

    public string ManagerOfProject(int projectId)
    {
        var project = db.Projects.Find(projectId);
        return project.ManagerId;
    }
}