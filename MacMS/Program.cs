using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EquipmentManagementSystem.Domain.Data.DbAccess;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EquipmentManagementSystem.Domain.Service.Extension;

namespace EquipmentManagementSystem {

    public partial class Program {


        public static void Main(string[] args) {

            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseSetting("https_port", "443")
            .UseStartup<Startup>()
            .ConfigureLogging(logging => {

                logging.ClearProviders();
                logging.AddConsole();
            });
        
    }
}
