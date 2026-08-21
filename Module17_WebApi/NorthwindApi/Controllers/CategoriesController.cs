using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Contracts;
using NorthwindApi.Models;
using NorthwindApi.Services.Categories;
using NorthwindApi.Services.Common;

namespace NorthwindApi.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(ICategoryService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<Category>> GetAll() => Ok(service.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<Category> GetById(int id) => this.ToGetActionResult(service.GetById(id));

    [HttpPost]
    public ActionResult<Category> Create([FromBody] CategoryUpsertRequest request)
    {
        ServiceResult<Category> result = service.Create(request);
        return this.ToCreatedActionResult(result, nameof(GetById), new { id = result.Value!.CategoryId });
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CategoryUpsertRequest request)
        => this.ToMutationActionResult(service.Update(id, request));

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) => this.ToMutationActionResult(service.Delete(id));
}
