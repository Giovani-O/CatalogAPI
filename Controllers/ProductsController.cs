using CatalogAPI.Filters;
using CatalogAPI.Models;
using CatalogAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    //private readonly IRepository<Product> _repository;
    private readonly IProductRepository _productRepository;
    public ProductsController(/*IRepository<Product> repository,*/ IProductRepository productRepository)
    {
        //_productRepository = repository;
        _productRepository = productRepository;
    }

    [HttpGet("products/{id}")]
    public ActionResult<IEnumerable<Product>> GetProductsByCategory(int id)
    {
        var products = _productRepository.GetProductsByCategory(id);

        if (products is null)
            return NotFound();

        return Ok(products);
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<IEnumerable<Product>> Get()
    {
        var products = _productRepository.GetAll();

        if (products is null) return NotFound();

        return Ok(products);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<Product> Get(int id)
    {
        var product = _productRepository.Get(p => p.Id == id);

        if (product is null) return NotFound("Produto não encontrado");

        return Ok(product);
    }

    [HttpPost]
    //[ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Post(Product product)
    {
        if (product is null) return BadRequest("");

        var newProduct = _productRepository.Create(product);

        return new CreatedAtRouteResult(
            "GetProduct", new { id = newProduct.Id }, newProduct);

    }

    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Put(int id, Product product)
    {
        if (product is null)
            throw new InvalidOperationException("Produto é null");

        var updatedProduct = _productRepository.Update(product);

        return Ok(updatedProduct);
    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Delete(int id)
    {
        if (id <= 0) return BadRequest("");

        var product = _productRepository.Get(p => p.Id == id);

        if (product is null)
        {
            return NotFound("Produto não encontrado");
        }

        var deletedProduct = _productRepository.Delete(product);
        return Ok(deletedProduct);
    }
}