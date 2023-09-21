﻿using GDSwithREST.Services.GdsBackgroundService.Databases;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;
using System.Collections;
using System.Collections.ObjectModel;

namespace MinAPI.Services.GdsBackgroundService
{
    public class GdsService: IGdsService
    {

        private ApplicationInstance? _applicationInstance;
        private readonly IApplicationsDatabase _applications;
        private readonly ICertificateRequest _certificateRequests;
        private readonly ICertificateGroup _certificateGroups;

        public GdsService(IApplicationsDatabase applications, ICertificateGroup certificateGroup, ICertificateRequest certificateRequests)
        {
            _applications = applications;
            _certificateGroups = certificateGroup;
            _certificateRequests = certificateRequests;
        }

        public async Task StartServer(CancellationToken stoppingToken)
        {
            if (_applicationInstance == null)
            {
                //Create OPC Server Application to host GDS
                _applicationInstance = new ApplicationInstance
                {
                    ApplicationName = "Global Discovery Server",
                    ApplicationType = ApplicationType.Server,
                    ConfigSectionName = "Opc.Ua.GlobalDiscoveryServer"
                };
                // load the application configuration.
                await _applicationInstance.LoadApplicationConfiguration("Services/GdsBackgroundService/Opc.Ua.GlobalDiscoveryServer.Config.xml", false);
                // check the application certificate.
                await _applicationInstance.CheckApplicationInstanceCertificate(false, 0);

                _applications.Initialize();
                _certificateRequests.Initialize();
                //await _certificateGroup.Init();
                var gdsServer = new GlobalDiscoverySampleServer(
                        _applications,
                        _certificateRequests,
                        _certificateGroups
                       );

                //start GDS
                await _applicationInstance.Start(gdsServer);

                var endpoints = _applicationInstance.Server.GetEndpoints().Select(e => e.EndpointUrl).Distinct();

                foreach (var endpoint in endpoints)
                {
                    Console.WriteLine(endpoint);
                }
            }
        }

        public void StopServer()
        {
            _applicationInstance?.Stop();
        }
        public IEnumerable<String> GetEndpointURLs()
        {
            if (_applicationInstance is null) return new List<String>();
            return _applicationInstance.Server.GetEndpoints().Select(e => e.EndpointUrl).Distinct();
        }
    }
}
