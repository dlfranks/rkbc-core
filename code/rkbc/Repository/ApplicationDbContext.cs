using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using rkbc.core.models;
using rkbc.web.viewmodels;

namespace rkbc.core.repository
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
                                                          ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
                                                          ApplicationRoleClaim, ApplicationUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //disable initializer
            
        }
        public DbSet <ApplicationUser> applicationUsers { get; set; }
        public DbSet<ApplicationRole> applicationRoles { get; set; }
        public DbSet<ApplicationUserRole> applicationUserRoles { get; set; }
        public DbSet<ApplicationUserClaim> applicationUserClaims { get; set; }
        public DbSet<ApplicationUserLogin> applicationUserLogins { get; set; }
        public DbSet<ApplicationRoleClaim> applicationRoleClaims { get; set; }
        public DbSet<ApplicationUserToken> applicationUserTokens { get; set; }
        public DbSet<HomePage> homePages { get; set; }
        public DbSet<Attachment> attachments { get; set; }
        public DbSet<VideoAttachment> videoAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
                b.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            });
            modelBuilder.Entity<ApplicationRole>(b =>
            {
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });
            modelBuilder.Entity<ApplicationUserRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId);

                b.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId);

            });
            modelBuilder.Entity<ApplicationUserClaim>(b =>
            {
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                // Each Role can have many entries in the UserRole join table
                b.HasOne(x => x.User)
                .WithMany(x => x.Claims)
                .HasForeignKey(x => x.UserId);
            });
            modelBuilder.Entity<ApplicationUserLogin>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasOne(x => x.User)
                .WithMany(x => x.Logins)
                .HasForeignKey(x => x.UserId);
            });
            modelBuilder.Entity<ApplicationRoleClaim>(b =>
            {
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                // Each Role can have many entries in the UserRole join table
                b.HasOne(x => x.Role)
                .WithMany(x => x.RoleClaims)
                .HasForeignKey(x => x.RoleId);
            });
            modelBuilder.Entity<ApplicationUserToken>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasOne(x => x.User)
                .WithMany(x => x.Tokens)
                .HasForeignKey(x => x.UserId);
            });
        }

        
    }
}
