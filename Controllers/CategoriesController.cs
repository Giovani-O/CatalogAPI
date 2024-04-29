using CatalogAPI.DTOs;
using CatalogAPI.Filters;
using CatalogAPI.Models;
using CatalogAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    //private readonly IConfiguration _configuration;
    private readonly ILogger? _logger;
    public CategoriesController(
         IUnitOfWork unitOfWork,
         //IConfiguration configuration, 
         ILogger<CategoriesController> logger
    )
    {
        _unitOfWork = unitOfWork;
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
    public ActionResult<IEnumerable<CategoryDTO>> Get()
    {
        // Queries are usually tracked in the context, this can disrupt performance
        // To prevent that disruption, we can use AsNoTracking() on read only queries.
        var categories = _unitOfWork.CategoryRepository.GetAll();

        var categoriesDto = new List<CategoryDTO>();
        foreach (var category in categories) 
        {
            var categoryDto = new CategoryDTO()
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
            };

            categoriesDto.Add(categoryDto);
        }

        return Ok(categoriesDto);
    }

    [HttpGet("{id:int}", Name = "GetCategory")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<CategoryDTO> Get(int id)
    {
        var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);

        if (category is null)
        {
            _logger?.LogWarning($"Dados inválidos");
            return BadRequest("Dados inválidos");
        }

        if (category.Id == 0) return NotFound("Categoria não encontrada");

        var categoryDto = new CategoryDTO()
        {
            Id = category.Id,
            Name = category.Name,
            ImageUrl = category.ImageUrl,
        };

        return Ok(categoryDto);
    }

    [HttpPost]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<CategoryDTO> Post(CategoryDTO categoryDto)
    {
        if (categoryDto is null)
        {
            _logger?.LogWarning($"Dados inválidos");
            return BadRequest("Dados inválidos");
        }

        var newCategory = new Category()
        {
            Id = categoryDto.Id,
            Name = categoryDto.Name,
            ImageUrl = categoryDto.ImageUrl,
        };

        var createdCategory = _unitOfWork.CategoryRepository.Create(newCategory);
        _unitOfWork.Commit();

        var newCategoryDto = new CategoryDTO()
        {
            Id = createdCategory.Id,
            Name = createdCategory.Name,
            ImageUrl = createdCategory.ImageUrl,
        };

        // Returns the newly created category and the HTTP Code 201
        return new CreatedAtRouteResult(
            "GetCategory", new { id = newCategoryDto.Id }, newCategoryDto
        );
    }

    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<CategoryDTO> Put(int id, CategoryDTO categoryDto)
    {
        if (categoryDto is null || id != categoryDto.Id)
        {
            _logger?.LogWarning($"Dados inválidos");
            return BadRequest("Dados inválidos");
        }

        var updatedCategory = new Category()
        {
            Id = categoryDto.Id,
            Name = categoryDto.Name,
            ImageUrl = categoryDto.ImageUrl,
        };

        _unitOfWork.CategoryRepository.Update(updatedCategory);
        _unitOfWork.Commit();

        var updatedCategoryDto = new CategoryDTO()
        {
            Id = updatedCategory.Id,
            Name = updatedCategory.Name,
            ImageUrl = updatedCategory.ImageUrl,
        };

        return Ok(updatedCategoryDto);
    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<CategoryDTO> Delete(int id)
    {
        var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);

        if (category == null)
        {
            _logger?.LogWarning($"Dados inválidos");
            return BadRequest("Dados inválidos");
        }

        var deletedCategory = _unitOfWork.CategoryRepository.Delete(category);
        _unitOfWork.Commit();

        var deletedCategoryDto = new CategoryDTO()
        {
            Id = deletedCategory.Id,
            Name = deletedCategory.Name,
            ImageUrl = deletedCategory.ImageUrl,
        };
        return Ok(deletedCategoryDto);
    }
}