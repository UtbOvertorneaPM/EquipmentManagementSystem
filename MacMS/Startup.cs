using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using EquipmentManagementSystem.Models;
using EquipmentManagementSystem.Data;
using EquipmentManagementSystem.Business.Data;
using EquipmentManagementSystem.Domain.Data.DbAccess;
using EquipmentManagementSystem.Domain.Service;
using EquipmentManagementSystem.Domain.Service.Authorization;

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

            // Gets connectionstring and crendentials
            var path = "";
#if DEBUG
            path = @"C:\Users\peter\source\repos\prodSettings.json";
#elif RELEASE

            path = $"{Configuration.GetValue<string>(WebHostDefaults.ContentRootKey)}" + @"\prodSettings.json";
#endif

            var credentials = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText(path)).Credentials;
            var connection = $"Server={credentials.Server};port=3306;Database={credentials.DbName};user={credentials.User};password={credentials.Password}";

            var roles = "";

            // Sets up roles that have access using policy
#if DEBUG
            roles = credentials.DebugDomain;
#elif RELEASE
            roles = credentials.Domain;
#endif

            roles = roles.Replace("\\", @"\");
            


            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            
            // Sets Localization to use SharedResource.sv-SE.resx
            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

            // Adds in Localization services
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



            // Inserts Localization into MVC framework
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options => {
                    options.DataAnnotationLocalizerProvider = (type, factory) => {
                        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("SharedResource", assemblyName.Name);
                    };
                });

            services.AddMvcCore().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);
            services.AddMvcCore().AddRazorViewEngine();

            //services.AddSingleton<IAuthorizationHandler, RoleAuthentication>();
            //services.AddSingleton<IAuthorizationHandler, UserAuthenticationHandler>();
            services.AddTransient<IAuthorizationHandler, UserAuthenticationHandler>();

            // Sets database to MySQL, and connects it to database using ManagementContext
            services.AddDbContextPool<ManagementContext>(
                options => options.UseMySQL(connection));

            services.AddAuthorization(options => {
                options.AddPolicy("Administrators", policy => {
                    policy.Requirements.Add(
                        new UserRequirement(services
                            .BuildServiceProvider()
                            .GetService<ManagementContext>()
                            .Users
                            .ToArrayAsync()
                            .Result));
                });
                
            });

            services.AddSingleton<Localizer>();
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
                    template: "{controller=Login}/{action=Login}/{id?}");
            });
        }


    }
}
