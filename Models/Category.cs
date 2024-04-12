using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogAPI.Models;
[Table("Categories")]
public class Category
{
  public Category()
  {
    Products = new Collection<Product>(); // Initialize the products collection in the constructor
  }

  [Key]
  public int Id { get; set; }

  [Required]
  [StringLength(80)]
  public string? Name { get; set; }

  [Required]
  [StringLength(300)]
  public string? ImageUrl { get; set; }

  public ICollection<Product>? Products { get; set; } // Navigation property, Category will contain a collection of products
}
