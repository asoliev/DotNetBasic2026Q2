using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Models;
using NorthwindApi.Repositories;

namespace NorthwindApi.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(INorthwindRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<Category>> GetAll()
    {
        return Ok(repository.GetCategories());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Category> GetById(int id)
    {
        Category? category = repository.GetCategory(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public ActionResult<Category> Create([FromBody] CategoryUpsertRequest request)
    {
        Category created = repository.CreateCategory(new Category
        {
            CategoryName = request.CategoryName,
            Description = request.Description
        });

        return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, created);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CategoryUpsertRequest request)
    {
        if (repository.GetCategory(id) is null)
        {
            return NotFound();
        }

        bool updated = repository.UpdateCategory(new Category
        {
            CategoryId = id,
            CategoryName = request.CategoryName,
            Description = request.Description
        });

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        bool deleted = repository.DeleteCategory(id);
        return deleted ? NoContent() : Conflict("Delete the products in the category first.");
    }
}