using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CatalogAPI.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(20, ErrorMessage = "O nome deve ter entre 3 e 20 caracteres", MinimumLength = 3)]
        // [StartsWithUpperCase]
        public string? Name { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres")]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 10)]
        public string? ImageUrl { get; set; }

        public float Stock { get; set; }

        public DateTime RegisterDate { get; set; }

        public int CategoryId { get; set; }
    }
}
