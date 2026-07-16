using System;
using System.Collections.Generic;
using System.Linq;
using Task1.DoNotChange;

namespace Task1;

public static class LinqTask
{
    // Customers whose total order sum exceeds the limit
    public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit) =>
        customers.Where(c => c.Orders.Sum(o => o.Total) > limit);

    // For each customer, suppliers in the same city and country (without grouping)
    public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
        IEnumerable<Customer> customers,
        IEnumerable<Supplier> suppliers
    ) =>
        customers.Select(c => (c, suppliers.Where(s =>
            string.Equals(s.City, c.City, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Country, c.Country, StringComparison.OrdinalIgnoreCase))));

    // For each customer, suppliers in the same city and country (using group-join)
    public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
        IEnumerable<Customer> customers,
        IEnumerable<Supplier> suppliers
    ) =>
        customers.GroupJoin(
            suppliers,
            c => (c.Country.ToLower(), c.City.ToLower()),
            s => (s.Country.ToLower(), s.City.ToLower()),
            (c, sups) => (c, sups));

    // Customers that have at least one order exceeding the limit
    public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit) =>
        customers.Where(c => c.Orders.Any(o => o.Total > limit));

    // Customers with orders, paired with their first order date
    public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
        IEnumerable<Customer> customers
    ) =>
        customers
            .Where(c => c.Orders.Length != 0)
            .Select(c => (customer: c, dateOfEntry: c.Orders.Min(o => o.OrderDate)));

    // Same as Linq4 ordered by year, month, total turnover (desc), then customer name
    public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
        IEnumerable<Customer> customers
    ) =>
        customers
            .Where(c => c.Orders.Length != 0)
            .Select(c => (customer: c, dateOfEntry: c.Orders.Min(o => o.OrderDate)))
            .OrderBy(t => t.dateOfEntry.Year)
            .ThenBy(t => t.dateOfEntry.Month)
            .ThenByDescending(t => t.customer.Orders.Sum(o => o.Total))
            .ThenBy(t => t.customer.CompanyName);

    // Customers with a non-digit postal code, undefined region, or no parentheses in phone
    public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers) =>
        customers.Where(c =>
            !c.PostalCode.All(char.IsDigit) ||
            string.IsNullOrEmpty(c.Region) ||
            !c.Phone.Contains('('));

    // Products grouped by category, then by units in stock (desc), prices sorted ascending
    public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products) =>
        products
            .GroupBy(p => p.Category)
            .Select(cg => new Linq7CategoryGroup
            {
                Category = cg.Key,
                UnitsInStockGroup = cg
                    .GroupBy(p => p.UnitsInStock)
                    .OrderByDescending(sg => sg.Key)
                    .Select(sg => new Linq7UnitsInStockGroup
                    {
                        UnitsInStock = sg.Key,
                        Prices = sg.OrderBy(p => p.UnitPrice).Select(p => p.UnitPrice)
                    })
            });

    // Products grouped into cheap/average/expensive price bands
    public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
        IEnumerable<Product> products,
        decimal cheap,
        decimal middle,
        decimal expensive
    ) =>
        [
            (cheap,      products.Where(p => p.UnitPrice <= cheap)),
            (middle,     products.Where(p => p.UnitPrice > cheap && p.UnitPrice <= middle)),
            (expensive,  products.Where(p => p.UnitPrice > middle && p.UnitPrice <= expensive))
        ];

    // Average income (sum of orders) and average intensity (order count) per city
    public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
        IEnumerable<Customer> customers
    ) =>
        customers
            .GroupBy(c => c.City)
            .Select(g => (
                g.Key,
                (int)Math.Round(g.Average(c => c.Orders.Sum(static o => o.Total))),
                (int)Math.Round(g.Average(c => (double)c.Orders.Length))
            ));

    // Unique supplier countries sorted by length then alphabetically, concatenated
    public static string Linq10(IEnumerable<Supplier> suppliers) =>
        string.Concat(
            suppliers
                .Select(s => s.Country)
                .Distinct()
                .OrderBy(c => c.Length)
                .ThenBy(c => c));
}