Questions for the self-check:

1. What is HTTP?
  HTTP (Hypertext Transfer Protocol) is an application-layer protocol used for transferring web resources between a client and a server. It usually runs over TCP, while HTTP/3 runs over QUIC, which uses UDP underneath.
  Example: when you open a web page in a browser, the browser sends an HTTP request to the server and gets an HTTP response back.

2. What is the header?
  A header is metadata in an HTTP request or response that provides additional information such as content type, language, authorization, caching rules, and cookies. Headers help the client and server understand how to process the message.
  There is no fixed total number of headers, but each request or response usually has practical limits set by the client, server, or proxy, such as maximum header size or maximum number of header fields.
  Example: `Content-Type: application/json` tells the receiver that the body contains JSON data.

3. What are Cookies, what types of cookies are there?
  Cookies are small pieces of data stored by the browser for a website. They are often used for authentication, user preferences, and tracking. Session cookies are stored in the browser for the current session only, while the server may store the real session data and use the cookie as a session ID.
  Other common types include persistent cookies, first-party cookies, third-party cookies, secure cookies, HttpOnly cookies, and SameSite cookies.
  Example: a site may store a session cookie to keep you signed in while you browse, and a persistent cookie to remember your language choice.

4. What is the HTTP message status?
  The HTTP status is the response code that shows the result of a request, such as 200 OK, 301 Moved Permanently, 404 Not Found, or 500 Internal Server Error.

5. What parts does a URL consist of? What can it contain?
  A URL can contain a scheme, host, port, path, query string, and fragment. It may also include user info and other parameters depending on the address. The scheme shows the protocol, the host identifies the server, the path points to a resource, the query string carries parameters, and the fragment points to a section within the page.
  Example: `https://example.com:443/products/details?id=10#reviews` includes the scheme `https`, host `example.com`, port `443`, path `/products/details`, query `id=10`, and fragment `reviews`.
