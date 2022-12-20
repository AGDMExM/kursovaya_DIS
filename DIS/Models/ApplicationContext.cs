using Microsoft.EntityFrameworkCore;

namespace DIS.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PostOffice> PostOffices { get; set; }
        public DbSet<Package> Packages { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            Users.Load();
            Roles.Load();
            PostOffices.Load();
            Packages.Load();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminLogin = "admin";
            string adminPassword = "admin";

            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };
            User adminUser = new User { 
                Id = 1, 
                FIO = "Administrator", 
                PassportNumber = 0, 
                PassportSeries = 0, 
                Login = adminLogin, 
                Password = adminPassword, 
                RoleId = adminRole.Id 
            };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            base.OnModelCreating(modelBuilder);
        }
    }
}
