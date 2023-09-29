using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using GDSwithREST.Data;
using GDSwithREST.Services.GdsBackgroundService;
using GDSwithREST.Services.GdsBackgroundService.Databases;
using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;
using NSwag.Generation.AspNetCore;


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
builder.Services.AddDbContext<GdsDbContext>(
    options => options.UseSqlServer(
        new SqlConnectionStringBuilder(
                builder.Configuration.GetConnectionString("Default"))
                {
                Password = builder.Configuration["DbPassword"]
                }.ConnectionString));
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}
//Enable OpenApiDocumenation
builder.Services.AddOpenApiDocument(options =>
{ 
    options.Title = "GDSwithREST API";
    options.DocumentName = "GDSwithREST API"; 
}) ;

var app = builder.Build();
#endregion
if (!app.Environment.IsDevelopment())
{
    //enforce https
    app.UseHsts();
}
//automatically redirect to https endpoints
app.UseHttpsRedirection();
//activate web API documentation with OpenApi
app.UseOpenApi();

//GET: /swagger
app.UseSwaggerUi3();

//Map Endpoints
app.MapControllers();

//GET: /
app.MapGet("/", (IGdsService gds) => gds.GetEndpointURLs());
//start WebApplication
app.Run();

