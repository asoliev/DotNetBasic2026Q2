using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindMvc.Data;
using NorthwindMvc.Models;

namespace NorthwindMvc.Controllers;

public class ProductsController(NorthwindRepository repository, IConfiguration configuration) : Controller
{

    public async Task<IActionResult> Index(int? rowLimit = null)
    {
        ViewData["FullWidth"] = true;

        int configuredDefaultRowLimit = configuration.GetValue<int?>("Products:Maximum") ?? 0;
        int selectedRowLimit = rowLimit ?? configuredDefaultRowLimit;

        ProductListViewModel model = new()
        {
            RowLimit = selectedRowLimit,
            RowLimitOptions = CreateRowLimitOptions(selectedRowLimit),
            Products = await repository.GetProductsAsync(selectedRowLimit)
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        ProductDetailsViewModel? model = await repository.GetProductDetailsAsync(id);

        if (model is null)
            return NotFound();

        return View(model);
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
            return View(model);

        await repository.CreateProductAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        ProductEditViewModel? model = await repository.GetProductForEditAsync(id);

        if (model is null)
            return NotFound();

        await PopulateLookupsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductEditViewModel model)
    {
        await PopulateLookupsAsync(model);

        if (!ModelState.IsValid)
            return View(model);

        bool updated = await repository.UpdateProductAsync(model);
        if (!updated)
            return NotFound();

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

        model.Categories = [.. categories.Select(category => new SelectListItem
        {
            Value = category.CategoryID.ToString(),
            Text = category.CategoryName,
            Selected = category.CategoryID == model.CategoryID
        })];

        model.Suppliers = [.. suppliers.Select(supplier => new SelectListItem
        {
            Value = supplier.SupplierID.ToString(),
            Text = supplier.CompanyName,
            Selected = supplier.SupplierID == model.SupplierID
        })];
    }

    private IReadOnlyList<SelectListItem> CreateRowLimitOptions(int selectedRowLimit)
    {
        int[] configuredRowLimits = configuration.GetSection("Products:RowLimits").Get<int[]>() ?? [5, 10, 20, 30, 50, 100, 0];

        return [.. configuredRowLimits
            .Distinct()
            .Select(rowLimit => new SelectListItem
            {
                Value = rowLimit.ToString(),
                Text = rowLimit == 0 ? "Unlimited" : rowLimit.ToString(),
                Selected = rowLimit == selectedRowLimit
            })];
    }
}
