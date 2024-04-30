using System.ComponentModel.DataAnnotations;

namespace CatalogAPI.DTOs
{
    public class ProductDTOUpdateRequest : IValidatableObject
    {
        [Range(1, 9999, ErrorMessage = "Estoque deve estar entre 1 e 9999")]
        public float Stock { get; set; }

        public DateTime RegisterDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RegisterDate.Date <= DateTime.Now.Date)
            {
                yield return new ValidationResult("A data deve ser maior que a data atual",
                    new[] { nameof(this.RegisterDate) });
            }
        }
    }
}
