using GDSwithREST.Domain.Services;
using GDSwithRest.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDSwithREST.Controllers;
using GDSwithREST.Domain.ApiModels;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace GDSwithRest.Tests.ApplicationsControllerTests
{
    public class RegisterApplicationTests
    {
        private readonly FakeApplicationRepository _applicationRepository;
        private readonly FakeServerEndpointRepository _serverEndpointRepository;
        private readonly FakeApplicationNameRepository _applicationNameRepository;
        private readonly ApplicationService _applicationService;

        public RegisterApplicationTests()
        {
            _applicationRepository = new FakeApplicationRepository();
            _serverEndpointRepository = new FakeServerEndpointRepository();
            _applicationNameRepository = new FakeApplicationNameRepository();
            var serviceScopeFactory = DIService.GetServiceScopeFactory(_applicationRepository, _serverEndpointRepository, _applicationNameRepository);
            _applicationService = new ApplicationService(serviceScopeFactory);
        }


        [Fact]
        public async Task RegisterNewApplicationSuccessful()
        {
            //Arrange
            var id = Guid.NewGuid();
            var application = new ApplicationApiModel(id, "opc.tpc://localhost", "MyGoodApplication", 1, "https://localhost.com");
            ApplicationsController controller = new ApplicationsController(_applicationRepository, _applicationService);

            //Act
            var response = await controller.RegisterApplication(application);

            //Assert
            Assert.IsAssignableFrom<CreatedAtActionResult>(response.Result);
            var result = response.Result as CreatedAtActionResult;
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ApplicationApiModel>(result.Value);
            var retApplication = result.Value as ApplicationApiModel;
            Assert.NotNull(retApplication);
            Assert.Equal(new ApplicationApiModel(_applicationRepository.Applications.Single(y => y.ApplicationId == retApplication.ApplicationId)), retApplication);
            Assert.NotEqual(id, retApplication.ApplicationId);
        }

        [Fact]
        public async Task RegisterExistingApplicationSuccessful()
        {
            //Arrange
            var application = new ApplicationApiModel(_applicationRepository.Applications.First());
            var id = application.ApplicationId;
            ApplicationsController controller = new ApplicationsController(_applicationRepository, _applicationService);

            //Act
            var response = await controller.RegisterApplication(application);

            //Assert
            Assert.IsAssignableFrom<CreatedAtActionResult>(response.Result);
            var result = response.Result as CreatedAtActionResult;
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ApplicationApiModel>(result.Value);
            var retApplication = result.Value as ApplicationApiModel;
            Assert.NotNull(retApplication);
            Assert.Equal(new ApplicationApiModel(_applicationRepository.Applications.Single(y => y.ApplicationId == retApplication.ApplicationId)), application);
            Assert.Equal(id, retApplication.ApplicationId);
        }

        [Theory]
        [MemberData(nameof(InvalidApplications))]
        public async Task RegisterIncompleteApplicationFails(ApplicationApiModel application)
        {
            //Arrange
            var id = Guid.NewGuid();
            ApplicationsController controller = new ApplicationsController(_applicationRepository, _applicationService);
            
            //Act + Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(async () => await controller.RegisterApplication(application));
        }

        public static IEnumerable<object[]> InvalidApplications =>
        new List<object[]>
        {
            new object[] { null!},
            new object[] { new ApplicationApiModel(Guid.NewGuid(), null!, "MyBadApplication", 1, "https://localhost.com") },
            new object[] { new ApplicationApiModel(Guid.NewGuid(), "opc.tpc://localhost", null!, 1, "https://localhost.com") },
            new object[] { new ApplicationApiModel(Guid.NewGuid(), "opc.tpc://localhost", "MyBadApplication3", 100, "https://localhost.com") },
            new object[] { new ApplicationApiModel(Guid.NewGuid(), "opc.tpc://localhost", "MyBadApplication4", 1, null!) },
        };
    }
}
