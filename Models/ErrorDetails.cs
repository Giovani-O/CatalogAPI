namespace CatalogAPI.Models;

// Classe de definição dos detalhes dos erros
public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public string? Trace { get; set; }
}