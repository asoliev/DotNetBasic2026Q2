# Questions for the self-check

1. What is API? What is WebAPI?
  API (Application Programming Interface) is a contract that lets one piece of software use the features or data of another piece of software. It can exist between any kinds of components, not only over the web. A Web API is a specific kind of API that is exposed through HTTP, so it can be consumed by browsers, mobile apps, desktop apps, or other services.
  In short: API is the broader term, and Web API is a web-based API.

2. What is the difference between ASP.NET Core MVC and ASP.NET Core Web API?
  ASP.NET Core MVC is mainly used to build web applications that return HTML views for users. ASP.NET Core Web API is mainly used to build HTTP services that return data, typically JSON or XML, for other applications to consume.

3. What does RESTful mean? Are there any constraints?
  REST is an architectural style for designing networked systems. RESTful means an API follows that style closely enough to use REST principles. So REST is the idea, and RESTful describes an implementation that follows the idea.
  Common REST constraints include client-server separation, stateless communication, cacheability, a uniform interface, layered system, and optional code-on-demand.

4. What is considered as a resource in REST API?
  A resource is any identifiable thing the API exposes through a URI. It can be a domain object such as a user, order, product, file, collection, or even a computed result.

5. For what tasks PATCH method could be used in a RESTful world?
  PATCH is used for partial updates of a resource when you want to change only some fields instead of replacing the whole resource. It is useful for edits like updating a user’s email, changing an order status, or toggling a single property.
