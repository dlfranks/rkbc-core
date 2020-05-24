using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using rkbc.config.models;
using rkbc.core.models;
using rkbc.web.viewmodels;

namespace rkbc.core.repository
{
    public class TableChange
    {
        public int id { get; set; }
        public string Action { get; set; }
        public string ColumnName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string TableName { get; set; }
        public string TableID { get; set; }

    }
    public class DBAudit:IEntity
    {
        public int id { get; set; }
        public string action { get; set; }
        public DateTime date { get; set; }
        public string user { get; set; }
        public string recordTypeName { get; set; }
        public string singleId { get; set; }
        public string complexId { get; set; }
        public string recordData { get; set; }
    }
    public class ApplicationDbContext : AuditableDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpAccessor, IOptions<RkbcConfig> rkbcConfig)
            : base(options, httpAccessor, rkbcConfig)
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
        public DbSet<HomePage> HomePages { get; set; }
        public DbSet<PastorPage> PastorPages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<HomeContentItem> HomeContentItems { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
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

    public class AuditableDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
                                                          ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
                                                          ApplicationRoleClaim, ApplicationUserToken>
    {
        protected IHttpContextAccessor httpAccessot;
        protected IOptions<RkbcConfig> rkbcSetting;
        public AuditableDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor _httpAccessot, IOptions<RkbcConfig> _rkbcConfig) : base(options)
        {
            httpAccessot = _httpAccessot;
            rkbcSetting = _rkbcConfig;
        }
        public DbSet<DBAudit> DBAudits { get; set; }
        protected class ChangeTrack
        {
            public EntityEntry entry;
            public EntityState state;
            public DBAudit audit;

            public IList<string> modifiedProp;
        }
        protected string getUserName()
        {
            var _user = httpAccessot.HttpContext.User;
            return (_user == null ? "" : (_user.Identity == null ? "" : _user.Identity.Name));
        }
        protected void auditEntity(EntityEntry entry, IList<string> modifiedProps, StringBuilder json)
        {
            //Start the object
            if (entry == null)
            {
                json.Append("null");
                return;
            }
            json.Append("{");

            //We have a list of properties to record
            if (modifiedProps != null)
            {
                bool next = false;
                foreach (string o in modifiedProps)
                {
                    if (next) json.Append(", ");
                    object val = entry.Property(o).CurrentValue;
                    json.Append(("\"" + o + "\"") + ":");
                    if (val is EntityEntry)
                        auditEntity((EntityEntry)val, null, json);
                    else
                    {
                        if (val != null)
                            json.Append("\"" + val.ToString() + "\"");
                        else
                            json.Append("null");
                    }
                    next = true;
                }
            }
            //Loop through the entire object
            else
            {
                bool next = false;
                foreach (var property in entry.CurrentValues.Properties)
                {
                    var val = entry.Property(property.Name).CurrentValue;
                    if (next) json.Append(", ");
                    json.Append("\"" + property.Name + "\"" + " : ");
                    if (val is EntityEntry)
                        auditEntity((EntityEntry)val, null, json);
                    else
                    {
                        if (val != null)
                            json.Append("\"" + val.ToString() + "\"");
                        else
                            json.Append("null");
                    }
                    next = true;
                }
            }
            json.Append("}");
        }

        protected void setEntityAuditKey(EntityEntry entry, DBAudit audit)
        {
            audit.singleId = null;
            audit.complexId = null;

            //Setup the id, almost always is IEntity with single id
            var primaryKey = entry.Metadata.FindPrimaryKey();
            var keys = primaryKey.Properties.ToDictionary(x => x.Name, x => x.PropertyInfo.GetValue(entry.Entity)).ToList();

            if (keys.Count == 1)
            {
                audit.singleId = keys.First().Value.ToString();
            }
            //Supports other comination keys or not integer keys
            else
            {
                int count = keys.Count();
                audit.complexId = "{";
                bool next = false;
                for (int i = 0; i < count; i++)
                {
                    if (next) audit.complexId += ",";
                    audit.complexId += keys[i].Key + ":" + keys[i].Value.ToString();
                    next = true;
                }
                audit.complexId += "}";
            }
        }
        protected List<ChangeTrack> performAuditSetup(ChangeTracker tracker, bool withInlineAudit, bool withAuditStamp)
        {
            List<ChangeTrack> modLst = new List<ChangeTrack>();
            if (withInlineAudit || withAuditStamp)
            {
                //Log the modified objects before updating them.  This is logged here and audited
                //later since SaveChanges will change all the identity id values and FK properties   



                foreach (EntityEntry entry in tracker.Entries())
                {
                    //Verify modified and not DBAudit entry, relationships are not handled as 
                    //many to many is not supported
                    if (!(entry.Entity is DBAudit) &&
                        (entry.State == EntityState.Added ||
                        entry.State == EntityState.Deleted ||
                        entry.State == EntityState.Modified))
                    {
                        //Handle IAuditable (note these changes are made after DetectChanges,
                        //so while they will be saved they will not be audited inline)
                        if (withAuditStamp && entry.Entity is IAuditStamp)
                        {
                            var table = entry.Entity as IAuditStamp;
                            if (entry.State == EntityState.Added)
                            {

                                table.createDt = DateTime.Now;
                                table.createUser = getUserName();
                            }
                            if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                            {
                                table.lastUpdDt = DateTime.Now;
                                table.lastUpdUser = getUserName();
                            }
                        }

                        //Handle inline auditing
                        if (withInlineAudit)
                        {
                            var track = new ChangeTrack();
                            track.entry = entry;
                            track.state = entry.State;
                            if (entry.State == EntityState.Modified)
                                track.modifiedProp = entry.Properties.Where(q => q.IsModified == true).Select(r => r.Metadata.Name).ToList();

                            //Setup the auditable record
                            track.audit = new DBAudit();
                            track.audit.date = DateTime.Now;
                            track.audit.user = getUserName();

                            //Find the entity name minus the proxy wrapper if wrapped (if you name 
                            //your class hierarchry [name]_[othername] and you inherit from this
                            //with [name] then pass in a non-proxied instance of [name] this will 
                            //break) There was no other apparent way to find the non-proxied name.                    
                            var eproxy = entry.Entity.GetType();
                            var ename = eproxy.FullName;
                            if (eproxy.FullName.StartsWith("System.Data.Entity.DynamicProxies"))
                                ename = eproxy.BaseType.FullName;
                            track.audit.recordTypeName = ename;

                            if (entry.State == EntityState.Added)
                                track.audit.action = "C";
                            if (entry.State == EntityState.Deleted)
                            {
                                track.audit.action = "D";
                                //Can't be done after deletion
                                setEntityAuditKey(entry, track.audit);
                            }
                            if (entry.State == EntityState.Modified)
                                track.audit.action = "U";

                            modLst.Add(track);
                        }
                    }
                }
            }
            return (modLst);
        }
        protected async Task performAuditSave(List<ChangeTrack> modLst, bool withInlineAudit)
        {
            if (withInlineAudit)
            {
                foreach (var change in modLst)
                {
                    if (change.state != EntityState.Deleted)
                    {
                        //Set the keys
                        setEntityAuditKey(change.entry, change.audit);

                        //Set the data
                        var json = new StringBuilder();
                        auditEntity(change.entry, change.modifiedProp, json);
                        change.audit.recordData = json.ToString();
                    }
                    //Nothing to be done for deleted here

                    DBAudits.Add(change.audit);
                }

                await base.SaveChangesAsync();
            }
        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            bool withInlineAudit = rkbcSetting.Value.DbContextAuditInline;
            bool withStamp = rkbcSetting.Value.DbContextAuditStamp;
            //Validation in EF Core
            var entities = from e in ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(entity, validationContext);
            }

            int saveResult;
            //Auditing to the database
            if (withInlineAudit || withStamp)
            {
                List<ChangeTrack> modLst = performAuditSetup(this.ChangeTracker, withInlineAudit, withStamp);
                
                saveResult = await base.SaveChangesAsync(cancellationToken);
                
                await performAuditSave(modLst, withInlineAudit);
                
            }
            else
            {
                saveResult = await base.SaveChangesAsync(cancellationToken);
            }

            return saveResult;
        }
    }
}
