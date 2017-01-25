using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Awpbs.Web
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(null); // this disables checking for the correct version of the database
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Awpbs.Friendship>()
                .HasRequired(i => i.Athlete1)
                .WithMany(i => i.Friendships1)
                .HasForeignKey(i => i.Athlete1ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Awpbs.Friendship>()
                .HasRequired(i => i.Athlete2)
                .WithMany(i => i.Friendships2)
                .HasForeignKey(i => i.Athlete2ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Awpbs.Score>()
                .HasRequired(i => i.AthleteA)
                .WithMany(i => i.ScoreAs)
                .HasForeignKey(i => i.AthleteAID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Awpbs.Score>()
                .HasRequired(i => i.AthleteB)
                .WithMany(i => i.ScoreBs)
                .HasForeignKey(i => i.AthleteBID)
                .WillCascadeOnDelete(false);

            // set ON DELETE NO ACTION on AthleteID for GameHostInvite and GameHostComment (otherwise there would be an exception in EF)
            modelBuilder.Entity<Awpbs.GameHostInvite>()
                .HasRequired(i => i.Athlete)
                .WithMany(i => i.GameHostInvites)
                .HasForeignKey(i => i.AthleteID)
                .WillCascadeOnDelete(false);
            //modelBuilder.Entity<Awpbs.GameHostComment>()
            //    .HasRequired(i => i.Athlete)
            //    .WithMany(i => i.GameHostComments)
            //    .HasForeignKey(i => i.AthleteID)
            //    .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<IdentityUserLogin> UserLogins { get; set; }

        public System.Data.Entity.DbSet<Awpbs.Quote> Quotes { get; set; }
        public System.Data.Entity.DbSet<Awpbs.Sport> Sports { get; set; }
        public System.Data.Entity.DbSet<Awpbs.ResultType> ResultTypes { get; set; }
        public System.Data.Entity.DbSet<Awpbs.Result> Results { get; set; }
        public System.Data.Entity.DbSet<Awpbs.Score> Scores { get; set; }
        public System.Data.Entity.DbSet<Awpbs.Athlete> Athletes { get; set; }
        public System.Data.Entity.DbSet<Awpbs.Venue> Venues { get; set; }
        public System.Data.Entity.DbSet<Awpbs.Metro> Metros { get; set; }
        public System.Data.Entity.DbSet<Awpbs.Friendship> Friendships { get; set; }
        public System.Data.Entity.DbSet<Awpbs.VenueEdit> VenueEdits { get; set; }

        public System.Data.Entity.DbSet<Awpbs.GameHost> GameHosts { get; set; }
        public System.Data.Entity.DbSet<Awpbs.GameHostInvite> GameHostInvites { get; set; }
        //public System.Data.Entity.DbSet<Awpbs.GameHostComment> GameHostComments { get; set; }

        public System.Data.Entity.DbSet<Awpbs.DeviceToken> DeviceTokens { get; set; }
        public System.Data.Entity.DbSet<Awpbs.PushNotification> PushNotifications { get; set; }
        public System.Data.Entity.DbSet<Awpbs.DeviceInfo> DeviceInfos { get; set; }

        public System.Data.Entity.DbSet<Awpbs.Post> Posts { get; set; }
        public System.Data.Entity.DbSet<Awpbs.Comment> Comments { get; set; }

    }
}