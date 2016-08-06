namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUserProjects", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserProjects", "Projects_Id", "dbo.Projects");
            DropIndex("dbo.ApplicationUserProjects", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserProjects", new[] { "Projects_Id" });
            AddColumn("dbo.AspNetUsers", "Projects_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "Projects_Id");
            AddForeignKey("dbo.AspNetUsers", "Projects_Id", "dbo.Projects", "Id");
            DropTable("dbo.ApplicationUserProjects");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationUserProjects",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Projects_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Projects_Id });
            
            DropForeignKey("dbo.AspNetUsers", "Projects_Id", "dbo.Projects");
            DropIndex("dbo.AspNetUsers", new[] { "Projects_Id" });
            DropColumn("dbo.AspNetUsers", "Projects_Id");
            CreateIndex("dbo.ApplicationUserProjects", "Projects_Id");
            CreateIndex("dbo.ApplicationUserProjects", "ApplicationUser_Id");
            AddForeignKey("dbo.ApplicationUserProjects", "Projects_Id", "dbo.Projects", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserProjects", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
