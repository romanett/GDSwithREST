using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Opc.Ua.Configuration;
using Opc.Ua;
using Opc.Ua.Gds.Server;
using Microsoft.AspNetCore.Hosting.Server;
using MinAPI.Data;
using MinAPI.Services.GdsBackgroundService;
using Microsoft.Extensions.DependencyInjection;


#region webApplicationBuilder
var builder = WebApplication.CreateBuilder(args);
//Inject dependency for accessing the GDS and adding the Hosted Service running the GDS
builder.Services.AddSingleton<IGdsService, GdsService>();
builder.Services.AddHostedService<GdsBackgroundService>();
builder.Services.AddControllers();
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
app.MapGet("/Endpoints", (IGdsService gds) => gds.GetEndpoints());
app.Run();

