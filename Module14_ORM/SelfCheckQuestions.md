Questions for the self-check:

1. What is ORM?
	ORM is a way to map database tables and rows to objects and classes in code, so you can work with data through an object-oriented API instead of writing SQL for everything.
2. What is the difference between micro and fully featured ORM?
	A micro ORM is lightweight and focused on fast, direct data access with minimal abstraction. A fully featured ORM adds change tracking, relationships, migrations, and richer modeling features.
3. When will you choose Dapper and when EF Core?
	Choose Dapper when you want maximum control, simple queries, and low overhead. Choose EF Core when you want a richer domain model, change tracking, and productivity features like migrations and LINQ-based querying.
4. What is the deference between Code, Database and Model first approaches?
	Code First starts from C# classes and generates the database from them. Database First starts from an existing database and generates code from it. Model First starts from a visual model and generates both the code and database from that model.
5. What is the purpose of ‘Unit of Work’ pattern?
	Unit of Work coordinates a set of related operations and commits them as one transaction, so changes stay consistent and can be saved or rolled back together.
