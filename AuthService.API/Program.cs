using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();  // Added for Swagger UI
// SQL Connection 
builder.Services.AddTransient(x =>
  new MySqlConnection(builder.Configuration.GetConnectionString("Default")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();  // Added for Swagger JSON
    app.UseSwaggerUI();  // Added for Swagger UI
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
