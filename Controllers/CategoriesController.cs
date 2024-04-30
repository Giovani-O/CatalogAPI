using CatalogAPI.DTOs;
using CatalogAPI.DTOs.Mappings;
using CatalogAPI.Filters;
using CatalogAPI.Models;
using CatalogAPI.Pagination;
using CatalogAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        // Mapper
        var categoriesDto = categories.ToCategoryDTOList();

        return Ok(categoriesDto);
    }

    [HttpGet("pagination")]
    public ActionResult<IEnumerable<CategoryDTO>> Get([FromQuery] CategoriesParameters categoriesParameters)
    {
        var categories = _unitOfWork.CategoryRepository.GetCategories(categoriesParameters);

        var metadata = new
        {
            categories.TotalCount,
            categories.PageSize,
            categories.CurrentPage,
            categories.TotalPages,
            categories.HasNext,
            categories.HasPrevious,
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriesDTO = categories.ToCategoryDTOList();

        return Ok(categoriesDTO); 
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

        // Mapper
        var categoryDto = category.ToCategoryDTO();

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

        // Mapper
        var newCategory = categoryDto.ToCategory();

        var createdCategory = _unitOfWork.CategoryRepository.Create(newCategory);
        _unitOfWork.Commit();

        // Mapper
        var newCategoryDto = newCategory.ToCategoryDTO();

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

        // Mapper
        var updatedCategory = categoryDto.ToCategory();

        _unitOfWork.CategoryRepository.Update(updatedCategory);
        _unitOfWork.Commit();

        // Mapper
        var updatedCategoryDto = updatedCategory.ToCategoryDTO();

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

        // Mapper
        var deletedCategoryDto =deletedCategory.ToCategoryDTO();

        return Ok(deletedCategoryDto);
    }
}