using GDSwithREST.Domain.Services;
using GDSwithRest.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDSwithREST.Controllers;
using GDSwithREST.Domain.ApiModels;
using Opc.Ua;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace GDSwithRest.Tests.ApplicationsControllerTests
{
    public class DeleteApplicationTest
    {
        private readonly FakeApplicationRepository _applicationRepository;
        private readonly ApplicationService _applicationService;
        private readonly FakeServerEndpointRepository _serverEndpointRepository;
        private readonly FakeApplicationNameRepository _applicationNameRepository;
        private readonly FakeICertificateRequestRepository _certificateRequestRepository;
        private readonly CertificateGroupService _certificateGroupService;

        public DeleteApplicationTest()
        {
            _applicationRepository = new FakeApplicationRepository();
            _serverEndpointRepository = new FakeServerEndpointRepository();
            _applicationNameRepository = new FakeApplicationNameRepository();
            _certificateRequestRepository = new FakeICertificateRequestRepository();
            var serviceScopeFactory = DIService.GetServiceScopeFactory(_applicationRepository, _serverEndpointRepository, _applicationNameRepository, _certificateRequestRepository);
            _applicationService = new ApplicationService(serviceScopeFactory);
            _certificateGroupService = new CertificateGroupService();
        }

        [Fact]
        public async Task DeleteExistingApplication()
        {
            //Arrange
            var applicationRaw = _applicationRepository.Applications.First();
            var application = new ApplicationApiModel(applicationRaw);
            var id = application.ApplicationId;
            ApplicationsController controller = new ApplicationsController(_applicationRepository, _applicationService);

            //Act
            var response = await controller.DeleteApplications(id, _certificateGroupService);
            //Assert
            Assert.IsAssignableFrom<OkResult>(response);
            var result = response as OkResult;
            Assert.NotNull(result);
            Assert.DoesNotContain(applicationRaw, _applicationRepository.Applications);
        }

        [Fact]
        public async Task DeleteNonExistingApplicationFails()
        {
            //Arrange
            var applications = _applicationRepository.Applications.ToArray();
            ApplicationsController controller = new ApplicationsController(_applicationRepository, _applicationService);

            //Act
            var response = await controller.DeleteApplications(Guid.NewGuid(), _certificateGroupService);
            //Assert
            Assert.IsAssignableFrom<NotFoundResult>(response);
            var result = response as NotFoundResult;
            Assert.NotNull(result);
            Assert.Equal(applications, _applicationRepository.Applications.ToArray());
        }
    }
}

