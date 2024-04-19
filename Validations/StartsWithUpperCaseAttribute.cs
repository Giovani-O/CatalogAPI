using System.ComponentModel.DataAnnotations;

namespace CatalogAPI.Validations;
class StartsWithUpperCaseAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(
    object? value,
    ValidationContext validationContext
  )
  {
    if (string.IsNullOrEmpty(value?.ToString() ?? ""))
    {
      return ValidationResult.Success;
    }

    var firstLetter = value.ToString()[0].ToString();
    if (firstLetter != firstLetter.ToUpper())
    {
      return new ValidationResult("A primeira letra deve ser maiúscula");
    }

    return ValidationResult.Success;
  }
}