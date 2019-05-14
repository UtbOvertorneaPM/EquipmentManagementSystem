using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Razor;

using EquipmentManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Localization;
using System.Reflection;
using Microsoft.Extensions.Options;
using EquipmentManagementSystem.Data;
using Microsoft.Extensions.FileProviders;

namespace EquipmentManagementSystem {


    public class Startup {


        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration) {

            Configuration = configuration;
        }
              
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            
            services.Configure<CookiePolicyOptions>(options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            services.AddMemoryCache();
            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(
                options => {
                    var supportedCultures = new List<CultureInfo> {
                                    new CultureInfo("en-GB"),
                                    new CultureInfo("sv-SE")
                    };

                    options.DefaultRequestCulture = new RequestCulture("en-GB");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                    options.RequestCultureProviders = new List<IRequestCultureProvider> {
                        new QueryStringRequestCultureProvider(),
                        new CookieRequestCultureProvider()                     
                    };                    
                }
            );

            services.AddSingleton<Localizer>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options => {
                    options.DataAnnotationLocalizerProvider = (type, factory) => {
                        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("SharedResource", assemblyName.Name);
                    };
                });                

            var path = "";
#if DEBUG
            path = @"C:\Users\peter\source\repos\prodSettings.json";
#else
            path = @"C:\EMS\prodSettings.json";
#endif

            var credentials = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText(path)).Credentials;
            var connection = $"Server=localhost;port=3306;Database=EquipmentManagementSystem;user={credentials.User};password={credentials.Password}";

            services.AddDbContextPool<ManagementContext>(
                options => options.UseMySql(connection,
                mySqlOptionsAction => {
                    mySqlOptionsAction.ServerVersion(new Version(8, 0, 16), ServerType.MySql);
                }));
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {

            if (env.IsDevelopment()) {

                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRequestLocalization();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


    }
}
