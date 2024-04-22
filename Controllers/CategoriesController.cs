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
  private readonly IConfiguration _configuration;
  public CategoriesController(AppDbContext context, IConfiguration configuration)
  {
    _context = context;
    _configuration = configuration;
  }

  // Constructor
  // public CategoriesController(AppDbContext context)
  // {
  //   _context = context;
  // }

  [HttpGet("ReadConfigFile")]
  public string GetValues()
  {
    var key1 = _configuration["key1"];
    var key2 = _configuration["key2"];

    var section1 = _configuration["section1:key2"];

    return $"Key 1: {key1} Key 2: {key2} Section: {section1}";
  }

  [HttpGet("products")]
  public ActionResult<IEnumerable<Category>> GetCategoriesAndProducts()
  {
    try
    {
      return _context.Categories.Include(p => p.Products).ToList();
    }
    catch (Exception)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema na solicitação.");
    }
  }

  [HttpGet]
  public ActionResult<IEnumerable<Category>> Get()
  {
    try
    {
      // Queries are usually tracked in the context, this can disrupt performance
      // To prevent that disruption, we can use AsNoTracking() on read only queries.
      return _context.Categories.AsNoTracking().ToList();
    }
    catch (Exception)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema na solicitação.");
    }
  }

  [HttpGet("{id:int}", Name = "GetCategory")]
  public ActionResult<Category> Get(int id)
  {
    try
    {
      var category = _context.Categories?.AsNoTracking().FirstOrDefault(x => x.Id == id);
      if (category is null) return NotFound("Categoria não encontrada");
      return Ok(category);
    }
    catch (Exception)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema na solicitação.");
    }
  }

  [HttpPost]
  public ActionResult Post(Category category)
  {
    try
    {
      if (category is null) return BadRequest("");

      _context?.Categories?.Add(category);
      _context?.SaveChanges();

      // Returns the newly created category and the HTTP Code 201
      return new CreatedAtRouteResult(
        "GetCategory", new { id = category.Id }, category
      );
    }
    catch (Exception)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema na solicitação.");
    }
  }

  [HttpPut("{id:int}")]
  public ActionResult Put(int id, Category category)
  {
    try
    {
      if (id != category.Id) return BadRequest("");

      // Marks the entity as modified and save the changes
      _context.Entry(category).State = EntityState.Modified;
      _context.SaveChanges();

      return Ok(category);
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

      var category = _context.Categories?.AsNoTracking().FirstOrDefault(x => x.Id == id);

      if (category is null) return NotFound("Categoria não encontrada");

      _context.Categories?.Remove(category);
      _context.SaveChanges();

      return Ok();
    }
    catch (Exception)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema na solicitação.");
    }
  }
}