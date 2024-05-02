using CatalogAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.Context;
// Essa classe realiza a comunicação entre as entidades e o banco de dados
public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Category>? Categories { get; set; }
    public DbSet<Product>? Products { get; set; }
}