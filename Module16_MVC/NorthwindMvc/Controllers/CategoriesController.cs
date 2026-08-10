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
}