# Questions for the self-check

1. What is the difference between ASP.NET Core and ASP.NET Core MVC?
  ASP.NET Core is the general web application framework. ASP.NET Core MVC is a web UI framework built on top of ASP.NET Core that uses the MVC pattern for request handling, routing, controllers, views, and model binding.

2. What is MVC? What are the responsibilities  of the models, views, and controllers?
  MVC means Model-View-Controller. Models hold and represent application data and business rules, views render the UI, and controllers handle incoming requests, coordinate work, and select the response.

3. What is middleware in ASP.NET Core MVC? Is it possible to create your own middleware?
  Middleware is a component in the ASP.NET Core request pipeline that can inspect, modify, or short-circuit HTTP requests and responses. Yes, you can create custom middleware by writing a class or delegate and registering it in the pipeline.

4. How to validate models in ASP.NET Core MVC?
  Use data annotation attributes such as [Required], [StringLength], and [Range], or implement custom validation with IValidatableObject or a custom validation attribute. In controllers, check ModelState.IsValid before processing the model.
