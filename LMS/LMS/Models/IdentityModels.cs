using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using LMS.Models;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual ApplicationUserInfo UserInfo { get; set; }


        public virtual ICollection<AnswersByUser> AnswersByUsers { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public ApplicationUser() : base()
        {
            AnswersByUsers = new List<AnswersByUser>();
            Groups = new List<Group>();
        }

    }
    public class ApplicationUserInfo
    {
        [Key]
        [ForeignKey("User")]
        public string Id { get; set; }

        public string Image { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<Course> Courses { get; set; }

        public virtual DbSet<ApplicationUserInfo> ApplicationUserInfos { get; set; }

        public virtual DbSet<CourseTag> CourseTags { get; set; }

        public virtual DbSet<Group> Groups { get; set; }

        public virtual  DbSet<UserState> UserStates { get; set; }
        //Topic
        public virtual DbSet<Topic> Topics { get; set; }

        //Event
        public virtual DbSet<Fact> Facts { get; set; }

        public virtual DbSet<FactType> FactTypes { get; set; }

        public virtual DbSet<FactUploadFile> FacttUploadFiles { get; set; }

        public virtual DbSet<FactLink> FactLinks { get; set; }

        //Test
        public virtual DbSet<Test> Tests { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }

        public virtual DbSet<Question> Questions { get; set; }

        public virtual DbSet<Answer> Answers { get; set; }

        public virtual DbSet<AnswersByUser> AnswersByUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Group
            modelBuilder.Entity<Group>()
                .HasOptional(m => m.Course)//0..1
                .WithMany(m => m.Groups)//*
                .HasForeignKey(m => m.Course_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
                .HasOptional(m => m.Topic)
                .WithMany(m => m.Groups)
                .HasForeignKey(m => m.Topic_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
                .HasOptional(m => m.User)
                .WithMany(m => m.Groups)
                .WillCascadeOnDelete(false);

            //AnswersByUser
 

            modelBuilder.Entity<AnswersByUser>()
                .HasOptional(m => m.User)
                .WithMany(m => m.AnswersByUsers)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Test>()
                .HasOptional(m => m.Topic)
                .WithMany(m => m.Tests)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Fact>()
                .HasOptional(m => m.Topic)
                .WithMany(m => m.Facts)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<FactType>()
            //    .HasRequired(m => m.Fact)
            //    .WithRequiredPrincipal(m => m.FactType);


            modelBuilder.Entity<IdentityUserLogin>().Map(c =>
            {
                c.ToTable("UserLogin");
                c.Properties(p => new
                {
                    p.UserId,
                    p.LoginProvider,
                    p.ProviderKey
                });
            }).HasKey(p => new { p.LoginProvider, p.ProviderKey, p.UserId });

            
            modelBuilder.Entity<IdentityRole>().Map(c =>
            {
                c.ToTable("Role");
                c.Property(p => p.Id).HasColumnName("RoleId");
                c.Properties(p => new
                {
                    p.Name
                });
            }).HasKey(p => p.Id);
            modelBuilder.Entity<IdentityRole>().HasMany(c => c.Users).WithRequired().HasForeignKey(c => c.RoleId);

            

            modelBuilder.Entity<ApplicationUser>().Map(c =>
            {
                c.ToTable("User");
                c.Property(p => p.Id).HasColumnName("UserId");
                c.Properties(p => new
                {
                    
                    p.AccessFailedCount,
                    p.Email,
                    p.EmailConfirmed,
                    p.PasswordHash,
                    p.PhoneNumber,
                    p.PhoneNumberConfirmed,
                    p.TwoFactorEnabled,
                    p.SecurityStamp,
                    p.LockoutEnabled,
                    p.LockoutEndDateUtc,
                    p.UserName
                });
            }).HasKey(c => c.Id);
            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Logins).WithOptional().HasForeignKey(c => c.UserId);
            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Claims).WithOptional().HasForeignKey(c => c.UserId);
            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Roles).WithRequired().HasForeignKey(c => c.UserId);

            modelBuilder.Entity<ApplicationUserInfo>().HasRequired(c => c.User).WithOptional(c => c.UserInfo).WillCascadeOnDelete(true);
            //modelBuilder.Entity<ApplicationUserInfo>().Map(c =>
            //{
            //    c.ToTable("UserInfo");
            //    c.Property(p => p.Id).HasColumnName("UserId");
            //    c.Properties(p => new
            //    {
            //        p.FirstName,
            //        p.LastName,
            //        p.Image,
            //        p.Birthday,
            //        p.User
            //    });
            //}).HasKey(c => c.Id);
            

            modelBuilder.Entity<IdentityUserRole>().Map(c =>
            {
                c.ToTable("UserRole");
                c.Properties(p => new
                {
                    p.UserId,
                    p.RoleId
                });
            })
            .HasKey(c => new { c.UserId, c.RoleId });


            modelBuilder.Entity<IdentityUserClaim>().Map(c =>
            {
                c.ToTable("UserClaim");
                c.Property(p => p.Id).HasColumnName("UserClaimId");
                c.Properties(p => new
                {
                    p.UserId,
                    p.ClaimValue,
                    p.ClaimType
                });
            }).HasKey(c => c.Id);
        }

        
    }
}