using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MinAPI;
using Opc.Ua.Configuration;
using Opc.Ua;
using Opc.Ua.Gds.Server;
using Microsoft.AspNetCore.Hosting.Server;
using MinAPI.Data;


#region webApplicationBuilder
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<MyBackgroundService>();

// Inject database dependency
var conStrBuilder = new SqlConnectionStringBuilder(
    builder.Configuration.GetConnectionString("Default"))
{
    Password = builder.Configuration["DbPassword"]
};
var connection = conStrBuilder.ConnectionString;
builder.Services.AddDbContext<GdsdbContext>(options => options.UseSqlServer(connection));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();
#endregion



app.MapGet("/", () => "Hello World");
app.Run();

