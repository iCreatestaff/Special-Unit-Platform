using Microsoft.EntityFrameworkCore;

namespace WeatherApi.Models  
{  
    public class AppDbContext : DbContext  
    {  
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }  

        public DbSet<Equipment> Equipments { get; set; }  
        public DbSet<SubEquipment> SubEquipments { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)  
        {  
            // Configure foreign key relationship  
            modelBuilder.Entity<SubEquipment>()  
                .HasOne(se => se.Equipment) // Each SubEquipment has one Equipment  
                .WithMany(e => e.SubEquipments) // Each Equipment has many SubEquipments  
                .HasForeignKey(se => se.EquipmentId) // Set foreign key  
                .OnDelete(DeleteBehavior.Cascade); // Optional: Defines cascade delete behavior  
        }  
    }

    
}