using System.Collections.ObjectModel;

namespace CatalogAPI.Models;
public class Category
{
  public Category()
  {
    Products = new Collection<Product>(); // Initialize the products collection in the constructor
  }

  public int Id { get; set; }
  public string? Name { get; set; }
  public string? ImageUrl { get; set; }

  public ICollection<Product>? Products { get; set; } // Navigation property, Category will contain a collection of products
}
