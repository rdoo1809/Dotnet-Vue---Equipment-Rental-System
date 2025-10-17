using Microsoft.EntityFrameworkCore;

namespace Midterm_PROG3340_RDooley.Data;

public class AppDbContext : DbContext
{
    private readonly IWebHostEnvironment? _env;
    
    public AppDbContext(DbContextOptions<AppDbContext> options, IWebHostEnvironment? env = null) : base(options)
    {
        _env = env;
    }
    
    public DbSet<Equipment> Equipment { get; set; } = null!;
    public DbSet<User> User { get; set; } = null!;
         
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Equipment>().HasData(
            new Equipment { Id = 1, Name = "Excavator", Description = "Large hydraulic excavator for heavy lifting", Category = "Heavy Machinery", Condition = "New", RentalPrice = 250.00, IsAvailable = true, CreatedAt = DateTime.Parse("2023-01-15") },
            new Equipment { Id = 2, Name = "Cordless Drill", Description = "18V battery-powered drill for versatile use", Category = "Power Tools", Condition = "Exellent", RentalPrice = 15.00, IsAvailable = true, CreatedAt = DateTime.Parse("2022-07-10") },
            new Equipment { Id = 3, Name = "Pickup Truck", Description = "4x4 truck for transporting equipment", Category = "Vehicles", Condition = "Fair", RentalPrice = 80.00, IsAvailable = false, CreatedAt = DateTime.Parse("2021-03-22") },
            new Equipment { Id = 4, Name = "Safety Helmet", Description = "High-impact protective helmet", Category = "Safety", Condition = "Poor", RentalPrice = 5.00, IsAvailable = true, CreatedAt = DateTime.Parse("2024-05-05") },
            new Equipment { Id = 5, Name = "Theodolite", Description = "Precision surveying instrument for angle measurements", Category = "Surveying", Condition = "Good", RentalPrice = 40.00, IsAvailable = false, CreatedAt = DateTime.Parse("2023-11-12") },
            new Equipment { Id = 6, Name = "Chainsaw", Description = "Gas-powered chainsaw for tree cutting", Category = "Power Tools", Condition = "New", RentalPrice = 25.00, IsAvailable = true, CreatedAt = DateTime.Parse("2022-09-18") },
            new Equipment { Id = 7, Name = "Forklift", Description = "Electric forklift for warehouse operations", Category = "Heavy Machinery", Condition = "Fair", RentalPrice = 120.00, IsAvailable = true, CreatedAt = DateTime.Parse("2023-06-30") }
        );
        
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, UserName = "AdminOne", Password = "password", Role = "amdin" },
            new User { Id = 2, UserName = "UserOne", Password = "password", Role = "user" },
            new User { Id = 3, UserName = "UserTwo", Password = "password", Role = "user" },
            new User { Id = 4, UserName = "UserThree", Password = "password", Role = "user" }
        );
        
    }
}