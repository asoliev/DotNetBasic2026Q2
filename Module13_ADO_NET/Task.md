Database scheme:

ado_net_db.png

Statuses: NotStarted, Loading, InProgress, Arrived, Unloading, Cancelled, Done.

Functional requirements:

Create library and linked test library which cover following requirements:

  1. Ability to perform CRUD operations on product.
  2. Ability to perform CRUD operations on order.
  3. Ability to fetch all products.
  4. Ability to fetch orders (consider filtration by month, status, year or specific product, use stored procedure).
  5. Ability to delete orders in bulk (on the same conditions as in item 4).

NB! Scoreboard:

    0-59 - Average ( DB, Library and test library with the requested functionality was created )
    60-89 - Good ( Connected and disconnected models were used )
    90-100 - Excellent ( Transaction for bulk delete requirement was used )
