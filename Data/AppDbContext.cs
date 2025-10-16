using Microsoft.EntityFrameworkCore;

namespace Midterm_PROG3340_RDooley.Data;

public class AppDbContext : DbContext
{
    private readonly IWebHostEnvironment? _env;
    
    public AppDbContext(DbContextOptions<AppDbContext> options, IWebHostEnvironment? env = null) : base(options)
    {
        _env = env;
    }
    
    public DbSet<Book> Books { get; set; } = null!;
         
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        bool isDevelopment = _env?.IsDevelopment() ?? true;
        
        if (isDevelopment)
        {
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Dev IT", Author = "Stephen King", 
                    Price = 14.99m, Genre = "Horror", IsAvavilable = true, PublishedYear = 1986 },
                new Book { Id = 2, Title = "Dev Pet Semetary", Author = "Stephen King", 
                    Price = 9.99m, Genre = "Horror", IsAvavilable = true, PublishedYear = 1983 },
                new Book { Id = 3, Title = "Dev Salem's Lot", Author = "Stephen King", 
                    Price = 6.99m, Genre = "Horror", IsAvavilable = false, PublishedYear = 1975 },
                new Book { Id = 4, Title = "Dev Greenlights", Author = "Matthew McConaughey", 
                    Price = 18.99m, Genre = "Memoirs", IsAvavilable = false, PublishedYear = 2020},
                new Book { Id = 5, Title = "Dev On The Road With Bob Dylan", Author = "Larry Sloman", 
                    Price = 7.99m, Genre = "Music", IsAvavilable = true, PublishedYear = 1978},
                new Book { Id = 6, Title = "Dev Road Cases", Author = "Meat", 
                    Price = 17.99m, Genre = "Music", IsAvavilable = true, PublishedYear = 2025},
                new Book { Id = 7, Title = "Dev Harry Potter", Author = "J.K. Rowling", 
                    Price = 11.99m, Genre = "Fiction", IsAvavilable = false, PublishedYear = 1997}
            );    
        }
        else
        {
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Production IT", Author = "Stephen King", 
                    Price = 14.99m, Genre = "Horror", IsAvavilable = true, PublishedYear = 1986 },
                new Book { Id = 2, Title = "Production Pet Semetary", Author = "Stephen King", 
                    Price = 9.99m, Genre = "Horror", IsAvavilable = true, PublishedYear = 1983 },
                new Book { Id = 3, Title = "Production Salem's Lot", Author = "Stephen King", 
                    Price = 6.99m, Genre = "Horror", IsAvavilable = false, PublishedYear = 1975 },
                new Book { Id = 4, Title = "Production Greenlights", Author = "Matthew McConaughey", 
                    Price = 18.99m, Genre = "Memoirs", IsAvavilable = false, PublishedYear = 2020},
                new Book { Id = 5, Title = "Production On The Road With Bob Dylan", Author = "Larry Sloman", 
                    Price = 7.99m, Genre = "Music", IsAvavilable = true, PublishedYear = 1978},
                new Book { Id = 6, Title = "Production Road Cases", Author = "Meat", 
                    Price = 17.99m, Genre = "Music", IsAvavilable = true, PublishedYear = 2025},
                new Book { Id = 7, Title = "Dev Harry Potter", Author = "J.K. Rowling", 
                    Price = 11.99m, Genre = "Fiction", IsAvavilable = false, PublishedYear = 1997}
            ); 
        }
    }
}