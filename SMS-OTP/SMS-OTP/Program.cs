
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

var sampleData = new Random().Next();

var apiTodo = app.MapGroup("/api/otp");

apiTodo.MapGet("/generate", () => Results.Ok(sampleData));

app.Run();



/*using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

/*builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});#1#

var app = builder.Build();

var sampleData = new Random().Next();

var apiTodo = app.MapGroup("api/otp");
builder.WebHost.UseUrls("http://localhost:5000");


apiTodo.MapGet("/generate", () => Results.Ok(sampleData));

app.Run();

/*
[JsonSerializable(typeof(int))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}#1#*/