using CatalogAPI.Context;
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
  public ActionResult<IEnumerable<Product>> Get()
  {
    // Queries are usually tracked in the context, this can disrupt performance
    // To prevent that disruption, we can use AsNoTracking() on read only queries.
    var products = _context.Products?.AsNoTracking().ToList();
    if (products is null) return NotFound(
      "Não é possível encontrar produtos. Tente novamente mais tarde."
    );
    return products;
  }

  [HttpGet("{id:int}", Name = "GetProduct")]
  public ActionResult<Product> Get(int id)
  {
    var product = _context.Products?.AsNoTracking().FirstOrDefault(x => x.Id == id);
    if (product is null) return NotFound("Produto não encontrado");
    return product;
  }

  [HttpPost]
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
  public ActionResult Put(int id, Product product)
  {
    if (id != product.Id) return BadRequest("");

    // Marks the entity as modified and save the changes
    _context.Entry(product).State = EntityState.Modified;
    _context.SaveChanges();

    return Ok(product);
  }

  [HttpDelete("{id:int}")]
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