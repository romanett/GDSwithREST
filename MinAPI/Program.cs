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

var database = new GdsDatabase();
var gdsServer = new GlobalDiscoverySampleServer(
        database,
        database,
        new CertificateGroup()
       );
application.Start(gdsServer).Wait();


var endpoints = application.Server.GetEndpoints().Select(e => e.EndpointUrl).Distinct();
foreach (var endpoint in endpoints)
{
    Console.WriteLine(endpoint);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var conStrBuilder = new SqlConnectionStringBuilder(
    builder.Configuration.GetConnectionString("Default"))
{
    Password = builder.Configuration["DbPassword"]
};
var connection = conStrBuilder.ConnectionString;
builder.Services.AddDbContext<gdsdbContext>(options => options.UseSqlServer(connection));
var app = builder.Build();


app.MapGet("/", () => "Hello World!");

app.Run();
