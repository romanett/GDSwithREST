using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MinAPI.Data;
using MinAPI.Services.GdsBackgroundService;
using GDSwithREST.Services.GdsBackgroundService.Databases;
using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;


#region webApplicationBuilder
var builder = WebApplication.CreateBuilder(args);
//Inject dependencies for the GDS
builder.Services.AddSingleton<IApplicationsDatabase, ApplicationDb>();
builder.Services.AddSingleton<ICertificateGroupDb, CertificateGroupDb>();
builder.Services.AddSingleton<ICertificateRequest, CertificateRequestDb>();
builder.Services.AddSingleton<IGdsService, GdsService>();
//Run GDS as hosted service in the background
builder.Services.AddHostedService<GdsBackgroundService>();
builder.Services.AddControllers();
// Inject database dependency
builder.Services.AddDbContext<GdsdbContext>(
    options => options.UseSqlServer(
        new SqlConnectionStringBuilder(
                builder.Configuration.GetConnectionString("Default"))
                {
                Password = builder.Configuration["DbPassword"]
                }.ConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//builder.Services.AddOpenApiDocument();

var app = builder.Build();
#endregion
//activate web API
app.MapGet("/", (IGdsService gds) => gds.GetEndpointURLs());
app.MapControllers();

//app.UseOpenAPI;
//app.UseSwaggerUi3;
app.Run();

