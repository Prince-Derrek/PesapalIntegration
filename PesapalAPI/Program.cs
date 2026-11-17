using PesapalAPI.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
