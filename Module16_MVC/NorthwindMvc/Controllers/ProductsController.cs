using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    public async Task<IActionResult> Create()
    {
        ProductEditViewModel model = await CreateEditViewModelAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductEditViewModel model)
    {
        await PopulateLookupsAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await repository.CreateProductAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        ProductEditViewModel? model = await repository.GetProductForEditAsync(id);

        if (model is null)
        {
            return NotFound();
        }

        await PopulateLookupsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductEditViewModel model)
    {
        await PopulateLookupsAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool updated = await repository.UpdateProductAsync(model);
        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<ProductEditViewModel> CreateEditViewModelAsync()
    {
        ProductEditViewModel model = new();
        await PopulateLookupsAsync(model);
        return model;
    }

    private async Task PopulateLookupsAsync(ProductEditViewModel model)
    {
        IReadOnlyList<CategoryListItem> categories = await repository.GetCategoriesAsync();
        IReadOnlyList<SupplierListItem> suppliers = await repository.GetSuppliersAsync();

        model.Categories = categories.Select(category => new SelectListItem
        {
            Value = category.CategoryID.ToString(),
            Text = category.CategoryName,
            Selected = category.CategoryID == model.CategoryID
        }).ToList();

        model.Suppliers = suppliers.Select(supplier => new SelectListItem
        {
            Value = supplier.SupplierID.ToString(),
            Text = supplier.CompanyName,
            Selected = supplier.SupplierID == model.SupplierID
        }).ToList();
    }
}