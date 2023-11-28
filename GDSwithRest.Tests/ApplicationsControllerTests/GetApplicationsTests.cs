using GDSwithRest.Tests.Infrastructure;
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
    private readonly FakeApplicationRepository _applicationRepository;
    private readonly ApplicationService _applicationService;

    public GetApplicationsTests()
    {
        _applicationRepository = new FakeApplicationRepository();
        var serviceScopeFactory= DIService.GetServiceScopeFactory(_applicationRepository);
        _applicationService = new ApplicationService(serviceScopeFactory);
    }

    [Fact]
    public async Task Returns_AllApplications()
    {
        //Arrange
        ApplicationsController controller = new ApplicationsController(_applicationRepository, _applicationService);

        //Act
        var response = await controller.GetApplications();

        //Assert
        Assert.IsAssignableFrom<OkObjectResult>(response.Result);
        var result = response.Result as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsAssignableFrom<ApplicationApiModel[]>(result.Value);
        var applications = result.Value as ApplicationApiModel[];
        Assert.NotNull(applications);
        Assert.Equal(_applicationRepository.Applications.Count, applications.Length);
    }

    [Fact]
    public async Task Returns_NotFound()
    {
        //Arrange
        _applicationRepository.Applications.Clear();
        ApplicationsController controller = new ApplicationsController(_applicationRepository, _applicationService);

        //Act
        var response = await controller.GetApplications();

        //Assert
        Assert.IsAssignableFrom<NotFoundResult>(response.Result);
        var result = response.Result as NotFoundResult;
        Assert.NotNull(result);
    }
}