if (args.Length > 0 && args[0].Equals("task2", StringComparison.OrdinalIgnoreCase))
{
	await RunTask2Async();
}
else
{
	await RunTask1Async();
}

static async Task RunTask1Async()
{
	using HttpClient client = new();

	HttpResponseMessage response = await client.GetAsync("http://localhost:8888/MyName/");
	response.EnsureSuccessStatusCode();

	string name = await response.Content.ReadAsStringAsync();
	Console.WriteLine(name);
}

static async Task RunTask2Async()
{
	using HttpClient client = new(new HttpClientHandler { AllowAutoRedirect = false });

	string[] paths =
	{
		"Information",
		"Success",
		"Redirection",
		"ClientError",
		"ServerError"
	};

	foreach (string path in paths)
	{
		using HttpRequestMessage request = new(HttpMethod.Get, $"http://localhost:8888/{path}/");
		using HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
		Console.WriteLine($"{path}: {(int)response.StatusCode} {response.StatusCode}");
	}
}
