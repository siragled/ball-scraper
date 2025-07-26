using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Wishlist.Services;
using Wishlist.Models.DTOs;
using Wishlist.Models;
using Wishlist.Models.Common;

namespace BallScraper.Controllers;

[ApiController]
[Route("products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        var product = await _productService.CreateProductFromUrlAsync(dto.SourceUrl);
        var productDto = MapToDto(product);

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();

        return Ok(MapToDto(product));
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery] ProductsRequest request)
    {
        var result = await _productService.GetProductsAsync(request);
        var productDtos = new PagedResult<ProductDto>
        {
            Items = result.Items.Select(MapToDto),
            TotalCount = result.TotalCount,
            Skip = result.Skip,
            Take = result.Take
        };

        return Ok(productDtos);
    }

    private static ProductDto MapToDto(Product product) => new(
        product.Id,
        product.Name,
        product.Description,
        product.ImageUrl,
        product.Brand,
        product.SourceUrl,
        product.StoreName,
        product.LastPrice,
        product.CreatedAt
    );
}