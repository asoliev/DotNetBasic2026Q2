Queries:

Select the customers whose total turnover (the sum of all orders) exceeds a certain value.

For each customer make a list of suppliers located in the same country and the same city. Compose queries with and without grouping.

1. Find all customers with the sum of all orders that exceed a certain value.

2. Select the clients, including the date of their first order.

3. Repeat the previous query but order the result by year, month, turnover (descending) and customer name.

4. Select the clients which either have:
   a. non-digit postal code
   b. undefined region
   c. operator code in the phone is not specified (does not contain parentheses)

5. Group the products by category, then by availability in stock with ordering by cost.

6. Group the products by “cheap”, “average” and “expensive” following the rules:
  a. From 0 to cheap inclusive
  b. From cheap exclusive to average inclusive
  c. From average exclusive to expensive inclusive

7. Calculate the average profitability of each city (average amount of orders per customer) and average rate (average number of orders per customer from each city).

8. Build a string of unique supplier country names, sorted first by length and then by country.
