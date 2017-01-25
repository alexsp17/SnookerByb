namespace Awpbs.Web.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Athletes",
                c => new
                    {
                        AthleteID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Name = c.String(),
                        IsPro = c.Boolean(nullable: false),
                        DOB = c.DateTime(),
                        Gender = c.Int(nullable: false),
                        Country = c.String(),
                        HasFacebook = c.Boolean(nullable: false),
                        SportID1 = c.Int(nullable: false),
                        SportID2 = c.Int(nullable: false),
                        SportID3 = c.Int(nullable: false),
                        SportID4 = c.Int(nullable: false),
                        SportID5 = c.Int(nullable: false),
                        TimeModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AthleteID);

            CreateTable(
                "dbo.Friendships",
                c => new
                    {
                        FriendshipID = c.Int(nullable: false, identity: true),
                        Athlete1ID = c.Int(nullable: false),
                        Athlete2ID = c.Int(nullable: false),
                        FriendshipStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FriendshipID)
                .ForeignKey("dbo.Athletes", t => t.Athlete1ID)
                .ForeignKey("dbo.Athletes", t => t.Athlete2ID)
                .Index(t => t.Athlete1ID)
                .Index(t => t.Athlete2ID);

            CreateTable(
                "dbo.Results",
                c => new
                    {
                        ResultID = c.Int(nullable: false, identity: true),
                        AthleteID = c.Int(nullable: false),
                        ResultTypeID = c.Int(nullable: false),
                        Time = c.Double(),
                        Distance = c.Double(),
                        Count = c.Int(),
                        Count2 = c.Int(),
                        Date = c.DateTime(),
                        WorldRecord = c.Int(nullable: false),
                        AgeGroup = c.Int(nullable: false),
                        Notes = c.String(),
                        VenueID = c.Int(),
                        TimeModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Guid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ResultID)
                .ForeignKey("dbo.Athletes", t => t.AthleteID, cascadeDelete: true)
                .ForeignKey("dbo.ResultTypes", t => t.ResultTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Venues", t => t.VenueID)
                .Index(t => t.AthleteID)
                .Index(t => t.ResultTypeID)
                .Index(t => t.VenueID);

            CreateTable(
                "dbo.ResultTypes",
                c => new
                    {
                        ResultTypeID = c.Int(nullable: false),
                        SportID = c.Int(nullable: false),
                        Name = c.String(),
                        ShortName = c.String(),
                        Distance = c.Double(),
                        Time = c.Double(),
                        IsCountRequired = c.Boolean(nullable: false),
                        IsCount2Available = c.Boolean(nullable: false),
                        CountName = c.String(),
                        Count2Name = c.String(),
                    })
                .PrimaryKey(t => t.ResultTypeID)
                .ForeignKey("dbo.Sports", t => t.SportID, cascadeDelete: true)
                .Index(t => t.SportID);

            CreateTable(
                "dbo.Sports",
                c => new
                    {
                        SportID = c.Int(nullable: false),
                        Name = c.String(),
                        AthleteThatDoesThisSport = c.String(),
                        IsActualSport = c.Boolean(nullable: false),
                        IsPrimarilyDistance = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SportID);

            CreateTable(
                "dbo.Venues",
                c => new
                    {
                        VenueID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Country = c.String(),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                    })
                .PrimaryKey(t => t.VenueID);

            CreateTable(
                "dbo.Metroes",
                c => new
                    {
                        MetroID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Country = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.MetroID);

            CreateTable(
                "dbo.Quotes",
                c => new
                    {
                        QuoteID = c.Int(nullable: false, identity: true),
                        QuoteText = c.String(),
                        Author = c.String(),
                        AuthorCredentials = c.String(),
                        Url = c.String(),
                        SportID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuoteID);

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

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
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Results", "VenueID", "dbo.Venues");
            DropForeignKey("dbo.ResultTypes", "SportID", "dbo.Sports");
            DropForeignKey("dbo.Results", "ResultTypeID", "dbo.ResultTypes");
            DropForeignKey("dbo.Results", "AthleteID", "dbo.Athletes");
            DropForeignKey("dbo.Friendships", "Athlete2ID", "dbo.Athletes");
            DropForeignKey("dbo.Friendships", "Athlete1ID", "dbo.Athletes");
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ResultTypes", new[] { "SportID" });
            DropIndex("dbo.Results", new[] { "VenueID" });
            DropIndex("dbo.Results", new[] { "ResultTypeID" });
            DropIndex("dbo.Results", new[] { "AthleteID" });
            DropIndex("dbo.Friendships", new[] { "Athlete2ID" });
            DropIndex("dbo.Friendships", new[] { "Athlete1ID" });
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Quotes");
            DropTable("dbo.Metroes");
            DropTable("dbo.Venues");
            DropTable("dbo.Sports");
            DropTable("dbo.ResultTypes");
            DropTable("dbo.Results");
            DropTable("dbo.Friendships");
            DropTable("dbo.Athletes");
        }
    }
}
