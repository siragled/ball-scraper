using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Wishlist.Services;
using Wishlist.Models.DTOs;
using Wishlist.Models;
using Wishlist.Models.Common;

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
        var product = await _productService.GetOrCreateProductFromUrlAsync(dto.SourceUrl);
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
        var pagedResult = await _productService.GetProductsAsync(request);

        var productDtos = new PagedResult<ProductDto>
        {
            Items = pagedResult.Items.Select(MapToDto),
            TotalCount = pagedResult.TotalCount,
            Skip = pagedResult.Skip,
            Take = pagedResult.Take
        };

        return Ok(productDtos);
    }

    [HttpGet("{id:guid}/snapshots")]
    public async Task<ActionResult<PagedResult<ProductSnapshotDto>>> GetProductSnapshots(Guid id, [FromQuery] ProductSnapshotsRequest request)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { Message = "Product not found." });
        }

        var pagedResult = await _productService.GetProductSnapshotsAsync(id, request);

        var snapshotDtos = new PagedResult<ProductSnapshotDto>
        {
            Items = pagedResult.Items.Select(MapToSnapshotDto),
            TotalCount = pagedResult.TotalCount,
            Skip = pagedResult.Skip,
            Take = pagedResult.Take
        };

        return Ok(snapshotDtos);
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
        product.UsualPrice,
        product.IsOnSale,
        product.IsInStock,
        product.CreatedAt
    );

    private static ProductSnapshotDto MapToSnapshotDto(ProductSnapshot snapshot) => new(
        snapshot.Id,
        snapshot.CreatedAt,
        snapshot.Price,
        snapshot.UsualPrice,
        snapshot.IsOnSale,
        snapshot.IsInStock
    );
}