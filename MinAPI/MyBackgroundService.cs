using Microsoft.Extensions.DependencyInjection;
using MinAPI.Data;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Gds.Server;
using static System.Formats.Asn1.AsnWriter;

namespace MinAPI
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ApplicationInstance? _application;
        public MyBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Create OPC Server Application to host GDS
            _application = new ApplicationInstance
            {
                ApplicationName = "Global Discovery Server",
                ApplicationType = ApplicationType.Server,
                ConfigSectionName = "Opc.Ua.GlobalDiscoveryServer"
            };
            // load the application configuration.
            await _application.LoadApplicationConfiguration(false);
            // check the application certificate.
            await _application.CheckApplicationInstanceCertificate(false, 0);

            var database = new GdsDatabase(_serviceScopeFactory);
            var gdsServer = new GlobalDiscoverySampleServer(
                    database,
                    database,
                    new CertificateGroup()
                   );

            //start GDS
            await _application.Start(gdsServer);

            var endpoints = _application.Server.GetEndpoints().Select(e => e.EndpointUrl).Distinct();

            foreach (var endpoint in endpoints)
            {
                Console.WriteLine(endpoint);
            }
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _application?.Stop();

            await base.StopAsync(stoppingToken);
        }
    }
}
