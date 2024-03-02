using Microsoft.EntityFrameworkCore;
using GDSwithREST.Domain.Services;
using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;
using GDSwithREST.Infrastructure;
using GDSwithREST.Domain.Repositories;
using GDSwithREST.Infrastructure.Repositories;
using GDSwithREST;


#region webApplicationBuilder
var builder = WebApplication.CreateBuilder(args);
// Inject Infrastructure dependencies
builder.Services.AddDbContext<GdsDbContext>(
    options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IApplicationNameRepository, ApplicationNameRepository>();
builder.Services.AddScoped<ICertificateRequestRepository, CertificateRequestRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IPersistencyRepository, PersistencyRepository>();
builder.Services.AddScoped<IServerEndpointRepository, ServerEndpointRepository>();


//Inject dependencies for the GDS
builder.Services.AddSingleton<IApplicationsDatabase, ApplicationService>();
builder.Services.AddSingleton<ICertificateGroupService, CertificateGroupService>();
builder.Services.AddSingleton<ICertificateRequest, CertificateRequestService>();
builder.Services.AddSingleton<IGdsService, GdsService>();
//Run GDS as hosted service in the background
builder.Services.AddHostedService<GdsBackgroundService>();
builder.Services.AddControllers();

builder.Services.AddHealthChecks();

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
app.UseSwaggerUi();

//Map Endpoints
app.MapControllers();

app.MapHealthChecks("/hc");

//GET: /
app.MapGet("/", (IGdsService gds) => gds.GetEndpointURLs());
//start WebApplication
app.Run();

