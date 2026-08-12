using Microsoft.AspNetCore.Mvc;
using NorthwindMvc.Data;
using NorthwindMvc.Models;

namespace NorthwindMvc.Controllers;

public class CategoriesController(NorthwindRepository repository) : Controller
{

    public async Task<IActionResult> Index()
    {
        IReadOnlyList<CategoryListItem> categories = await repository.GetCategoriesAsync();
        return View(categories);
    }

    public async Task<IActionResult> Details(int id)
    {
        CategoryDetailsViewModel? model = await repository.GetCategoryDetailsAsync(id);

        if (model is null)
            return NotFound();

        return View(model);
    }
}
