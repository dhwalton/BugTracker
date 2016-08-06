namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Projects",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    StartDate = c.DateTimeOffset(nullable: false, precision: 7),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Tickets",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(nullable: false),
                    Description = c.String(nullable: false),
                    Created = c.DateTimeOffset(nullable: false, precision: 7),
                    Updated = c.DateTimeOffset(nullable: false, precision: 7),
                    ProjectId = c.Int(nullable: false),
                    TicketTypeId = c.Int(nullable: false),
                    TicketPriorityId = c.Int(nullable: false),
                    TicketStatusId = c.Int(nullable: false),
                    OwnerUserId = c.String(maxLength: 128),
                    AssignedUserId = c.String(maxLength: 128),
                    //ApplicationUser_Id = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                //.ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AssignedUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerUserId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.TicketPriorities", t => t.TicketPriorityId, cascadeDelete: true)
                .ForeignKey("dbo.TicketStatuses", t => t.TicketStatusId, cascadeDelete: true)
                .ForeignKey("dbo.TicketTypes", t => t.TicketTypeId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.TicketTypeId)
                .Index(t => t.TicketPriorityId)
                .Index(t => t.TicketStatusId)
                .Index(t => t.OwnerUserId)
                .Index(t => t.AssignedUserId);
                //.Index(t => t.ApplicationUser_Id);

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    FirstName = c.String(),
                    LastName = c.String(),
                    Displayname = c.String(),
                    Email = c.String(maxLength: 256),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "dbo.TicketAttachments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    TicketId = c.Int(nullable: false),
                    FilePath = c.String(),
                    Description = c.String(),
                    Created = c.DateTimeOffset(nullable: false, precision: 7),
                    UserId = c.String(maxLength: 128),
                    FileUrl = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.TicketId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.TicketComments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Comment = c.String(nullable: false),
                    Created = c.DateTimeOffset(nullable: false, precision: 7),
                    TicketId = c.Int(nullable: false),
                    UserId = c.String(),
                    Author_Id = c.String(maxLength: 128),
                    Tickets_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Author_Id)
                .ForeignKey("dbo.Tickets", t => t.Tickets_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.Tickets_Id);

            CreateTable(
                "dbo.TicketHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Property = c.String(),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        Changed = c.Boolean(nullable: false),
                        ChangedDate = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.TicketId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.TicketNotifications",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    TicketId = c.Int(nullable: false),
                    UserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.TicketId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    RoleId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.TicketPriorities",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.TicketStatuses",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.TicketTypes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.ProjectUsers",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ProjectId = c.Int(nullable: false),
                    UserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ProjectId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            //CreateTable(
            //    "dbo.ApplicationUserProjects",
            //    c => new
            //    {
            //        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
            //        Projects_Id = c.Int(nullable: false),
            //    })
            //    .PrimaryKey(t => new { t.ApplicationUser_Id, t.Projects_Id })
            //    .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
            //    .ForeignKey("dbo.Projects", t => t.Projects_Id, cascadeDelete: true)
            //    .Index(t => t.ApplicationUser_Id)
            //    .Index(t => t.Projects_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ProjectUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes");
            DropForeignKey("dbo.Tickets", "TicketStatusId", "dbo.TicketStatuses");
            DropForeignKey("dbo.Tickets", "TicketPriorityId", "dbo.TicketPriorities");
            DropForeignKey("dbo.Tickets", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Tickets", "OwnerUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "AssignedUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserProjects", "Projects_Id", "dbo.Projects");
            DropForeignKey("dbo.ApplicationUserProjects", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketNotifications", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketNotifications", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketHistories", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketHistories", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketComments", "Tickets_Id", "dbo.Tickets");
            DropForeignKey("dbo.TicketComments", "Author_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketAttachments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketAttachments", "TicketId", "dbo.Tickets");
            DropIndex("dbo.ApplicationUserProjects", new[] { "Projects_Id" });
            DropIndex("dbo.ApplicationUserProjects", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ProjectUsers", new[] { "UserId" });
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.TicketNotifications", new[] { "UserId" });
            DropIndex("dbo.TicketNotifications", new[] { "TicketId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.TicketHistories", new[] { "UserId" });
            DropIndex("dbo.TicketHistories", new[] { "TicketId" });
            DropIndex("dbo.TicketComments", new[] { "Tickets_Id" });
            DropIndex("dbo.TicketComments", new[] { "Author_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.TicketAttachments", new[] { "UserId" });
            DropIndex("dbo.TicketAttachments", new[] { "TicketId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Tickets", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Tickets", new[] { "AssignedUserId" });
            DropIndex("dbo.Tickets", new[] { "OwnerUserId" });
            DropIndex("dbo.Tickets", new[] { "TicketStatusId" });
            DropIndex("dbo.Tickets", new[] { "TicketPriorityId" });
            DropIndex("dbo.Tickets", new[] { "TicketTypeId" });
            DropIndex("dbo.Tickets", new[] { "ProjectId" });
            DropTable("dbo.ApplicationUserProjects");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ProjectUsers");
            DropTable("dbo.TicketTypes");
            DropTable("dbo.TicketStatuses");
            DropTable("dbo.TicketPriorities");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.TicketNotifications");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.TicketHistories");
            DropTable("dbo.TicketComments");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.TicketAttachments");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Tickets");
            DropTable("dbo.Projects");
        }
    }
}
