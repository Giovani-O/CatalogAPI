using CatalogAPI.Context;
using CatalogAPI.Filters;
using CatalogAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
  private readonly AppDbContext _context;

  public ProductsController(AppDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public async Task<ActionResult<IEnumerable<Product>>> Get()
  {
    
    // Queries are usually tracked in the context, this can disrupt performance
    // To prevent that disruption, we can use AsNoTracking() on read only queries.
    var products = await _context.Products.AsNoTracking().ToListAsync();
    if (products is null) return NotFound(
    "Não é possível encontrar produtos. Tente novamente mais tarde."
    );
    return products;
  }

  [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public async Task<ActionResult<Product>> Get(int id)
  {
    var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    if (product is null) return NotFound("Produto não encontrado");
    return product;
  }

  [HttpPost]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Post(Product product)
  {
    if (product is null) return BadRequest("");

    _context?.Products?.Add(product);
    _context?.SaveChanges();

    // Returns the newly created product and the HTTP Code 201
    return new CreatedAtRouteResult(
    "GetProduct", new { id = product.Id }, product
    );
  }

  [HttpPut("{id:int}")]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Put(int id, Product product)
  {
    if (id != product.Id) return BadRequest("");

    // Marks the entity as modified and save the changes
    _context.Entry(product).State = EntityState.Modified;
    _context.SaveChanges();

    return Ok(product);
  }

  [HttpDelete("{id:int}")]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Delete(int id)
  {
    if (id <= 0) return BadRequest("");

    var product = _context.Products?.AsNoTracking().FirstOrDefault(x => x.Id == id);

    if (product is null) return NotFound("Produto não encontrado");

    _context.Products?.Remove(product);
    _context.SaveChanges();

    return Ok();
  }
}