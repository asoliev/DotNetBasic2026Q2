using HttpClient client = new();

HttpResponseMessage response = await client.GetAsync("http://localhost:8888/MyName/");
response.EnsureSuccessStatusCode();

string name = await response.Content.ReadAsStringAsync();
Console.WriteLine(name);
