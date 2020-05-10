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
using Serilog;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Rewrite;
using WebEssentials.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCaching;

namespace rkbc
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
            //Configuration = configuration;
            WebHostEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add our Config object so it can be injected
            services.Configure<RkbcConfig>(Configuration.GetSection("RkbcConfig"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<BlogSettings>(Configuration.GetSection("BlogSettings"));


            var keysFolder = Path.Combine(WebHostEnvironment.ContentRootPath, "temp-keys");
            services.AddDataProtection()
                .SetApplicationName("rkbc")
                .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                //options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            }).AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();
            //If using the CookieAuthenticationDefaults.AuthenticationSchem, HttpConctex.User.Indentity doesn't work.


            //services.AddHttpContextAccessor();
            //services.AddResponseCaching();
            //services.AddRazorPages();

            //Ioc
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<UserService>();
            services.AddScoped<BlogService>();
            services.AddSingleton<FileHelper>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x => {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                //var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                //options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddXmlSerializerFormatters()
            .AddRazorRuntimeCompilation()
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(2);
                //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                //options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Expiration = TimeSpan.FromMinutes(3);
                options.Cookie.Path = "/RKBC";
                options.SlidingExpiration = false;
            });
            // Output caching (https://github.com/madskristensen/WebEssentials.AspNetCore.OutputCaching)
            //services.AddOutputCaching(
            //    options =>
            //    {
            //        options.Profiles["default"] = new OutputCacheProfile
            //        {
            //            Duration = 3600
            //        };
            //    });
            //ElmahCore
            EmailOptions emailOptions = new EmailOptions
            {
                MailRecipient = "deoksoonf@gmail.com",
                MailSender = "deoksoonf@gmail.com",
                SmtpServer = "smtp.gmail.com",
                SmtpPort = 587,
                AuthUserName = "deoksoonf@gmail.com",
                AuthPassword = "gmail8516"
            };
            services.AddElmah<XmlFileErrorLog>(options =>
            {
                options.FiltersConfig = "elmah.xml";
                options.LogPath = "./elmahLogs";
                options.Notifiers.Add(new ErrorMailNotifier("Email", emailOptions));
            });
            //services.AddElmah<SqlErrorLog>(options =>
            //{
            //    options.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            //    options.ApplicationName = "RKBC"; //Configuration["RKBC"];
            //    options.Notifiers.Add(new ErrorMailNotifier("Email", emailOptions));
            //});
            // Add functionality to inject IOptions<T>
            //services.AddOptions();

            services.AddAuthentication("AuthCookies")
            .AddCookie("AuthCookies", options => {
                options.LoginPath = "/Administration/Login";
                options.AccessDeniedPath = "/Administration/AccessDenied";
                
                
            });


            services.AddAuthorization(options =>
            {
                //options.AddPolicy("AdminRequired",
                //    policy => policy.RequireRole("Admin"));
            });

            
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.Use(async (context, next) =>
            //{
            //    var url = context.Request.Path.Value;
            //    await context.Response.WriteAsync(context.Request.Path.Value);
                
            //    // Rewrite to index
            //    //if (url.Contains("/home/privacy"))
            //    //{
            //    //    // rewrite and continue processing
            //    //    context.Request.Path = "/home/index";
            //    //}

            //    //await next();
            //});
            //Testing for hosting process
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync(context.Request.Path.Value);
            //    //await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            //});
            //app.UseHttpsRedirection();
            //app.UseDefaultFiles();
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };
            app.UseCookiePolicy(cookiePolicyOptions);
            

            app.UseStaticFiles();
            app.UseSession();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            //app.UseOutputCaching();
            app.UseElmah();
            
           // After UseRouting, so that route information is available for authentication decisions.
            //Before UseEndpoints, so that users are authenticated before accessing the endpoints.
            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMvc();
            //app.UseResponseCaching();

            //app.Use(async (context, next) =>
            //{
            //    context.Response.GetTypedHeaders().CacheControl =
            //        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
            //        {
            //            Public = true,
            //            MaxAge = TimeSpan.FromSeconds(60)
            //        };
            //    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
            //        new string[] { "Accept-Encoding" };
            //    var responseCachingFeature = context.Features.Get<IResponseCachingFeature>();

            //    if (responseCachingFeature != null)
            //    {
            //        responseCachingFeature.VaryByQueryKeys = new[] { "page" };
            //    }

            //    await next();
            //});
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
