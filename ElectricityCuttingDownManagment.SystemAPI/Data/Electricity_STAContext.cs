using ElectricityCuttingDownManagmentSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricityCuttingDownManagmentSystem.API.Data
{

    public class Electricity_STAContext : DbContext
    {
        public Electricity_STAContext(DbContextOptions<Electricity_STAContext> options)
        : base(options)
        {
        }

        // DbSets
        public DbSet<Cutting_Down_A> Cutting_Down_A { get; set; }
        public DbSet<Cutting_Down_B> Cutting_Down_B { get; set; }
        public DbSet<Cabin> Cabin { get; set; }
        public DbSet<Cable> Cable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // تكوين جدول Cabin
            modelBuilder.Entity<Cabin>(entity =>
            {
                entity.ToTable("Cabin");
                entity.HasKey(e => e.Cabin_Key);

                entity.Property(e => e.Cabin_Key)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Tower_Key)
                    .IsRequired();

                entity.Property(e => e.Cabin_Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // تكوين جدول Cable
            modelBuilder.Entity<Cable>(entity =>
            {
                entity.ToTable("Cable");
                entity.HasKey(e => e.Cable_Key);

                entity.Property(e => e.Cable_Key)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Cabin_Key)
                    .IsRequired();

                entity.Property(e => e.Cable_Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // علاقة Cable مع Cabin
                entity.HasOne<Cabin>()
                    .WithMany()
                    .HasForeignKey(e => e.Cabin_Key)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // تكوين جدول Cutting_Down_A
            modelBuilder.Entity<Cutting_Down_A>(entity =>
            {
                entity.ToTable("Cutting_Down_A");
                entity.HasKey(e => e.CuttingDownAIncidentID);

                entity.Property(e => e.CuttingDownAIncidentID)
                    .HasColumnName("Cutting_Down_A_Incident_ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CabinKey)
                    .IsRequired()
                    .HasColumnName("Cabin_Key");

                entity.Property(e => e.ProblemTypeKey)
                    .IsRequired()
                    .HasColumnName("Problem_Type_Key");

                entity.Property(e => e.CreateDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.EndDate)
                    .IsRequired(false);

                entity.Property(e => e.IsPlanned)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.IsGlobal)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.PlannedStartDTS)
                    .IsRequired(false);

                entity.Property(e => e.PlannedEndDTS)
                    .IsRequired(false);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Source A user");

                entity.Property(e => e.UpdatedUser)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(e => e.IsProcessed)
                    .IsRequired()
                    .HasDefaultValue(false);
                entity.HasOne<Cabin>()
                  .WithMany()
                  .HasForeignKey(e => e.CabinKey)
                  .OnDelete(DeleteBehavior.Restrict);

                // Index لتحسين الأداء
                entity.HasIndex(e => e.CabinKey);
                entity.HasIndex(e => e.ProblemTypeKey);
                entity.HasIndex(e => e.CreateDate);
                entity.HasIndex(e => e.EndDate);
                entity.HasIndex(e => e.IsActive);
            });

            // تكوين جدول Cutting_Down_B
            modelBuilder.Entity<Cutting_Down_B>(entity =>
            {
                entity.ToTable("Cutting_Down_B");
                entity.HasKey(e => e.CuttingDownBIncidentID);

                entity.Property(e => e.CuttingDownBIncidentID)
                    .HasColumnName("Cutting_Down_B_Incident_ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CableKey)
                    .IsRequired()
                    .HasColumnName("Cable_Key");

                entity.Property(e => e.ProblemTypeKey)
                    .IsRequired()
                    .HasColumnName("Problem_Type_Key");

                entity.Property(e => e.CreateDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.EndDate)
                    .IsRequired(false);

                entity.Property(e => e.IsPlanned)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.IsGlobal)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.PlannedStartDTS)
                    .IsRequired(false);

                entity.Property(e => e.PlannedEndDTS)
                    .IsRequired(false);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Source B user");

                entity.Property(e => e.UpdatedUser)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(e => e.IsProcessed)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.HasOne<Cable>()
                   .WithMany()
                   .HasForeignKey(e => e.CableKey)
                   .OnDelete(DeleteBehavior.Restrict);


                // Index 
                entity.HasIndex(e => e.CableKey);
                entity.HasIndex(e => e.ProblemTypeKey);
                entity.HasIndex(e => e.CreateDate);
                entity.HasIndex(e => e.EndDate);
                entity.HasIndex(e => e.IsActive);
            });



        }
    }

}