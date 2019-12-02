
using EquipmentManagementSystem.Domain.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace EquipmentManagementSystem.Domain.Data.DbAccess {


    public class ManagementContext : DbContext {

        
        public ManagementContext(DbContextOptions<ManagementContext> options) : base(options) {

        }

        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Equipment>()
                .HasKey(e => e.ID);

            modelBuilder.Entity<Owner>()
                .HasKey(o => o.ID);
                

        }


    }
}
