using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Contracts;
using NorthwindApi.Models;
using NorthwindApi.Services.Products;
using NorthwindApi.Services.Common;

namespace NorthwindApi.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<Product>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int? categoryId = null)
    {
        PagedResult<Product> result = service.GetAll(pageNumber, pageSize, categoryId);

        Response.Headers["X-Total-Items"] = result.TotalItems.ToString();
        Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
        Response.Headers["X-Page-Number"] = result.PageNumber.ToString();
        Response.Headers["X-Page-Size"] = result.PageSize.ToString();

        return Ok(result.Items);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Product> GetById(int id) => this.ToGetActionResult(service.GetById(id));

    [HttpPost]
    public ActionResult<Product> Create([FromBody] ProductUpsertRequest request)
    {
        ServiceResult<Product> result = service.Create(request);
        return this.ToCreatedActionResult(result, nameof(GetById), new { id = result.Value!.ProductId });
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] ProductUpsertRequest request)
        => this.ToMutationActionResult(service.Update(id, request));

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) => this.ToMutationActionResult(service.Delete(id));
}