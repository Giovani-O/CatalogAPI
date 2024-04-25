using CatalogAPI.Filters;
using CatalogAPI.Models;
using CatalogAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _repository;
    //private readonly IConfiguration _configuration;
    private readonly ILogger? _logger;
    public CategoriesController(
         ICategoryRepository repository,
         //IConfiguration configuration, 
         ILogger<CategoriesController> logger
    )
    {
        _repository = repository;
        //_configuration = configuration;
        _logger = logger;
    }

    //[HttpGet("ReadConfigFile")]
    //[ServiceFilter(typeof(ApiLoggingFilter))]
    //  public string GetValues()
    //{
    //  var key1 = _configuration["key1"];
    //  var key2 = _configuration["key2"];

    //  var section1 = _configuration["section1:key2"];

    //  return $"Key 1: {key1} Key 2: {key2} Section: {section1}";
    //}

    //[HttpGet("products")]
    //[ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    //  public ActionResult<IEnumerable<Category>> GetCategoriesAndProducts()
    //{
    //  return _context.Categories.Include(p => p.Products).ToList();
    //}

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<IEnumerable<Category>> Get()
    {
        // Queries are usually tracked in the context, this can disrupt performance
        // To prevent that disruption, we can use AsNoTracking() on read only queries.
        var categories = _repository.GetCategories();
        return Ok(categories);
    }

    [HttpGet("{id:int}", Name = "GetCategory")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<Category> Get(int id)
    {
        var category = _repository.GetCategory(id);

        if (category is null)
        {
            _logger?.LogWarning($"Dados inválidos");
            return BadRequest("Dados inválidos");
        }

        if (category.Id == 0) return NotFound("Categoria não encontrada");

        return Ok(category);
    }

    [HttpPost]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Post(Category category)
    {
        if (category is null)
        {
            _logger?.LogWarning($"Dados inválidos");
            return BadRequest("Dados inválidos");
        }

        var createdCategory = _repository.Create(category);

        // Returns the newly created category and the HTTP Code 201
        return new CreatedAtRouteResult(
            "GetCategory", new { id = createdCategory.Id }, category
        );
    }

    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Put(int id, Category category)
    {
        if (category is null || id != category.Id)
        {
            _logger?.LogWarning($"Dados inválidos");
            return BadRequest("Dados inválidos");
        }

        _repository.Update(category);
        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult Delete(int id)
    {
        var category = _repository.GetCategory(id);

        if (category == null)
        {
            _logger?.LogWarning($"Dados inválidos");
            return BadRequest("Dados inválidos");
        }

        var deletedCategory = _repository.Delete(id);
        return Ok(deletedCategory);
    }
}