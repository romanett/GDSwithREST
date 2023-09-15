using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Gds.Server;
using System.Collections.ObjectModel;

namespace MinAPI.Services.GdsBackgroundService
{
    public class GdsService: IGdsService
    {

        private ApplicationInstance? _application;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GdsService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartServer(CancellationToken stoppingToken)
        {
            if (_application == null)
            {
                //Create OPC Server Application to host GDS
                _application = new ApplicationInstance
                {
                    ApplicationName = "Global Discovery Server",
                    ApplicationType = ApplicationType.Server,
                    ConfigSectionName = "Opc.Ua.GlobalDiscoveryServer"
                };
                // load the application configuration.
                await _application.LoadApplicationConfiguration("Services/GdsBackgroundService/Opc.Ua.GlobalDiscoveryServer.Config.xml", false);
                // check the application certificate.
                await _application.CheckApplicationInstanceCertificate(false, 0);

                var database = new GdsDatabase(_serviceScopeFactory);
                database.Initialize();
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
        }

        public void StopServer()
        {
            _application?.Stop();
        }
        public ReadOnlyCollection<EndpointDescription> GetEndpoints()
        {
            if (_application is null) return new EndpointDescriptionCollection().AsReadOnly<EndpointDescription>();
            return _application.Server.GetEndpoints().AsReadOnly<EndpointDescription>();
        }
    }
}
