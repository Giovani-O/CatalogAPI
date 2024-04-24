using CatalogAPI.Context;
using CatalogAPI.Filters;
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
  private readonly ILogger? _logger;
  public CategoriesController(
       AppDbContext context, 
       IConfiguration configuration, 
       ILogger<CategoriesController> logger
  )
  {
    _context = context;
    _configuration = configuration;
    _logger = logger;
  }

  [HttpGet("ReadConfigFile")]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public string GetValues()
  {
    var key1 = _configuration["key1"];
    var key2 = _configuration["key2"];

    var section1 = _configuration["section1:key2"];

    return $"Key 1: {key1} Key 2: {key2} Section: {section1}";
  }

  [HttpGet("products")]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<IEnumerable<Category>> GetCategoriesAndProducts()
  {
    return _context.Categories.Include(p => p.Products).ToList();
  }

  [HttpGet]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
  public ActionResult<IEnumerable<Category>> Get()
  {
    // Queries are usually tracked in the context, this can disrupt performance
    // To prevent that disruption, we can use AsNoTracking() on read only queries.
    return _context.Categories.AsNoTracking().ToList();
  }

  [HttpGet("{id:int}", Name = "GetCategory")]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<Category> Get(int id)
  {

    var category = _context.Categories?.AsNoTracking().FirstOrDefault(x => x.Id == id);

    if (category is null) return NotFound("Categoria não encontrada");
    return Ok(category);

  }

  [HttpPost]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
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
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Put(int id, Category category)
  {
    if (id != category.Id) return BadRequest("");

    // Marks the entity as modified and save the changes
    _context.Entry(category).State = EntityState.Modified;
    _context.SaveChanges();

    return Ok(category);
  }

  [HttpDelete("{id:int}")]
  [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
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