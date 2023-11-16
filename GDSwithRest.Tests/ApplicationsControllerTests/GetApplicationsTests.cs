using GDSwithREST.Controllers;
using GDSwithREST.Domain.ApiModels;
using GDSwithREST.Domain.Entities;
using GDSwithREST.Domain.Repositories;
using GDSwithREST.Domain.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Moq;
using System.Collections.Generic;

namespace GDSwithRest.Tests.ApplicationsControllerTests;

public class GetApplicationsTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository;
    private readonly ApplicationService _applicationService;
    private readonly CertificateGroupService _certificateGroupService;

    public GetApplicationsTests()
    {
        _applicationRepository = new Mock<IApplicationRepository>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(sp => _applicationRepository.Object);
        var serviceScopeFactory = serviceCollection.BuildServiceProvider().GetService<IServiceScopeFactory>();

        if (serviceScopeFactory is null)
            throw new TestCanceledException("Could not intialite DI Service");
        _applicationService = new ApplicationService(serviceScopeFactory);
        _certificateGroupService = new CertificateGroupService();
    }

    [Fact]
    public async Task Returns_AllApplications()
    {
        //Arrange
        var baseApplications = GetApplications();
        _applicationRepository.Setup(x => x.GetAllApplications()).Returns(Task.FromResult(baseApplications));

        ApplicationsController controller = new ApplicationsController(_applicationRepository.Object, _applicationService);

        //Act
        var response = await controller.GetApplications();

        //Assert
        Assert.IsAssignableFrom<OkObjectResult>(response.Result);
        var result = response.Result as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsAssignableFrom<ApplicationApiModel[]>(result.Value);
        var applications = result.Value as ApplicationApiModel[];
        Assert.NotNull(applications);
        Assert.Equal(2, applications.Length);
    }

    [Fact]
    public async Task Returns_NotFound()
    {
        //Arrange
        var baseApplications = GetEmptyApplicationsCollection();
        _applicationRepository.Setup(x => x.GetAllApplications()).Returns(Task.FromResult(baseApplications));
        ApplicationsController controller = new ApplicationsController(_applicationRepository.Object, _applicationService);

        //Act
        var response = await controller.GetApplications();

        //Assert
        Assert.IsAssignableFrom<NotFoundResult>(response.Result);
        var result = response.Result as NotFoundResult;
        Assert.NotNull(result);
    }

    private IEnumerable<Application> GetApplications()
    {
        return new List<Application>()
        {
            new Application() {ApplicationId= new Guid(), ApplicationName = "TestApplication1", ApplicationType= 1 },
            new Application() {ApplicationId= new Guid(), ApplicationName = "TestApplication2", ApplicationType= 0 }
        };
    }
    private IEnumerable<Application> GetEmptyApplicationsCollection()
    {
        return new List<Application>();
    }
}