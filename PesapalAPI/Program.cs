using PesapalApi.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Create a typed Http Client
//fetch the base url
string? baseUrl = builder.Configuration["Integration:BaseUrl"];
ArgumentException.ThrowIfNullOrEmpty(baseUrl);

builder.Services.AddHttpClient<PesapalService>(
    client =>
    {
        client.BaseAddress = new Uri(baseUrl);

    }
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
