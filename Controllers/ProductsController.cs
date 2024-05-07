using AutoMapper;
using CatalogAPI.DTOs;
using CatalogAPI.Filters;
using CatalogAPI.Models;
using CatalogAPI.Pagination;
using CatalogAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;
using Microsoft.AspNetCore.Http;

namespace CatalogAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
//[ApiExplorerSettings(IgnoreApi = true)]
public class ProductsController : ControllerBase
{
    //private readonly IRepository<Product> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductsController(/*IRepository<Product> repository,*/ IUnitOfWork unitOfWork, IMapper mapper)
    {
        //_productRepository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet("products/{id}")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByCategory(int id)
    {
        var products = await _unitOfWork.ProductRepository.GetProductsByCategoryAsync(id);

        if (products is null)
            return NotFound();

        // Mapping to IEnumerable<ProductDTO>
        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    private ActionResult<IEnumerable<ProductDTO>> GetProducts(IPagedList<Product> products)
    {
        var metadata = new
        {
            products.Count,
            products.PageSize,
            products.PageCount,
            products.TotalItemCount,
            products.HasNextPage,
            products.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get([FromQuery] ProductsParameters productsParameters) 
    { 
        var products = await _unitOfWork.ProductRepository.GetProductsAsync(productsParameters);

        return GetProducts(products);
    }

    [HttpGet("filter/price/pagination")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsFilteredByPrice([FromQuery] ProductsPriceFilter productsPriceFilter)
    {
        var products = await _unitOfWork.ProductRepository.GetProductsFilteredByPriceAsync(productsPriceFilter);

        return GetProducts(products);
    }

    // [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    /// <summary>
    /// Exibe todos os produtos
    /// </summary>
    /// <returns>Retorna uma lista de objetos Product</returns>
    [Authorize(Policy = "UserOnly")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
    {
        try
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync();

            if (products is null) return NotFound();

            // Mapping to IEnumerable<ProductDTO>
            var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(productsDTO);
        }
        catch (Exception)
        {
            return BadRequest();
        }
        
    }

    // [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    /// <summary>
    /// Obtém um produto pelo Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>O produto buscado</returns>
    [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDTO>> Get(int? id)
    {
        if (id == null || id <= 0)
            return BadRequest("Id inválido");

        var product = await _unitOfWork.ProductRepository.GetAsync(p => p.Id == id);

        if (product is null) return NotFound("Produto não encontrado");

        var productDTO = _mapper.Map<ProductDTO>(product);

        return Ok(productDTO);
    }

    [HttpPost]
    //[ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public async Task<ActionResult<ProductDTO>> Post(ProductDTO productDto)
    {
        if (productDto is null) return BadRequest("");

        var product = _mapper.Map<Product>(productDto);

        var newProduct = _unitOfWork.ProductRepository.Create(product);
        await _unitOfWork.CommitAsync();

        var newProductDTO = _mapper.Map<ProductDTO>(newProduct);

        return new CreatedAtRouteResult(
            "GetProduct", new { id = newProductDTO.Id }, newProductDTO);

    }

    [HttpPatch("{id}/PartialUpdate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ProductDTOUpdateResponse>> Patch(int id, JsonPatchDocument<ProductDTOUpdateRequest> patchProductDTO)
    {
        if (patchProductDTO is null || id <= 0) 
            return BadRequest();

        var product = await _unitOfWork.ProductRepository.GetAsync(c => c.Id == id);

        if (product is null)
            return NotFound();

        var productUpdateRequest = _mapper.Map<ProductDTOUpdateRequest>(product);

        patchProductDTO.ApplyTo(productUpdateRequest, ModelState);

        if (!ModelState.IsValid || TryValidateModel(productUpdateRequest))
            return BadRequest(ModelState);

        _mapper.Map(productUpdateRequest, product);
        await _unitOfWork.CommitAsync();

        return Ok(_mapper.Map<ProductDTOUpdateResponse>(product));
    }

    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ProductDTO>> Put(int id, ProductDTO productDto)
    {
        if (productDto is null)
            throw new InvalidOperationException("Produto é null");

        var product = _mapper.Map<Product>(productDto);

        var updatedProduct = _unitOfWork.ProductRepository.Update(product);
        await _unitOfWork.CommitAsync();

        var newProductDTO = _mapper.Map<ProductDTO>(updatedProduct);

        return Ok(newProductDTO);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public async Task<ActionResult<ProductDTO>> Delete(int id)
    {
        if (id <= 0) return BadRequest("");

        var product = await _unitOfWork.ProductRepository.GetAsync(p => p.Id == id);

        if (product is null)
        {
            return NotFound("Produto não encontrado");
        }

        var deletedProduct = _unitOfWork.ProductRepository.Delete(product);
        await _unitOfWork.CommitAsync();

        var deletedProductDTO = _mapper.Map<ProductDTO>(product);

        return Ok(deletedProductDTO);
    }
}