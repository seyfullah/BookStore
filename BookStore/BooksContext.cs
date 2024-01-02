using Microsoft.EntityFrameworkCore;

namespace BooksApi.Models;

public class BooksContext : DbContext
{
    public BooksContext(DbContextOptions<BooksContext> options)
        : base(options)
    {
    }

    public DbSet<Books> Books { get; set; } = null!;

    public DbSet<Books> BookPrices { get; set; } = null!;

}