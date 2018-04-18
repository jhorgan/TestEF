using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TestCascadedDelete.Model;

namespace TestCascadedDelete
{
    internal class AppDbContext : DbContext
    {
        public AppDbContext() : base("DatbaseConnectionString")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AppDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // CapacityPlan configuration
            modelBuilder.Entity<CapacityPlan>()
                .HasKey(e => e.Id, config => config.IsClustered(false));

            // ProductionUnit configuration
            // Relationship between CapacityPlan => ProductionUnit
            modelBuilder.Entity<ProductionUnit>()
                .HasRequired(pu => pu.CapacityPlan)
                .WithMany(cp => cp.ProductionUnits)
                .HasForeignKey(pu => pu.CapacityPlanId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductionUnit>()
                .HasKey(e => e.Id, config => config.IsClustered(false));
            modelBuilder.Entity<ProductionUnit>()
                .HasIndex(e => e.CapacityPlanId)
                .IsClustered();
                

            // ProductionUnitMode configuration
            // Relationship between ProductionUnit => ProductionUnitMode
            modelBuilder.Entity<ProductionUnitMode>()
                .HasRequired(pum => pum.ProductionUnit)
                .WithMany(pu => pu.ProductionUnitModes)
                .HasForeignKey(pum => pum.ProductionUnitId)
                .WillCascadeOnDelete(false); 

            modelBuilder.Entity<ProductionUnitMode>()
                .HasKey(e => e.Id, config => config.IsClustered(false));
            modelBuilder.Entity<ProductionUnitMode>()
                .HasIndex(e => e.ProductionUnitId)
                .IsClustered();

            // EfficiencyProfileDetail configuration
            // Relationship between ProductionUnitMode => EfficiencyProfileDetail
            modelBuilder.Entity<EfficiencyProfileDetail>()
                .HasRequired(epd => epd.ProductionUnitMode)
                .WithMany(pum => pum.EfficiencyProfileDetails)
                .HasForeignKey(efd => efd.ProductionUnitModeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EfficiencyProfileDetail>()
                .HasKey(e => e.Id, config => config.IsClustered(false));
            modelBuilder.Entity<EfficiencyProfileDetail>()
                .HasIndex(e => e.ProductionUnitModeId)
                .IsClustered();
        }

        public DbSet<CapacityPlan> CapacityPlans { get; set; }
        public DbSet<ProductionUnit> ProductionUnits { get; set; }
        public DbSet<ProductionUnitMode> ProductionUnitModes { get; set; }
        public DbSet<EfficiencyProfileDetail> EfficiencyProfileDetails { get; set; }
    }
}
