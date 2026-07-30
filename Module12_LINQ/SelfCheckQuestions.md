# Questions for the self-check:

1. What is LINQ? What is it used for?

   **LINQ** (Language Integrated Query) is a set of C# language features and .NET library methods that allow querying data sources using a unified, type-safe syntax directly in C#. It is used to filter, project, sort, group, and aggregate data from any source that implements `IEnumerable<T>` or `IQueryable<T>` — including in-memory collections, XML, databases, and more — without switching to a different query language.

2. When are query expressions executed? What is materialization in LINQ? Name query methods which lead to query execution.

   Query expressions use **deferred execution**: the query is not run when it is defined but when it is *iterated*. **Materialization** is the act of forcing that iteration and storing the results in a concrete data structure. Methods that trigger immediate execution (materializing the query) include: `ToList()`, `ToArray()`, `ToDictionary()`, `ToHashSet()`, `Count()`, `Sum()`, `Min()`, `Max()`, `Average()`, `First()`, `FirstOrDefault()`, `Single()`, `SingleOrDefault()`, `Last()`, `LastOrDefault()`, `Any()`, `All()`, and `Aggregate()`.

3. What is query syntax? What is the method syntax?

   **Query syntax** (also called *query expression syntax*) uses SQL-like keywords built into C#: `from`, `where`, `select`, `orderby`, `group by`, `join`, etc.
   ```csharp
   var result = from c in customers
                where c.City == "London"
                select c.CompanyName;
   ```
   **Method syntax** (also called *fluent* or *lambda syntax*) chains extension methods from `System.Linq` with lambda expressions:
   ```csharp
   var result = customers
       .Where(c => c.City == "London")
       .Select(c => c.CompanyName);
   ```
   Both produce identical IL; method syntax is more flexible because every LINQ operator is available as a method.

4. When is query syntax converted to method calls?

   At **compile time**. The C# compiler translates every query expression into the equivalent method call chain before emitting IL. For example, `from x in src where x > 0 select x` becomes `src.Where(x => x > 0).Select(x => x)` during compilation.

5. If query expression syntax does not have some operations, for example Count or Max, how to combine query syntax with method calls?

   Wrap the query expression in parentheses and chain the method call directly:
   ```csharp
   int count = (from c in customers
                where c.Country == "UK"
                select c).Count();

   decimal max = (from o in orders
                  select o.Total).Max();
   ```

6. Which are basic LINQ query operations?

   - **Filtering** – `Where`
   - **Projection** – `Select`, `SelectMany`
   - **Ordering** – `OrderBy`, `OrderByDescending`, `ThenBy`, `ThenByDescending`
   - **Grouping** – `GroupBy`
   - **Joining** – `Join`, `GroupJoin`
   - **Aggregation** – `Count`, `Sum`, `Min`, `Max`, `Average`, `Aggregate`
   - **Quantifiers** – `Any`, `All`, `Contains`
   - **Set operations** – `Distinct`, `Union`, `Intersect`, `Except`
   - **Element access** – `First`, `Single`, `ElementAt`, `Last`
   - **Partitioning** – `Take`, `Skip`, `TakeWhile`, `SkipWhile`

7. How do you transform data with LINQ?

   Use `Select` to project each element into a new shape, and `SelectMany` to flatten nested collections. The output type can be a primitive, a named type, a tuple, or an anonymous type:
   ```csharp
   // project to a tuple
   var result = customers.Select(c => (c.CompanyName, Total: c.Orders.Sum(o => o.Total)));

   // flatten orders across all customers
   var allOrders = customers.SelectMany(c => c.Orders);
   ```

8. What is LINQ to Objects?

   **LINQ to Objects** is the use of LINQ queries against any in-memory `IEnumerable<T>` or `IEnumerable` sequence — arrays, lists, dictionaries, strings, etc. — without an intermediate query provider. All operators are implemented in `System.Linq.Enumerable` and execute as regular .NET code with deferred/streaming iteration.

9. Can you project a query to an anonymous type?

   Yes. Use `select new { ... }` in query syntax or `Select(x => new { ... })` in method syntax. The compiler generates a sealed class with read-only properties inferred from the initializer:
   ```csharp
   var summary = customers.Select(c => new
   {
       c.CompanyName,
       OrderCount = c.Orders.Length,
       Total = c.Orders.Sum(o => o.Total)
   });
   ```
   Anonymous types are useful for intermediate projections within a query but cannot be returned from a method as a named type.
