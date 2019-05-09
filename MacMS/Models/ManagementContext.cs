using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace EquipmentManagementSystem.Models {


    public class ManagementContext : DbContext {


        public ManagementContext(DbContextOptions<ManagementContext> options) : base(options) {

        }


        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Owner> Owners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Owner>()
                .HasMany(e => e.Equipment)
                .WithOne(o => o.Owner);

            modelBuilder.Entity<Equipment>()
                .HasOne(o => o.Owner);
    
        }


    }
}
