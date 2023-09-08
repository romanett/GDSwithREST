using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MinAPI;
using Opc.Ua.Configuration;
using Opc.Ua;
using Opc.Ua.Gds.Server;
using Microsoft.AspNetCore.Hosting.Server;
using MinAPI.Data;

ApplicationInstance application = new ApplicationInstance
{
    ApplicationName = "Global Discovery Server",
    ApplicationType = ApplicationType.Server,
    ConfigSectionName = "Opc.Ua.GlobalDiscoveryServer"
};

// load the application configuration.
application.LoadApplicationConfiguration(false).Wait();

// check the application certificate.
application.CheckApplicationInstanceCertificate(false, 0).Wait();


var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/", () => "Hello World!");

using (var scope = app.Services.CreateScope()) { 
    var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();

var database = new GdsDatabase(context);
var gdsServer = new GlobalDiscoverySampleServer(
        database,
        database,
        new CertificateGroup()
       );

application.Start(gdsServer).Wait();

app.Run();


var endpoints = application.Server.GetEndpoints().Select(e => e.EndpointUrl).Distinct();
foreach (var endpoint in endpoints)
{
    Console.WriteLine(endpoint);
}
}
