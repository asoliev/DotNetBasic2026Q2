Questions for the self-check:

 1. What are the four principles of object-oriented programming? Can you explain each in your own words?
	- Encapsulation: hide internal state and expose only safe operations (for example through properties/methods).
	- Abstraction: show what an object does, hide how it does it.
	- Inheritance: create a new type based on an existing type to reuse and extend behavior.
	- Polymorphism: one interface, multiple implementations; same call can behave differently depending on object type.

 2. What is multiple inheritance? How are we able to achieve it in .NET?
	- Multiple inheritance means inheriting from more than one base class.
	- In C#, classes cannot inherit from multiple classes.
	- In .NET we achieve similar flexibility by implementing multiple interfaces, and by composition (combining objects).

 3. What do S.O.L.I.D. principles stand for? Can you decipher each letter?
	- S: Single Responsibility Principle (one class, one reason to change).
	- O: Open/Closed Principle (open for extension, closed for modification).
	- L: Liskov Substitution Principle (derived types must be safely substitutable for base types).
	- I: Interface Segregation Principle (many small focused interfaces are better than one fat interface).
	- D: Dependency Inversion Principle (depend on abstractions, not concrete implementations).

 4. What other principles do you know, do they conflict or complement each other?
	- DRY (Don’t Repeat Yourself), KISS (Keep It Simple), YAGNI (You Aren’t Gonna Need It), Separation of Concerns, Law of Demeter.
	- They mostly complement each other and SOLID.
	- Potential tension exists in practice, for example between DRY and readability, or OCP and YAGNI; balance depends on context.
