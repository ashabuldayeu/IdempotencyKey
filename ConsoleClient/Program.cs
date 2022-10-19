// See https://aka.ms/new-console-template for more information
using System.Net;
Console.WriteLine("press any button to start");
Console.ReadLine();
HttpClient HttpClient = new HttpClient();
HttpClient.DefaultRequestHeaders.Add("X-Idempotency-Key", Guid.NewGuid().ToString());
HttpClient.BaseAddress = new Uri("https://localhost:7163/WeatherForecast");
List<Task> tasks = new List<Task>(1000);
for (int i = 0; i < 1000; i++)
{
    tasks.Add( Task.Run(async ()=> { int a = i; Console.WriteLine($"{( await HttpClient.PostAsync("", new StringContent(""))).StatusCode},  index :{a}"); }));
    //var res = await HttpClient.PostAsync("", new StringContent(""));
}
 await Task.WhenAll(tasks);
