using System.ComponentModel.DataAnnotations;

namespace CatalogAPI.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(80)]
        public string? Name { get; set; }

        [Required]
        [StringLength(300)]
        public string? ImageUrl { get; set; }
    }
}
