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
  public async Task<ActionResult<IEnumerable<Product>>> Get()
  {
    try
    {
      // Queries are usually tracked in the context, this can disrupt performance
      // To prevent that disruption, we can use AsNoTracking() on read only queries.
      var products = await _context.Products.AsNoTracking().ToListAsync();
      if (products is null) return NotFound(
        "Não é possível encontrar produtos. Tente novamente mais tarde."
      );
      return products;
    }
    catch (Exception)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema na solicitação.");
    }
  }

  [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
  public async Task<ActionResult<Product>> Get(int id)
  {
    try
    {
      var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
      if (product is null) return NotFound("Produto não encontrado");
      return product;
    }
    catch (Exception)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema na solicitação.");
    }
  }

  [HttpPost]
  public ActionResult Post(Product product)
  {
    try
    {
      if (product is null) return BadRequest("");

      _context?.Products?.Add(product);
      _context?.SaveChanges();

      // Returns the newly created product and the HTTP Code 201
      return new CreatedAtRouteResult(
        "GetProduct", new { id = product.Id }, product
      );
    }
    catch (Exception)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema na solicitação.");
    }
  }

  [HttpPut("{id:int}")]
  public ActionResult Put(int id, Product product)
  {
    try
    {
      if (id != product.Id) return BadRequest("");

      // Marks the entity as modified and save the changes
      _context.Entry(product).State = EntityState.Modified;
      _context.SaveChanges();

      return Ok(product);
    }
    catch (Exception)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema na solicitação.");
    }
  }

  [HttpDelete("{id:int}")]
  public ActionResult Delete(int id)
  {
    try
    {
      if (id <= 0) return BadRequest("");

      var product = _context.Products?.AsNoTracking().FirstOrDefault(x => x.Id == id);

      if (product is null) return NotFound("Produto não encontrado");

      _context.Products?.Remove(product);
      _context.SaveChanges();

      return Ok();
    }
    catch (Exception)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema na solicitação.");
    }
  }
}