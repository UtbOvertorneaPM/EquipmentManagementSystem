
using EquipmentManagementSystem.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace EquipmentManagementSystem.Domain.Data.DbAccess {


    public class ManagementContext : DbContext {


        public ManagementContext(DbContextOptions<ManagementContext> options) : base(options) {

        }


        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Owner> Owners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Equipment>()
                .HasKey(e => e.ID);

            modelBuilder.Entity<Owner>()
                .HasKey(o => o.ID);
                

        }


    }
}
