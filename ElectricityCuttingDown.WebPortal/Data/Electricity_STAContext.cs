using ElectricityCuttingDown.WebPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace ElectricityCuttingDown.WebPortal.Data
{
    public class Electricity_STAContext : DbContext
    {
        public Electricity_STAContext(DbContextOptions<Electricity_STAContext> options)
            : base(options) { }

        public DbSet<User> dbo_Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map the User entity to the actual table name 'Users' in the default schema
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(u => u.User_Key);

            modelBuilder.Entity<User>()
                .Property(u => u.User_Key)
                .HasColumnName("User_Key");

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasColumnName("Name");

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .HasColumnName("Password");
        }
    }
}
