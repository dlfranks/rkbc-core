using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using rkbc.core.models;
using rkbc.core.repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Hosting;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using ElmahCore.Mvc.Notifiers;
using ElmahCore;
using rkbc.core.helper;
using rkbc.map.models;
using rkbc.core.service;
using System.IO;
using rkbc.config.models;

namespace rkbc.config.models
{
    public class RkbcConfig
    {
        public int HomePageId { get; set; }
        public string Version { get; set; }
    }
}
namespace rkbc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                //.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            })
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                //.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();
            services.AddAuthorization(options =>
            {
                //options.AddPolicy("AdminRequired",
                //    policy => policy.RequireRole("Admin"));
            });
            services.ConfigureApplicationCookie(options =>
            {
                //options.Cookie.HttpOnly = true;
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(100);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Administration/AccessDenied";
                options.SlidingExpiration = true;
            });
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            //services.AddRazorPages();

            //Ioc
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<UserService>();
            services.AddSingleton<FileHelper>();
            


            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                //var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                //options.Filters.Add(new AuthorizeFilter(policy));
            })//.AddXmlSerializerFormatters()
            .AddRazorRuntimeCompilation()
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //ElmahCore
            //EmailOptions emailOptions = new EmailOptions
            //{
            //    MailRecipient = "deana.franks@woodplc.com",
            //    MailSender = "deana.franks@woodplc.com",
            //    SmtpServer = "imts@amecfw.com",
            //    SmtpPort = 25,
            //    AuthUserName = "ImtsADQueryUser",
            //    AuthPassword = "qu3rYAD!mtsUsEr"
            //};
            services.AddElmah<XmlFileErrorLog>(options =>
            {
                options.FiltersConfig = "elmah.xml";
                options.LogPath = "~/logs";
                //options.Notifiers.Add(new ErrorMailNotifier("Email", emailOptions));
            });
            //services.AddElmah<SqlErrorLog>(options =>
            //{
            //    options.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            //    options.ApplicationName = "RKBC"; //Configuration["RKBC"];
            //    options.Notifiers.Add(new ErrorMailNotifier("Email", emailOptions));
            //});
            // Add functionality to inject IOptions<T>
            services.AddOptions();

            // Add our Config object so it can be injected
            services.Configure<RkbcConfig>(Configuration.GetSection("RkbcConfig"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.Use(async (context, next) =>
            //{
            //    var url = context.Request.Path.Value;

            //    // Rewrite to index
            //    //if (url.Contains("/home/privacy"))
            //    //{
            //    //    // rewrite and continue processing
            //    //    context.Request.Path = "/home/index";
            //    //}

            //    await next();
            //});
            //Testing for hosting process
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync(context.Request.Path.Value);
            //    //await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            //});
            app.UseHttpsRedirection();
            //app.UseDefaultFiles();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSession();
            app.UseElmah();
            app.UseRouting();
           // After UseRouting, so that route information is available for authentication decisions.
            //Before UseEndpoints, so that users are authenticated before accessing the endpoints.
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvc();
            app.UseEndpoints(endpoints =>
            {

                //endpoints.MapControllers();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();

            });

        }
    }
}
