using Microsoft.EntityFrameworkCore;
using sp_backend.Models;
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
        public DbSet<Mission> Missions { get; set; } // Added Mission entity

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Equipment-SubEquipment relationship
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.SubEquipments)
                .WithOne(se => se.Equipment)
                .HasForeignKey(se => se.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Account uniqueness on Username
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Username)
                .IsUnique();

            // Relationship between Account and Nonavailability
            modelBuilder.Entity<Nonavailability>()
                .HasOne(n => n.Account)
                .WithMany(a => a.Nonavailabilities)
                .HasForeignKey(n => n.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mission and Assigned Accounts (Many-to-Many)
            modelBuilder.Entity<Mission>()
                .HasMany(m => m.AssignedAccounts)
                .WithMany(a => a.Missions)
                .UsingEntity(j => j.ToTable("MissionAccounts")); // Many-to-many relationship table

            // Mission and Assigned Equipment (Many-to-Many)
            modelBuilder.Entity<Mission>()
                .HasMany(m => m.AssignedEquipment)
                .WithMany(e => e.Missions)
                .UsingEntity(j => j.ToTable("MissionEquipment")); // Many-to-many relationship table

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update AssignedMissions for Accounts and Equipments when a mission is created or modified
            var addedEntities = ChangeTracker.Entries<Mission>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);

            foreach (var mission in addedEntities)
            {
                // Update AssignedAccounts in Accounts and Missions in Accounts
                foreach (var account in mission.AssignedAccounts)
                {
                    account.Missions.Add(mission);  // Adding the mission to the account's Missions list
                }

                // Update AssignedEquipment in Equipments and Missions in Equipments
                foreach (var equipment in mission.AssignedEquipment)
                {
                    equipment.Missions.Add(mission);  // Adding the mission to the equipment's Missions list
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
