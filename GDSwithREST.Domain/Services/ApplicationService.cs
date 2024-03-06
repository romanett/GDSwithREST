using Opc.Ua;
using Opc.Ua.Gds;
using Opc.Ua.Gds.Server.Database;
using GDSwithREST.Domain.Entities;
using GDSwithREST.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GDSwithREST.Domain.Services
{
    public class ApplicationService : ApplicationsDatabaseBase
    {
        private DateTime m_lastCounterResetTime = DateTime.MinValue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ApplicationService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }


        #region IApplicationsDatabase
        public override void Initialize()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var persistencyRepository = scope.ServiceProvider.GetRequiredService<IPersistencyRepository>();

            persistencyRepository.MigrateDatabase();
        }

        public override NodeId RegisterApplication(
                ApplicationRecordDataType application
                )
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
            var serverEndpointRepository = scope.ServiceProvider.GetRequiredService<IServerEndpointRepository>();
            var applicationNameRepository = scope.ServiceProvider.GetRequiredService<IApplicationNameRepository>();

            NodeId appNodeId = base.RegisterApplication(application);
            if (NodeId.IsNull(appNodeId))
            {
                appNodeId = new NodeId(Guid.NewGuid(), NamespaceIndex);
            }
            Guid applicationId = GetNodeIdGuid(appNodeId);
            string capabilities = ServerCapabilities(application);

            Application? record = null;

            if (applicationId != Guid.Empty)
            {
                record = applicationRepository.GetApplicationById(applicationId).Result;

                if (record != null)
                {
                    serverEndpointRepository.RemoveServerEndpoints(
                        serverEndpointRepository.GetServerEndpointsByApplicationId(record.Id)
                        .Result.ToArray());

                    applicationNameRepository.RemoveApplicationNames(
                        applicationNameRepository.GetApplicationNamesByApplicationId(record.Id).Result.ToArray());
                }
            }

            bool isNew = false;

            if (record == null)
            {
                applicationId = Guid.NewGuid();
                record = new Application() { ApplicationId = applicationId };
                isNew = true;
            }

            record.ApplicationUri = application.ApplicationUri;
            record.ApplicationName = application.ApplicationNames[0].Text;
            record.ApplicationType = (int)application.ApplicationType;
            record.ProductUri = application.ProductUri;
            record.ServerCapabilities = capabilities;

            if (isNew)
            {
                applicationRepository.AddApplication(record);
            }

            if (application.DiscoveryUrls != null)
            {
                foreach (var discoveryUrl in application.DiscoveryUrls)
                {
                    serverEndpointRepository.AddServerEndpoint(new ServerEndpoint() { ApplicationId = record.Id, DiscoveryUrl = discoveryUrl });
                }
            }

            if (application.ApplicationNames != null && application.ApplicationNames.Count >= 1)
            {
                foreach (var applicationName in application.ApplicationNames)
                {
                    applicationNameRepository.AddApplicationName(new ApplicationName() { ApplicationId = record.Id, Locale = applicationName.Locale, Text = applicationName.Text });
                }
            }
            applicationRepository.SaveChanges(record);
            m_lastCounterResetTime = DateTime.UtcNow;
            return new NodeId(applicationId, NamespaceIndex); ;
        }

        public override void UnregisterApplication(NodeId applicationId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
            var serverEndpointRepository = scope.ServiceProvider.GetRequiredService<IServerEndpointRepository>();
            var applicationNameRepository = scope.ServiceProvider.GetRequiredService<IApplicationNameRepository>();
            var certificateRequestRepository = scope.ServiceProvider.GetRequiredService<ICertificateRequestRepository>();
            var trustListRepository = scope.ServiceProvider.GetRequiredService<ITrustListRepository>();

            Guid id = GetNodeIdGuid(applicationId);

            List<byte[]> certificates = new();

            var application = applicationRepository.GetApplicationById(id).Result;

            if (application == null)
            {
                throw new ArgumentException("A record with the specified application id does not exist.", nameof(applicationId));
            }
            certificateRequestRepository.RemoveCertificateRequests(application.CertificateRequests.ToArray());
            applicationNameRepository.RemoveApplicationNames(application.ApplicationNames.ToArray());
            serverEndpointRepository.RemoveServerEndpoints(application.ServerEndpoints.ToArray());
            applicationRepository.RemoveApplication(application);
            trustListRepository.RemoveTrustLists(application.TrustLists.ToArray());

            certificateRequestRepository.SaveChanges();
            applicationRepository.SaveChanges(application);
            m_lastCounterResetTime = DateTime.UtcNow;
        }

        public override ApplicationRecordDataType? GetApplication(
                NodeId applicationId
                )
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();

            Guid id = GetNodeIdGuid(applicationId);


            var application = applicationRepository.GetApplicationById(id).Result;

            if (application == null)
            {
                return null;
            }

            LocalizedTextCollection names = new LocalizedTextCollection();
            if (application.ApplicationNames != null)
            {
                foreach (var entry in application.ApplicationNames)
                {
                    names.Add(new LocalizedText(entry.Locale, entry.Text));
                }
            }
            else
            {
                names.Add(new LocalizedText(application.ApplicationName));
            }

            StringCollection? discoveryUrls = null;

            if (application.ServerEndpoints != null)
            {
                discoveryUrls = new StringCollection();

                foreach (var endpoint in application.ServerEndpoints)
                {
                    discoveryUrls.Add(endpoint.DiscoveryUrl);
                }
            }

            string[]? capabilities = null;

            if (application.ServerCapabilities != null &&
                application.ServerCapabilities.Length > 0)
            {
                capabilities = application.ServerCapabilities.Split(',');
            }

            return new ApplicationRecordDataType()
            {
                ApplicationId = new NodeId(application.ApplicationId, NamespaceIndex),
                ApplicationUri = application.ApplicationUri,
                ApplicationType = (ApplicationType)application.ApplicationType,
                ApplicationNames = names,
                ProductUri = application.ProductUri,
                DiscoveryUrls = discoveryUrls,
                ServerCapabilities = capabilities
            };
        }

        public override ApplicationRecordDataType[] FindApplications(
                string applicationUri
                )
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();

            var applications = applicationRepository.GetApplicationsByUri(applicationUri);

            List<ApplicationRecordDataType> records = new();

            foreach (var application in applications)
            {
                LocalizedText[]? names = null;

                if (application.ApplicationName != null)
                {
                    names = new LocalizedText[] { application.ApplicationName };
                }

                StringCollection? discoveryUrls = null;

                if (application.ServerEndpoints != null)
                {
                    discoveryUrls = new StringCollection();

                    foreach (var endpoint in application.ServerEndpoints)
                    {
                        discoveryUrls.Add(endpoint.DiscoveryUrl);
                    }
                }

                string[]? capabilities = null;

                if (application.ServerCapabilities != null)
                {
                    capabilities = application.ServerCapabilities.Split(',');
                }

                records.Add(new ApplicationRecordDataType()
                {
                    ApplicationId = new NodeId(application.ApplicationId, NamespaceIndex),
                    ApplicationUri = application.ApplicationUri,
                    ApplicationType = (ApplicationType)application.ApplicationType,
                    ApplicationNames = new LocalizedTextCollection(names),
                    ProductUri = application.ProductUri,
                    DiscoveryUrls = discoveryUrls,
                    ServerCapabilities = capabilities
                });
            }

            return records.ToArray();

        }

        public override ApplicationDescription[] QueryApplications(
                uint startingRecordId,
                uint maxRecordsToReturn,
                string applicationName,
                string applicationUri,
                uint applicationType,
                string productUri,
                string[] serverCapabilities,
                out DateTime lastCounterResetTime,
                out uint nextRecordId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();

            lastCounterResetTime = DateTime.MinValue;
            nextRecordId = 0;
            var records = new List<ApplicationDescription>();

            lastCounterResetTime = m_lastCounterResetTime;

            var applications = from x in applicationRepository.GetAllApplications().Result
                               where (int)startingRecordId == 0 || (int)startingRecordId <= x.Id
                               orderby x.Id
                               select x;

            int lastID = 0;

            foreach (var application in applications)
            {

                if (!string.IsNullOrEmpty(applicationName))
                {
                    if (!Match(application.ApplicationName, applicationName))
                    {
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(applicationUri))
                {
                    if (!Match(application.ApplicationUri, applicationUri))
                    {
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(productUri))
                {
                    if (!Match(application.ProductUri, productUri))
                    {
                        continue;
                    }
                }

                string[]? capabilities = null;
                if (!string.IsNullOrEmpty(application.ServerCapabilities))
                {
                    capabilities = application.ServerCapabilities.Split(',');
                }

                if (serverCapabilities != null && serverCapabilities.Length > 0)
                {
                    bool match = true;

                    for (int ii = 0; ii < serverCapabilities.Length; ii++)
                    {
                        if (capabilities == null || !capabilities.Contains(serverCapabilities[ii]))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (!match)
                    {
                        continue;
                    }
                }

                // type filter, 0 and 3 returns all
                // filter for servers
                if (applicationType == 1 &&
                    application.ApplicationType == (int)ApplicationType.Client)
                {
                    continue;
                }
                else // filter for clients
                if (applicationType == 2 &&
                    application.ApplicationType != (int)ApplicationType.Client &&
                    application.ApplicationType != (int)ApplicationType.ClientAndServer)
                {
                    continue;
                }

                var discoveryUrls = new StringCollection();
                if (application.ServerEndpoints != null)
                {
                    discoveryUrls = new StringCollection();

                    foreach (var endpoint in application.ServerEndpoints)
                    {
                        discoveryUrls.Add(endpoint.DiscoveryUrl);
                    }
                }

                if (lastID == 0)
                {
                    lastID = application.Id;
                }
                else
                {
                    if (maxRecordsToReturn != 0 &&
                        records.Count >= maxRecordsToReturn)
                    {
                        break;
                    }

                    lastID = application.Id;
                }

                records.Add(new ApplicationDescription()
                {
                    ApplicationUri = application.ApplicationUri,
                    ProductUri = application.ProductUri,
                    ApplicationName = application.ApplicationName,
                    ApplicationType = (ApplicationType)application.ApplicationType,
                    GatewayServerUri = null,
                    DiscoveryProfileUri = null,
                    DiscoveryUrls = discoveryUrls
                });
                nextRecordId = (uint)lastID + 1;

            }
            return records.ToArray();

        }

        public override ServerOnNetwork[] QueryServers(
                uint startingRecordId,
                uint maxRecordsToReturn,
                string applicationName,
                string applicationUri,
                string productUri,
                string[] serverCapabilities,
                out DateTime lastCounterResetTime)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
            var serverEndpointRepository = scope.ServiceProvider.GetRequiredService<IServerEndpointRepository>();

            lastCounterResetTime = m_lastCounterResetTime;

            var applications = applicationRepository.GetAllApplications().Result;
            var serverEndpoints = serverEndpointRepository.GetAllServerEndpoints().Result;

            var results = from x in serverEndpoints
                          join y in applications on x.ApplicationId equals y.Id
                          where (int)startingRecordId == 0 || (int)startingRecordId <= x.Id
                          orderby x.Id
                          select new
                          {
                              x.Id,
                              y.ApplicationName,
                              y.ApplicationUri,
                              y.ProductUri,
                              x.DiscoveryUrl,
                              y.ServerCapabilities
                          };

            List<ServerOnNetwork> records = new();
            int lastID = 0;

            foreach (var result in results)
            {

                if (!string.IsNullOrEmpty(applicationName))
                {
                    if (!Match(result.ApplicationName, applicationName))
                    {
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(applicationUri))
                {
                    if (!Match(result.ApplicationUri, applicationUri))
                    {
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(productUri))
                {
                    if (!Match(result.ProductUri, productUri))
                    {
                        continue;
                    }
                }

                string[]? capabilities = null;
                if (!string.IsNullOrEmpty(result.ServerCapabilities))
                {
                    capabilities = result.ServerCapabilities.Split(',');
                }

                if (serverCapabilities != null && serverCapabilities.Length > 0)
                {
                    bool match = true;

                    for (int ii = 0; ii < serverCapabilities.Length; ii++)
                    {
                        if (capabilities == null || !capabilities.Contains(serverCapabilities[ii]))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (!match)
                    {
                        continue;
                    }
                }

                if (lastID == 0)
                {
                    lastID = result.Id;
                }
                else
                {
                    if (maxRecordsToReturn != 0 &&
                        lastID != result.Id &&
                        records.Count >= maxRecordsToReturn)
                    {
                        break;
                    }

                    lastID = result.Id;
                }

                records.Add(new ServerOnNetwork()
                {
                    RecordId = (uint)result.Id,
                    ServerName = result.ApplicationName,
                    DiscoveryUrl = result.DiscoveryUrl,
                    ServerCapabilities = capabilities
                });


            }

            return records.ToArray();

        }

        public override bool SetApplicationCertificate(
                NodeId applicationId,
                string certificateTypeId,
                byte[] certificate)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();

            Guid id = GetNodeIdGuid(applicationId);

            if (certificateTypeId.Equals(nameof(Opc.Ua.ObjectTypeIds.UserCredentialCertificateType), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var application = applicationRepository.GetApplicationById(id).Result;

            if (application == null)
            {
                return false;
            }

            if (certificateTypeId.Equals(nameof(Opc.Ua.ObjectTypeIds.HttpsCertificateType), StringComparison.OrdinalIgnoreCase))
            {
                application.HttpsCertificate = certificate;
            }
            else
            {
                application.Certificate = certificate;
            }

            applicationRepository.SaveChanges(application);


            return true;
        }

        public override bool GetApplicationCertificate(
            NodeId applicationId,
            string certificateTypeId,
            out byte[]? certificate)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();

            certificate = null;

            Guid id = (Guid)applicationId.Identifier;

            List<byte[]> certificates = new();

            if (certificateTypeId.Equals(nameof(Opc.Ua.ObjectTypeIds.UserCredentialCertificateType), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }


            var application = applicationRepository.GetApplicationById(id).Result;

            if (application == null)
            {
                throw new ArgumentException("A record with the specified application id does not exist.", nameof(applicationId));
            }

            if (certificateTypeId.Equals(nameof(Opc.Ua.ObjectTypeIds.HttpsCertificateType), StringComparison.OrdinalIgnoreCase))
            {
                certificate = application.HttpsCertificate;
            }
            else
            {
                certificate = application.Certificate;
            }


            return certificate != null;
        }


        public override bool SetApplicationTrustLists(
            NodeId applicationId,
            string certificateTypeId,
            string trustListId
            )
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
            var trustListRepository = scope.ServiceProvider.GetRequiredService<ITrustListRepository>();

            Guid id = GetNodeIdGuid(applicationId);
            var application = applicationRepository.GetApplicationById(id).Result;

            if (application == null || string.IsNullOrEmpty(trustListId) || string.IsNullOrEmpty(certificateTypeId))
            {
                return false;
            }
            var trustList = trustListRepository.GetTrustListsByApplicationId(application.Id).Result
                .SingleOrDefault(trustList => trustList.CertificateType == certificateTypeId);
            
            if (trustList == null)
            {
                trustListRepository.AddTrustList(
                    new TrustList
                    {
                        ApplicationId = application.Id,
                        Application = application,
                        CertificateType = certificateTypeId,
                        Path = trustListId
                    });
            }
            else
            {
                trustList.Path = trustListId;
            }
            return true;
        }

        public override bool GetApplicationTrustLists(
            NodeId applicationId,
            string certificateTypeId,
            out string trustListId
            )
        {
            trustListId = null!;
            using var scope = _serviceScopeFactory.CreateScope();
            var applicationRepository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
            var trustListRepository = scope.ServiceProvider.GetRequiredService<ITrustListRepository>();

            Guid id = GetNodeIdGuid(applicationId);
            var application = applicationRepository.GetApplicationById(id).Result;

            if (application == null || string.IsNullOrEmpty(certificateTypeId))
            {
                return false;
            }
            var trustList = trustListRepository.GetTrustListsByApplicationId(application.Id).Result
                .SingleOrDefault(trustList => trustList.CertificateType == certificateTypeId);
            
            if (trustList == null)
            {
                return false;
            }
            trustListId = trustList.Path;
            return true;
        }

        #endregion
    }
}
