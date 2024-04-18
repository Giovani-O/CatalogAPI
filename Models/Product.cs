using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogAPI.Models;
[Table("Products")]
public class Product
{
  [Key]
  public int Id { get; set; }

  [Required(ErrorMessage = "O nome é obrigatório")]
  [StringLength(20, ErrorMessage = "O nome deve ter entre 3 e 20 caracteres", MinimumLength = 3)]
  public string? Name { get; set; }

  [Required]
  [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres")]
  public string? Description { get; set; }

  [Required]
  [Column(TypeName = "decimal(10,2)")]
  [Range(1, 10000, ErrorMessage = "O preço deve estar entre {1} e {2}")]
  public decimal Price { get; set; }

  [Required]
  [StringLength(300, MinimumLength = 10)]
  public string? ImageUrl { get; set; }

  public float Stock { get; set; }

  public DateTime RegisterDate { get; set; }

  public int CategoryId { get; set; } // Defines foreign key

  [JsonIgnore]
  public Category? Category { get; set; } // Navigation property, Product will have a category
}