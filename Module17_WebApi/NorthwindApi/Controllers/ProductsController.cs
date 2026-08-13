using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Models;
using NorthwindApi.Repositories;

namespace NorthwindApi.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(INorthwindRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<Product>> GetAll()
    {
        return Ok(repository.GetProducts());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Product> GetById(int id)
    {
        Product? product = repository.GetProduct(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> Create([FromBody] ProductUpsertRequest request)
    {
        if (request.CategoryId is not null && repository.GetCategory(request.CategoryId.Value) is null)
        {
            ModelState.AddModelError(nameof(request.CategoryId), "The specified category does not exist.");
            return ValidationProblem(ModelState);
        }

        Product created = repository.CreateProduct(new Product
        {
            ProductName = request.ProductName,
            SupplierId = request.SupplierId,
            CategoryId = request.CategoryId,
            QuantityPerUnit = request.QuantityPerUnit,
            UnitPrice = request.UnitPrice,
            UnitsInStock = request.UnitsInStock,
            UnitsOnOrder = request.UnitsOnOrder,
            ReorderLevel = request.ReorderLevel,
            Discontinued = request.Discontinued
        });

        return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] ProductUpsertRequest request)
    {
        if (repository.GetProduct(id) is null)
        {
            return NotFound();
        }

        if (request.CategoryId is not null && repository.GetCategory(request.CategoryId.Value) is null)
        {
            ModelState.AddModelError(nameof(request.CategoryId), "The specified category does not exist.");
            return ValidationProblem(ModelState);
        }

        bool updated = repository.UpdateProduct(new Product
        {
            ProductId = id,
            ProductName = request.ProductName,
            SupplierId = request.SupplierId,
            CategoryId = request.CategoryId,
            QuantityPerUnit = request.QuantityPerUnit,
            UnitPrice = request.UnitPrice,
            UnitsInStock = request.UnitsInStock,
            UnitsOnOrder = request.UnitsOnOrder,
            ReorderLevel = request.ReorderLevel,
            Discontinued = request.Discontinued
        });

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        bool deleted = repository.DeleteProduct(id);
        return deleted ? NoContent() : NotFound();
    }
}