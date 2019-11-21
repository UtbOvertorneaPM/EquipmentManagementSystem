using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace EquipmentManagementSystem.Domain.Data.DbAccess {


    public class UserContext : ManagementContext {

        public UserContext(DbContextOptions<ManagementContext> options) : base(options) {

        }

        public DbSet<User> Users { get; set;}

    }

    public class User {

        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
    }
}
