using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentManagementSystem.Domain.Service.Extension {

    public static class WebHostExtension {

        public static IWebHost Database<T>(this IWebHost webHost) where T : DbContext {

            var serviceScopeFactory = (IServiceScopeFactory)webHost
                .Services.GetService(typeof(IServiceScopeFactory));

            using (var scope = serviceScopeFactory.CreateScope()) {
                var services = scope.ServiceProvider;

                var dbContext = services.GetRequiredService<T>();
                //dbContext.Database.Migrate();
            }

            return webHost;
        }
    }
}
