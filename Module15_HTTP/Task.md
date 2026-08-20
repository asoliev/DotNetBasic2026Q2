# HTTP Fundamentals

The goal of this module is to provide an overview of the Hypertext Transfer Protocol (HTTP). How is the exchange of messages organized, what statuses are there, how do process them, how to work with the Request/Response Header, what are Cookies on the client-side (browser), and how do work with them?

Create two console applications:

**Listener:** Create an http connection listener. Use the System.Net.HttpListener class. The listener must constantly listen to the specified address (for example - http://localhost:8888/) until it receives a command to exit.

**Client:** Create a client to work with the http protocol. Use the System.Net.Http.HttpClient class.

## Task 1: URL

Listener: implement a method to parse the request, get the Resource path from the request URL. Resource path will contain the name of the method to be executed. Implement the "GetMyName" method that will be called if “MyName” was passed to the Resource path, the method should return a response in which your name will be passed.

Client: implement a request to the URL - http://localhost:8888/MyName/, get a response, your name will be passed in the response, output it to the console.

## Task 2: HTTP status messages

Client: implement requests to 5 URLs:

- http://localhost:8888/Information/
- http://localhost:8888/Success/
- http://localhost:8888/Redirection/
- http://localhost:8888/ClientError/
- http://localhost:8888/ServerError/

get a response, output the status code to the console.

Listener: implement 5 methods that will form a response for the urls above with different statuses:

- 1xx – Information
- 2xx – Success
- 3xx – Redirection
- 4xx – Client error
- 5xx – Server error

## Task 3: Header

Client: Implement URL access http://localhost:8888/MyNameByHeader/ get the response, get the value of the header "X-MyName" and output it to the console.

Listener: Implement the "GetMyNameByHeader" method, in the response add a new header "X-MyName" in which pass your name.

## Task 4: Cookies

Client: Implement URL access http://localhost:8888/MyNameByCookies/ get a response, get the value "MyName" from cookies and output it to the console.

Listener: Implement the "MyNameByCookies" method, in the response add cookies "MyName" in which pass your name.

## NB! Scoreboard

  0-59 - Average (Task 1-2 were completed)
  60-89 - Good (Task 1-3 were completed)
  90-100 - Excellent (Task 1-4 were completed)
