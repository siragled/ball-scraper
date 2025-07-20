using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Wishlist.Services;
using Wishlist.Models.DTOs;
using Wishlist.Models;

namespace BallScraper.Controllers;

[ApiController]
[Route("products")]
[Authorize]
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
        var product = await _productService.CreateProductAsync(dto);
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

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts(
        [FromQuery] string? q,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        if (take > 100) take = 100;

        var products = await _productService.SearchProductsAsync(q, skip, take);
        var productDtos = products.Select(MapToDto);

        return Ok(productDtos);
    }

    private static ProductDto MapToDto(Product product) => new(
        product.Id,
        product.Name,
        product.Description,
        product.ImageUrl,
        product.Brand,
        product.CreatedAt
    );
}
