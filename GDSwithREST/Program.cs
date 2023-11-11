using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using GDSwithREST.Data;
using GDSwithREST.Services.GdsBackgroundService;
using GDSwithREST.Services.GdsBackgroundService.Databases;
using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NSwag;
using NSwag.Generation.Processors.Security;


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
                builder.Configuration.GetConnectionString("Default")));
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}
//Enable OpenApiDocumenation
builder.Services.AddOpenApiDocument(options =>
{ 
    options.Title = "GDSwithREST API";
    options.DocumentName = "GDSwithREST API";
    options.AddSecurity("Bearer",new OpenApiSecurityScheme
    { 
        Type=  OpenApiSecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In= OpenApiSecurityApiKeyLocation.Header,
        Name= "Authorization",
        Description = "Provide your JWT Token"
    });
    options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});

//Add Authentification
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

var app = builder.Build();
#endregion
if (!app.Environment.IsDevelopment())
{
    //enforce https
    app.UseHsts();
}
//automatically redirect to https endpoints
app.UseHttpsRedirection();

//add authentification & authorization
app.UseAuthentication();
app.UseAuthorization();

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

