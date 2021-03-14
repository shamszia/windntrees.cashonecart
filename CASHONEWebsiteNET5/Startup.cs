using Application.Models.Configuration;
using Application.Services;
using DataAccessNET5.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class Startup
    {
        #region Configuration
        private static IConfiguration _configuration = null;
        public static IConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }
        #endregion

        #region APP
        private static IApplicationBuilder _app = null;
        public static IApplicationBuilder APP
        {
            get
            {
                return _app;
            }
        }
        #endregion

        #region ENV
        private static IWebHostEnvironment _env = null;
        public static IWebHostEnvironment ENV
        {
            get
            {
                return _env;
            }
        }
        #endregion
        
        #region Services
        private static IServiceCollection _services = null;
        public static IServiceCollection Services
        {
            get
            {
                return _services;
            }
        }
        #endregion

        #region Antiforgery
        private static IAntiforgery _antiforgery = null;
        public static IAntiforgery Antiforgery
        {
            get
            {
                return _antiforgery;
            }
        }
        #endregion

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection")));

            services.AddDistributedMemoryCache();
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = options.FormFieldName;
                options.HeaderName = options.FormFieldName;
            });
            
            services.AddDataProtection(l =>
                l.ApplicationDiscriminator = Configuration.GetSection("ApplicationSettings")["ApplicationID"]
            );
            services.AddTransient<IEmailer, Emailer>();
            services.AddSession(options =>
            {
                options.Cookie.Name = ".Cashone.Session";
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromSeconds(int.Parse(Configuration.GetSection("ApplicationSettings")["SessionTimeOut"]));
            });

            services.AddLocalization();

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.AddControllersWithViews()
                .AddNewtonsoftJson()
                .AddSessionStateTempDataProvider();
            services.AddRazorPages()
                .AddSessionStateTempDataProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            _app = app;
            _env = env;
            _antiforgery = antiforgery;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
