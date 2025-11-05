using ElectricityCuttingDown.WebPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace ElectricityCuttingDown.WebPortal.Data
{
    public class Electricity_FTAContext : DbContext
    {
        public Electricity_FTAContext(DbContextOptions<Electricity_FTAContext> options)
            : base(options) { }

        public DbSet<Cutting_Down_Header> Cutting_Down_Header { get; set; }
        public DbSet<Cutting_Down_Detail> Cutting_Down_Detail { get; set; }
        public DbSet<Network_Element> Network_Element { get; set; }

        public DbSet<Cutting_Down_Ignored> Cutting_Down_Ignored { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Cutting_Down_Header>()
    .HasKey(x => x.Cutting_Down_Key);

            modelBuilder.Entity<Cutting_Down_Header>()
                .Property(x => x.Cutting_Down_Key)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Cutting_Down_Ignored>()
                .HasKey(x => x.Cutting_Down_Incident_ID);

            modelBuilder.Entity<Cutting_Down_Detail>()
                .HasKey(x => x.Cutting_Down_Detail_Key);

            modelBuilder.Entity<Network_Element>()
                .HasKey(x => x.Network_Element_Key);

            modelBuilder.Entity<Cutting_Down_Ignored>()
                .ToTable("Cutting_Down_Ignored")
                .HasKey(x => x.Cutting_Down_Incident_ID);
        }
    }
}
