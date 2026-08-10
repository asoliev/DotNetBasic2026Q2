using Microsoft.AspNetCore.Mvc;
using NorthwindMvc.Data;
using NorthwindMvc.Models;

namespace NorthwindMvc.Controllers;

public class ProductsController(NorthwindRepository repository) : Controller
{

    public async Task<IActionResult> Index()
    {
        IReadOnlyList<ProductListItem> products = await repository.GetProductsAsync();
        return View(products);
    }
}