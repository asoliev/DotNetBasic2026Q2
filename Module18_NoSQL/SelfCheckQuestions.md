# Questions for the self-check

1. What's the difference between SQL and NoSQL databases? When a NoSQL database should be used instead of a relational database?

    SQL databases are relational and store data in tables with fixed schemas, strong joins, and ACID transactions. NoSQL databases are non-relational and usually store data in flexible models such as documents, key-value pairs, wide columns, or graphs.

    Use NoSQL when the schema changes often, when you need horizontal scaling, when you work with large or semi-structured data, or when the data model fits documents or other non-tabular structures better than relations.

2. What are the different types of NoSQL databases?

    The main types are key-value databases, document databases, column-family databases, and graph databases.

3. What is a Document in MongoDB? What is a Collection in MongoDB?

    A document is a single record in MongoDB, stored as BSON and made of field-value pairs, similar to a JSON object. A collection is a group of documents, similar to a table in a relational database, but without a strictly fixed schema.

4. What are the data types in MongoDB?

    Common MongoDB data types include string, double, decimal128, int32, int64, boolean, date, timestamp, object, array, objectId, null, binary data, regular expression, and JavaScript/code-related types.

5. Explain advantages of BSON over JSON in MongoDB?

    BSON supports more data types than JSON, such as dates and ObjectId, and is designed for efficient storage and traversal inside MongoDB. It also includes type information, which makes it better suited for database operations than plain text JSON.
