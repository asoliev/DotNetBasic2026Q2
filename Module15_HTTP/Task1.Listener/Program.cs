using System.Net;
using System.Text;

const string prefix = "http://localhost:8888/";
const string myName = "Abdukodir";

using HttpListener listener = new();
listener.Prefixes.Add(prefix);
listener.Start();

Console.WriteLine($"Listening on {prefix}");
Console.WriteLine("Type 'exit' and press Enter to stop the listener.");

Task exitTask = Task.Run(() =>
{
    while (true)
    {
        string? command = Console.ReadLine();
        if (string.Equals(command, "exit", StringComparison.OrdinalIgnoreCase))
        {
            listener.Stop();
            break;
        }
    }
});

try
{
    while (listener.IsListening)
    {
        HttpListenerContext context;

        try
        {
            context = await listener.GetContextAsync();
        }
        catch (HttpListenerException)
        {
            break;
        }
        catch (ObjectDisposedException)
        {
            break;
        }

        string resourcePath = context.Request.Url?.AbsolutePath.Trim('/') ?? string.Empty;
        Console.WriteLine($"Request received: {resourcePath}");

        switch (resourcePath)
        {
            case "MyName":
                await GetMyNameAsync(context, myName);
                break;
            case "MyNameByHeader":
                await GetMyNameByHeaderAsync(context, myName);
                break;
            case "MyNameByCookies":
                await GetMyNameByCookiesAsync(context, myName);
                break;
            case "Information":
                await WriteStatusOnlyResponseAsync(context, HttpStatusCode.SwitchingProtocols);
                break;
            case "Success":
                await WriteResponseAsync(context, HttpStatusCode.OK, "Success");
                break;
            case "Redirection":
                await WriteResponseAsync(context, HttpStatusCode.Found, "Redirection");
                break;
            case "ClientError":
                await WriteResponseAsync(context, HttpStatusCode.BadRequest, "Client error");
                break;
            case "ServerError":
                await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "Server error");
                break;
            default:
                await WriteResponseAsync(context, HttpStatusCode.NotFound, "Resource not found.");
                break;
        }
    }
}
finally
{
    listener.Close();
}

await exitTask;

static Task GetMyNameAsync(HttpListenerContext context, string name) => WriteResponseAsync(context, HttpStatusCode.OK, name);

static Task GetMyNameByHeaderAsync(HttpListenerContext context, string name)
{
    context.Response.StatusCode = (int)HttpStatusCode.OK;
    context.Response.AddHeader("X-MyName", name);
    context.Response.ContentLength64 = 0;
    return context.Response.OutputStream.DisposeAsync().AsTask();
}

static Task GetMyNameByCookiesAsync(HttpListenerContext context, string name)
{
    context.Response.StatusCode = (int)HttpStatusCode.OK;
    context.Response.Cookies.Add(new Cookie("MyName", name) { Path = "/" });
    context.Response.ContentLength64 = 0;
    return context.Response.OutputStream.DisposeAsync().AsTask();
}

static async Task WriteResponseAsync(HttpListenerContext context, HttpStatusCode statusCode, string content)
{
    byte[] buffer = Encoding.UTF8.GetBytes(content);
    context.Response.StatusCode = (int)statusCode;
    context.Response.ContentType = "text/plain; charset=utf-8";
    context.Response.ContentLength64 = buffer.Length;

    await using Stream output = context.Response.OutputStream;
    await output.WriteAsync(buffer);
}

static Task WriteStatusOnlyResponseAsync(HttpListenerContext context, HttpStatusCode statusCode)
{
    context.Response.StatusCode = (int)statusCode;
    context.Response.ContentLength64 = 0;
    return context.Response.OutputStream.DisposeAsync().AsTask();
}
