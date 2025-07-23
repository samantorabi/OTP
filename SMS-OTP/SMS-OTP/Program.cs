using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);


var app = builder.Build();


var sampleData = new Random().Next();

var apiTodo = app.MapGroup("api/otp");


apiTodo.MapGet("/generate", () => sampleData);

app.Run();

