Questions for the self-check:

 1. What is ADO.NET?
	- ADO.NET is the .NET data-access technology for working with relational and other data sources.
	- It provides APIs for connections, commands, readers, adapters, datasets, parameters, and transactions.

 2. Explain the difference between terms: connected model and disconnected model?
	- Connected model: keeps an open DB connection while reading/writing data (for example SqlDataReader).
	- Disconnected model: loads data into memory (for example DataSet/DataTable), then works without a live connection.

 3. When do we need to use a DataReader and a DataSet?
	- DataReader: when you need fast, forward-only, read-only streaming with minimal memory use.
	- DataSet: when you need in-memory tables, relations, editing, or offline/disconnected processing.

 4. What do we need to use data providers for?
	- Data providers are database-specific implementations (for example SqlClient) used to connect and execute operations.
	- They provide core classes like Connection, Command, DataReader, DataAdapter, and Parameter.

 5. What do we need to use transactions for?
	- Transactions guarantee atomicity and consistency of grouped operations.
	- They allow commit/rollback so either all related changes succeed or none are applied.
