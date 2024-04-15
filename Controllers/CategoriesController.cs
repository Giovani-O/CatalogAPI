using CatalogAPI.Context;
using CatalogAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
  private readonly AppDbContext _context;

  // Constructor
  public CategoriesController(AppDbContext context)
  {
    _context = context;
  }

  [HttpGet("products")]
  public ActionResult<IEnumerable<Category>> GetCategoriesAndProducts()
  {
    return _context.Categories.Include(p => p.Products).ToList();
  }

  [HttpGet]
  public ActionResult<IEnumerable<Category>> Get()
  {
    // Queries are usually tracked in the context, this can disrupt performance
    // To prevent that disruption, we can use AsNoTracking() on read only queries.
    return _context.Categories.AsNoTracking().ToList();
  }

  [HttpGet("{id:int}", Name = "GetCategory")]
  public ActionResult<Category> Get(int id)
  {
    var category = _context.Categories?.AsNoTracking().FirstOrDefault(x => x.Id == id);
    if (category is null) return NotFound("Categoria não encontrada");
    return Ok(category);
  }

  [HttpPost]
  public ActionResult Post(Category category)
  {
    if (category is null) return BadRequest("");

    _context?.Categories?.Add(category);
    _context?.SaveChanges();

    // Returns the newly created category and the HTTP Code 201
    return new CreatedAtRouteResult(
      "GetCategory", new { id = category.Id }, category
    );
  }

  [HttpPut("{id:int}")]
  public ActionResult Put(int id, Category category)
  {
    if (id != category.Id) return BadRequest("");

    // Marks the entity as modified and save the changes
    _context.Entry(category).State = EntityState.Modified;
    _context.SaveChanges();

    return Ok(category);
  }

  [HttpDelete("{id:int}")]
  public ActionResult Delete(int id)
  {
    if (id <= 0) return BadRequest("");

    var category = _context.Categories?.AsNoTracking().FirstOrDefault(x => x.Id == id);

    if (category is null) return NotFound("Categoria não encontrada");

    _context.Categories?.Remove(category);
    _context.SaveChanges();

    return Ok();
  }
}