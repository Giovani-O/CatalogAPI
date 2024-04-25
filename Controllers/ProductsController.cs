using CatalogAPI.Filters;
using CatalogAPI.Models;
using CatalogAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<IEnumerable<Product>> Get()
    {
        var products = _repository.GetProducts().ToList();

        if (products is null) return NotFound();

        return Ok(products);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<Product> Get(int id)
    {
        var product = _repository.GetProduct(id);

        if (product is null) return NotFound("Produto não encontrado");

        return Ok(product);
    }

    [HttpPost]
    //[ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Post(Product product)
    {
        if (product is null) return BadRequest("");

        var newProduct = _repository.Create(product);

        return new CreatedAtRouteResult(
            "GetProduct", new { id = newProduct.Id }, newProduct);

    }

    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Put(int id, Product product)
    {
        if (product is null)
            throw new InvalidOperationException("Produto é null");

        bool updatedProduct = _repository.Update(product);

        if (updatedProduct)
        {
            return Ok(product);
        }
        else
        {
            return StatusCode(500, $"Falha ao atualizar o produto {id}");
        }

    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Delete(int id)
    {
        if (id <= 0) return BadRequest("");

        bool deletedProduct = _repository.Delete(id);

        if (deletedProduct)
        {
            return Ok($"O produto {id} foi excluído");
        }
        else
        {
            return StatusCode(500, $"Falha ao excluir o produto {id}");
        }
    }
}