using AutoMapper;
using CatalogAPI.DTOs;
using CatalogAPI.Filters;
using CatalogAPI.Models;
using CatalogAPI.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers;

[ApiController]
[Route("[controller]")]
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
    public ActionResult<IEnumerable<ProductDTO>> GetProductsByCategory(int id)
    {
        var products = _unitOfWork.ProductRepository.GetProductsByCategory(id);

        if (products is null)
            return NotFound();

        // Mapping to IEnumerable<ProductDTO>
        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<IEnumerable<ProductDTO>> Get()
    {
        var products = _unitOfWork.ProductRepository.GetAll();

        if (products is null) return NotFound();

        // Mapping to IEnumerable<ProductDTO>
        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<ProductDTO> Get(int id)
    {
        var product = _unitOfWork.ProductRepository.Get(p => p.Id == id);

        if (product is null) return NotFound("Produto não encontrado");

        var productDTO = _mapper.Map<ProductDTO>(product);

        return Ok(productDTO);
    }

    [HttpPost]
    //[ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<ProductDTO> Post(ProductDTO productDto)
    {
        if (productDto is null) return BadRequest("");

        var product = _mapper.Map<Product>(productDto);

        var newProduct = _unitOfWork.ProductRepository.Create(product);
        _unitOfWork.Commit();

        var newProductDTO = _mapper.Map<ProductDTO>(newProduct);

        return new CreatedAtRouteResult(
            "GetProduct", new { id = newProductDTO.Id }, newProductDTO);

    }

    [HttpPatch("{id}/PartialUpdate")]
    public ActionResult<ProductDTOUpdateResponse> Patch(int id, JsonPatchDocument<ProductDTOUpdateRequest> patchProductDTO)
    {
        if (patchProductDTO is null || id <= 0) 
            return BadRequest();

        var product = _unitOfWork.ProductRepository.Get(c => c.Id == id);

        if (product is null)
            return NotFound();

        var productUpdateRequest = _mapper.Map<ProductDTOUpdateRequest>(product);

        patchProductDTO.ApplyTo(productUpdateRequest, ModelState);

        if (!ModelState.IsValid || TryValidateModel(productUpdateRequest))
            return BadRequest(ModelState);

        _mapper.Map(productUpdateRequest, product);
        _unitOfWork.Commit();

        return Ok(_mapper.Map<ProductDTOUpdateResponse>(product));
    }

    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<ProductDTO> Put(int id, ProductDTO productDto)
    {
        if (productDto is null)
            throw new InvalidOperationException("Produto é null");

        var product = _mapper.Map<Product>(productDto);

        var updatedProduct = _unitOfWork.ProductRepository.Update(product);
        _unitOfWork.Commit();

        var newProductDTO = _mapper.Map<ProductDTO>(updatedProduct);

        return Ok(newProductDTO);
    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(ApiLoggingFilter))] // Using the filter
    public ActionResult<ProductDTO> Delete(int id)
    {
        if (id <= 0) return BadRequest("");

        var product = _unitOfWork.ProductRepository.Get(p => p.Id == id);

        if (product is null)
        {
            return NotFound("Produto não encontrado");
        }

        var deletedProduct = _unitOfWork.ProductRepository.Delete(product);
        _unitOfWork.Commit();

        var deletedProductDTO = _mapper.Map<ProductDTO>(product);

        return Ok(deletedProductDTO);
    }
}