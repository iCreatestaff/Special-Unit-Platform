using Microsoft.EntityFrameworkCore;
using WeatherApi.Models;

namespace WeatherApi
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<SubEquipment> SubEquipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Equipment-SubEquipment relationship
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.SubEquipments)
                .WithOne(se => se.Equipment)
                .HasForeignKey(se => se.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
