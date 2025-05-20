using Microsoft.EntityFrameworkCore;
using sp_backend.Models;
using sp_backend_March4.Models;
using WeatherApi.Models;

namespace WeatherApi
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<SubEquipment> SubEquipments { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Nonavailability> Nonavailabilities { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<AccountMission> AccountMissions { get; set; }  // Explicit many-to-many table
        public DbSet<EquipmentMission> EquipmentMissions { get; set; }  // Explicit many-to-many table
        public DbSet<EquipmentStock> EquipmentStocks { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<MessageAgent> MessageAgents { get; set; }
        public DbSet<AccountTraining> AccountTrainings { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<RequestMaintenance> RequestMaintenances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AccountTraining>()
    .HasKey(at => new { at.AccountId, at.TrainingId });

            modelBuilder.Entity<AccountTraining>()
                .HasOne(at => at.Account)
                .WithMany(a => a.AccountTrainings)
                .HasForeignKey(at => at.AccountId);

            modelBuilder.Entity<AccountTraining>()
                .HasOne(at => at.Training)
                .WithMany(t => t.AccountTrainings)
                .HasForeignKey(at => at.TrainingId);
            // Configure Equipment-SubEquipment relationship
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.SubEquipments)
                .WithOne(se => se.Equipment)
                .HasForeignKey(se => se.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Equipment>()
                            .HasOne(e => e.EquipmentStock)
                            .WithMany(es => es.Equipments)
                            .HasForeignKey(e => e.EquipmentStockId)
                            .OnDelete(DeleteBehavior.Cascade);

            // Configure Account uniqueness on Username
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Nonavailability>()
                .HasOne(n => n.Account)
                .WithMany(a => a.Nonavailabilities)
                .HasForeignKey(n => n.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Nonavailability - SubEquipment
            modelBuilder.Entity<Nonavailability>()
                .HasOne(n => n.Equipment)
                .WithMany(se => se.Nonavailabilities)
                .HasForeignKey(n => n.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Many-to-Many: Mission and Account via AccountMission table
            modelBuilder.Entity<AccountMission>()
                .HasKey(am => new { am.AccountId, am.MissionId }); // Composite primary key

            modelBuilder.Entity<AccountMission>()
                .HasOne(am => am.Account)
                .WithMany(a => a.AccountMissions)
                .HasForeignKey(am => am.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccountMission>()
                .HasOne(am => am.Mission)
                .WithMany(m => m.AccountMissions)
                .HasForeignKey(am => am.MissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Many-to-Many: Mission and Equipment via EquipmentMission table
            modelBuilder.Entity<EquipmentMission>()
                .HasKey(em => new { em.EquipmentId, em.MissionId }); // Composite primary key

            modelBuilder.Entity<EquipmentMission>()
                .HasOne(em => em.Equipment)
                .WithMany(e => e.EquipmentMissions)
                .HasForeignKey(em => em.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EquipmentMission>()
                .HasOne(em => em.Mission)
                .WithMany(m => m.EquipmentMissions)
                .HasForeignKey(em => em.MissionId)
                .OnDelete(DeleteBehavior.Cascade);


            // One-to-Many: One SubEquipment has multiple Maintenance records

            modelBuilder.Entity<Maintenance>()
                .HasMany(m => m.Items)
                .WithOne(i => i.Maintenance)
                .HasForeignKey(i => i.MaintenanceId)
                .OnDelete(DeleteBehavior.Cascade); // Delete items if maintenance is deleted

            // One-to-Many: SubEquipment -> Maintenances
            modelBuilder.Entity<SubEquipment>()
                .HasMany(se => se.Maintenances)
                .WithOne(m => m.SubEquipment)
                .HasForeignKey(m => m.SubEquipmentId)
                .OnDelete(DeleteBehavior.Cascade); // Set SubEquipmentId to NULL if subequipment is deleted

            // Ensure AssignedDate in AccountMission has default value (optional)
            modelBuilder.Entity<AccountMission>()
                .Property(am => am.AssignedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Prevents cascade delete

            modelBuilder.Entity<MessageAgent>()
                .HasOne(m => m.Receiver)
                .WithMany(a => a.ReceivedMessages) // Add navigation property in Account
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RequestMaintenance>()
            .HasOne(rm => rm.Maintenance)
            .WithOne(m => m.RequestMaintenance)
            .HasForeignKey<RequestMaintenance>(rm => rm.MaintenanceId)
            .OnDelete(DeleteBehavior.Cascade);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Add general-purpose logic here (e.g., auditing, soft deletes)
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
