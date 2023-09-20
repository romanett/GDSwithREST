using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Opc.Ua.Configuration;
using Opc.Ua;
using Microsoft.AspNetCore.Hosting.Server;
using MinAPI.Data;
using MinAPI.Services.GdsBackgroundService;
using Microsoft.Extensions.DependencyInjection;
using GDSwithREST.Services.GdsBackgroundService.Databases;
using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;


#region webApplicationBuilder
var builder = WebApplication.CreateBuilder(args);
//Inject dependencies for the GDS
builder.Services.AddSingleton<IApplicationsDatabase, ApplicationDb>();
builder.Services.AddSingleton<ICertificateGroup, CertificateGroupDb>();
builder.Services.AddSingleton<ICertificateRequest, CertificateRequestDb>();
builder.Services.AddSingleton<IGdsService, GdsService>();
//Run GDS as hosted service in the background
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

app.MapGet("/", (IGdsService gds) => gds.GetEndpointURLs());
app.MapControllers();
app.Run();

