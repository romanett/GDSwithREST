using GDSwithRest.Tests.Infrastructure;
using GDSwithREST.Controllers;
using GDSwithREST.Domain.ApiModels;
using GDSwithREST.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GDSwithRest.Tests.ApplicationsControllerTests;

public class GetSingleApplicationTests
{
    private readonly FakeApplicationRepository _applicationRepository;
    private readonly ApplicationService _applicationService;

    public GetSingleApplicationTests()
    {
        _applicationRepository = new FakeApplicationRepository();
        var serviceScopeFactory = DIService.GetServiceScopeFactory(_applicationRepository);
        _applicationService = new ApplicationService(serviceScopeFactory);
    }


    [Fact]
    public async Task Returns_CorrectApplication()
    {
        //Arrange
        var id = _applicationRepository.Applications.First().ApplicationId;

        ApplicationsController controller = new ApplicationsController(_applicationRepository, _applicationService);

        //Act
        var response = await controller.GetApplications(id);

        //Assert
            Assert.IsAssignableFrom<OkObjectResult>(response.Result);
            var result = response.Result as OkObjectResult;
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ApplicationApiModel>(result.Value);
            var application = result.Value as ApplicationApiModel;
            Assert.NotNull(application);
            Assert.Equal(new ApplicationApiModel(_applicationRepository.Applications.Single(y => y.ApplicationId == id)), application);
    }
    [Fact]
    public async Task Returns_CorrectApplication2()
    {
        //Arrange
        var id = _applicationRepository.Applications.Last().ApplicationId;

        ApplicationsController controller = new ApplicationsController(_applicationRepository, _applicationService);

        //Act
        var response = await controller.GetApplications(id);

        //Assert
        Assert.IsAssignableFrom<OkObjectResult>(response.Result);
        var result = response.Result as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsAssignableFrom<ApplicationApiModel>(result.Value);
        var application = result.Value as ApplicationApiModel;
        Assert.NotNull(application);
        Assert.Equal(new ApplicationApiModel(_applicationRepository.Applications.Single(y => y.ApplicationId == id)), application);
    }

    [Fact]
    public async Task Returns_NotFound()
    {
        //Arrange
        var id = Guid.NewGuid();
        ApplicationsController controller = new ApplicationsController(_applicationRepository, _applicationService);

        //Act
        var response = await controller.GetApplications(id);

        //Assert
        Assert.IsAssignableFrom<NotFoundResult>(response.Result);
        var result = response.Result as NotFoundResult;
        Assert.NotNull(result);
    }
}