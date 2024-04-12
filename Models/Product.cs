namespace CatalogAPI.Models;
class Product
{
  public int Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public decimal Price { get; set; }
  public string? ImageUrl { get; set; }
  public float Stored { get; set; }
  public DateTime RegisterDate { get; set; }
}