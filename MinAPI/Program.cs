using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var conStrBuilder = new SqlConnectionStringBuilder(
    builder.Configuration.GetConnectionString("Default"))
{
    Password = builder.Configuration["DbPassword"]
};
var connection = conStrBuilder.ConnectionString;
builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(connection));
var app = builder.Build();


app.MapGet("/", () => "Hello World!");

app.Run();
