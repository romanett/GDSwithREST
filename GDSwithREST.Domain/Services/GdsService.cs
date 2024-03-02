using GDSwithREST.Domain.ApiModels;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;
using Opc.Ua.Server.UserDatabase;

namespace GDSwithREST.Domain.Services
{
    public class GdsService : IGdsService
    {

        private ApplicationInstance? _applicationInstance;
        private readonly IApplicationsDatabase _applications;
        private readonly ICertificateRequest _certificateRequests;
        private readonly ICertificateGroupService _certificateGroups;

        public GdsService(IApplicationsDatabase applications, ICertificateGroupService certificateGroup, ICertificateRequest certificateRequests)
        {
            _applications = applications;
            _certificateGroups = certificateGroup;
            _certificateRequests = certificateRequests;
        }
        /// <summary>
        /// Starts the OPC UA Global Discovery Server
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public async Task StartServer(CancellationToken stoppingToken)
        {
            if (_applicationInstance == null)
            {
                //Create OPC Server Application to host GDS
                _applicationInstance = new ApplicationInstance
                {
                    ApplicationName = "Global Discovery Server",
                    ApplicationType = Opc.Ua.ApplicationType.Server,
                    ConfigSectionName = "Opc.Ua.GlobalDiscoveryServer"
                };
                // load the application configuration.
                await _applicationInstance.LoadApplicationConfiguration("/OPC Foundation/GDS/config/Opc.Ua.GlobalDiscoveryServer.Config.xml", false);
                // check the application certificate.
                await _applicationInstance.CheckApplicationInstanceCertificate(false, 0);

                _applications.Initialize();
                _certificateRequests.Initialize();

                // get the DatabaseStorePath configuration parameter.
                GlobalDiscoveryServerConfiguration gdsConfiguration = _applicationInstance.ApplicationConfiguration.ParseExtension<GlobalDiscoveryServerConfiguration>();
                string usersDatabaseStorePath = Utils.ReplaceSpecialFolderNames(gdsConfiguration.UsersDatabaseStorePath);
                var usersDatabase = JsonUserDatabase.Load(usersDatabaseStorePath);
                //await _certificateGroup.Init();
                var gdsServer = new GlobalDiscoverySampleServer(
                        _applications,
                        _certificateRequests,
                        _certificateGroups,
                        usersDatabase
                       );

                //start GDS
                await _applicationInstance.Start(gdsServer);

                ////trust GDS CA
                //var defaultCertificateGroup = _certificateGroups.CertificateGroups.SingleOrDefault(cg => cg.Id.Identifier is (uint)CertificateGroupType.DefaultApplicationGroup);
                //if (defaultCertificateGroup is null)
                //    throw new Exception("Failed to initialze GDS CA Certifcate");

                //await _applicationInstance.AddOwnCertificateToTrustedStoreAsync(defaultCertificateGroup.Certificate, stoppingToken);

                var endpoints = _applicationInstance.Server.GetEndpoints().Select(e => e.EndpointUrl).Distinct();

                foreach (var endpoint in endpoints)
                {
                    Console.WriteLine(endpoint);
                }
            }
        }
        /// <summary>
        /// Stops the OPC UA Global Discovery Server
        /// </summary>
        public void StopServer()
        {
            _applicationInstance?.Stop();
        }
        /// <summary>
        /// returns all Endpoint URLs of the running OPC UA GDS
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetEndpointURLs()
        {
            if (_applicationInstance is null) return new List<string>();
            return _applicationInstance.Server.GetEndpoints().Select(e => e.EndpointUrl).Distinct();
        }
    }
}
