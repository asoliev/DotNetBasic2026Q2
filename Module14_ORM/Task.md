# ORM Fundamentals

The goal of this module is to provide an overview of ORM fundamentals by considering two popular ORMs – Dapper and Entity Framework Core. The practical task includes the implementation of the simple DAL (Data Access Layer) using both ORMs.

Database scheme:

  Module14_ORM/db_scheme.png

Statuses: Not Started, Loading, InProgress, Arrived, Unloading, Cancelled, Done.

## Functional requirements

Create a library and a linked test library which cover the following requirements:

 1. Ability to perform CRUD operations on product.
 2. Ability to perform CRUD operations on order.
 3. Ability to fetch all products.
 4. Ability to fetch orders (consider filtration by month, status, year, or specific product, use stored procedure).
 5. Ability to delete orders in bulk (on the same conditions as in item 4).

## NB! Scoreboard

    - 0-69 - Tasks are implemented with some gaps
    - 70-89 - Average ( The task has been implemented by using one of the ORMs )
    - 90-100 - Excellent ( The task was implemented using both ORMs (separate applications, no need to combine both ORM in a single solution) )
