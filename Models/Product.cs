using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogAPI.Models;
[Table("Products")]
public class Product : IValidatableObject
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(20, ErrorMessage = "O nome deve ter entre 3 e 20 caracteres", MinimumLength = 3)]
    // [StartsWithUpperCase]
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(this.Name))
        {
            var firstLetter = this.Name[0].ToString();
            if (firstLetter != firstLetter.ToUpper())
            {
                // yield indica que o método é um iterador
                yield return new ValidationResult(
                  "A primeira letra deve ser maiuscula",
                  new[] { nameof(this.Name) });
            }

            if (this.Stock <= 0)
            {
                yield return new ValidationResult(
                  "O estoque não pode estar vazio",
                  new[] { nameof(this.Name) });
            }
        }
    }
}