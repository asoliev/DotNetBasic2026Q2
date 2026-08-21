# MVC Principles

The goal of this module is to overview MVC patterns on the example of the most popular MVC Framework – ASP.NET Core MVC. The practical task includes the implementation of a simple website that allows you to manage data for the Northwind Database.

You should create a website that enables operations with the Northwind Database (script with DB structure and test data can be found here - <https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql>).

## Task 1: Base site

Create a web site with the following pages:

1. The Home page that contains a welcome message and links to other pages
2. The Categories page that shows a list of categories without images
3. The Products page that shows a table with the products
4. The table contains all product fields
5. Instead of the references to Category and Supplier, their names should be shown

Note: All configuration parameters (connection strings, etc.) can remain in the code (hardcoded)

## Task 2: Startup and configuration

Add a configuration feature that supports two parameters:

Database connection string
Maximum (M) number of products shown on the Product page (show only first M products, others – ignored; if M == 0, then show all products)

## Task 3: Edit forms and Server-side validation

Add edit forms (New and Update) for the Products:

Related entities (such as Category) should be presented as a dropdown list
Add server-side validation for edited products (not less than 3 different rules)

## NB! Scoreboard:

0-59 - Average ( Task 1 was completed )
60-89 - Good ( Task 1-2 were completed )
90-100 - Excellent ( Task 1-3 were completed )
