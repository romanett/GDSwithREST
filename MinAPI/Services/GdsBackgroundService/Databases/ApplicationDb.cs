using Microsoft.EntityFrameworkCore;
using MinAPI.Data;
using MinAPI.Models;
using Newtonsoft.Json;
using Opc.Ua;
using Opc.Ua.Gds;
using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;
using System.Reflection;

namespace GDSwithREST.Services.GdsBackgroundService.Databases
{
    public class ApplicationDb : ApplicationsDatabaseBase
    {
        private DateTime m_lastCounterResetTime = DateTime.MinValue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ApplicationDb(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }


        #region IApplicationsDatabase
        public override void Initialize()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            context.Database.Migrate();
        }

        public override NodeId RegisterApplication(
                ApplicationRecordDataType application
                )
        {
            NodeId appNodeId = base.RegisterApplication(application);
            if (NodeId.IsNull(appNodeId))
            {
                appNodeId = new NodeId(Guid.NewGuid(), NamespaceIndex);
            }
            Guid applicationId = GetNodeIdGuid(appNodeId);
            string capabilities = ServerCapabilities(application);
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            Applications? record = null;

            if (applicationId != Guid.Empty)
            {
                var results = from ii in context.Applications
                              where ii.ApplicationId == applicationId
                              select ii;

                record = results.SingleOrDefault();

                if (record != null)
                {
                    var endpoints = from ii in context.ServerEndpoints
                                    where ii.ApplicationId == record.Id
                                    select ii;

                    foreach (var endpoint in endpoints)
                    {
                        context.ServerEndpoints.Remove(endpoint);
                    }

                    var names = from ii in context.ApplicationNames
                                where ii.ApplicationId == record.Id
                                select ii;

                    foreach (var name in names)
                    {
                        context.ApplicationNames.Remove(name);
                    }

                    context.SaveChanges();
                }
            }

            bool isNew = false;

            if (record == null)
            {
                applicationId = Guid.NewGuid();
                record = new Applications() { ApplicationId = applicationId };
                isNew = true;
            }

            record.ApplicationUri = application.ApplicationUri;
            record.ApplicationName = application.ApplicationNames[0].Text;
            record.ApplicationType = (int)application.ApplicationType;
            record.ProductUri = application.ProductUri;
            record.ServerCapabilities = capabilities;

            if (isNew)
            {
                context.Applications.Add(record);
            }
            context.SaveChanges();

            if (application.DiscoveryUrls != null)
            {
                foreach (var discoveryUrl in application.DiscoveryUrls)
                {
                    context.ServerEndpoints.Add(new ServerEndpoints() { ApplicationId = record.Id, DiscoveryUrl = discoveryUrl });
                }
            }

            if (application.ApplicationNames != null && application.ApplicationNames.Count >= 1)
            {
                foreach (var applicationName in application.ApplicationNames)
                {
                    context.ApplicationNames.Add(new ApplicationNames() { ApplicationId = record.Id, Locale = applicationName.Locale, Text = applicationName.Text });
                }
            }

            context.SaveChanges();
            m_lastCounterResetTime = DateTime.UtcNow;
            return new NodeId(applicationId, NamespaceIndex); ;
        }

        public override void UnregisterApplication(NodeId applicationId)
        {
            Guid id = GetNodeIdGuid(applicationId);

            List<byte[]> certificates = new();


            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();

            var result = (from ii in context.Applications
                          where ii.ApplicationId == id
                          select ii).SingleOrDefault();

            if (result == null)
            {
                throw new ArgumentException("A record with the specified application id does not exist.", nameof(applicationId));
            }

            foreach (var entry in new List<CertificateRequests>(result.CertificateRequests))
            {
                context.CertificateRequests.Remove(entry);
            }

            foreach (var entry in new List<ApplicationNames>(result.ApplicationNames))
            {
                context.ApplicationNames.Remove(entry);
            }

            foreach (var entry in new List<ServerEndpoints>(result.ServerEndpoints))
            {
                context.ServerEndpoints.Remove(entry);
            }

            context.Applications.Remove(result);
            context.SaveChanges();
            m_lastCounterResetTime = DateTime.UtcNow;

        }

        public override ApplicationRecordDataType? GetApplication(
                NodeId applicationId
                )
        {
            Guid id = GetNodeIdGuid(applicationId);

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();

            var results = from x in context.Applications
                          where x.ApplicationId == id
                          select x;

            var result = results.SingleOrDefault();

            if (result == null)
            {
                return null;
            }

            LocalizedTextCollection names = new LocalizedTextCollection();
            if (result.ApplicationNames != null)
            {
                foreach (var entry in new List<ApplicationNames>(result.ApplicationNames))
                {
                    names.Add(new LocalizedText(entry.Locale, entry.Text));
                }
            }
            else
            {
                names.Add(new LocalizedText(result.ApplicationName));
            }

            StringCollection? discoveryUrls = null;

            if (result.ServerEndpoints != null)
            {
                discoveryUrls = new StringCollection();

                foreach (var endpoint in result.ServerEndpoints)
                {
                    discoveryUrls.Add(endpoint.DiscoveryUrl);
                }
            }

            string[]? capabilities = null;

            if (result.ServerCapabilities != null &&
                result.ServerCapabilities.Length > 0)
            {
                capabilities = result.ServerCapabilities.Split(',');
            }

            return new ApplicationRecordDataType()
            {
                ApplicationId = new NodeId(result.ApplicationId, NamespaceIndex),
                ApplicationUri = result.ApplicationUri,
                ApplicationType = (ApplicationType)result.ApplicationType,
                ApplicationNames = names,
                ProductUri = result.ProductUri,
                DiscoveryUrls = discoveryUrls,
                ServerCapabilities = capabilities
            };
        }

        public override ApplicationRecordDataType[] FindApplications(
                string applicationUri
                )
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var results = from x in context.Applications
                          where x.ApplicationUri == applicationUri
                          select x;

            List<ApplicationRecordDataType> records = new();

            foreach (var result in results)
            {
                LocalizedText[]? names = null;

                if (result.ApplicationName != null)
                {
                    names = new LocalizedText[] { result.ApplicationName };
                }

                StringCollection? discoveryUrls = null;

                if (result.ServerEndpoints != null)
                {
                    discoveryUrls = new StringCollection();

                    foreach (var endpoint in result.ServerEndpoints)
                    {
                        discoveryUrls.Add(endpoint.DiscoveryUrl);
                    }
                }

                string[]? capabilities = null;

                if (result.ServerCapabilities != null)
                {
                    capabilities = result.ServerCapabilities.Split(',');
                }

                records.Add(new ApplicationRecordDataType()
                {
                    ApplicationId = new NodeId(result.ApplicationId, NamespaceIndex),
                    ApplicationUri = result.ApplicationUri,
                    ApplicationType = (ApplicationType)result.ApplicationType,
                    ApplicationNames = new LocalizedTextCollection(names),
                    ProductUri = result.ProductUri,
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
            lastCounterResetTime = DateTime.MinValue;
            nextRecordId = 0;
            var records = new List<ApplicationDescription>();

            lastCounterResetTime = m_lastCounterResetTime;

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var results = from x in context.Applications
                          where (int)startingRecordId == 0 || (int)startingRecordId <= x.Id
                          orderby x.Id
                          select x;

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

                // type filter, 0 and 3 returns all
                // filter for servers
                if (applicationType == 1 &&
                    result.ApplicationType == (int)ApplicationType.Client)
                {
                    continue;
                }
                else // filter for clients
                if (applicationType == 2 &&
                    result.ApplicationType != (int)ApplicationType.Client &&
                    result.ApplicationType != (int)ApplicationType.ClientAndServer)
                {
                    continue;
                }

                var discoveryUrls = new StringCollection();
                if (result.ServerEndpoints != null)
                {
                    discoveryUrls = new StringCollection();

                    foreach (var endpoint in result.ServerEndpoints)
                    {
                        discoveryUrls.Add(endpoint.DiscoveryUrl);
                    }
                }

                if (lastID == 0)
                {
                    lastID = result.Id;
                }
                else
                {
                    if (maxRecordsToReturn != 0 &&
                        records.Count >= maxRecordsToReturn)
                    {
                        break;
                    }

                    lastID = result.Id;
                }

                records.Add(new ApplicationDescription()
                {
                    ApplicationUri = result.ApplicationUri,
                    ProductUri = result.ProductUri,
                    ApplicationName = result.ApplicationName,
                    ApplicationType = (ApplicationType)result.ApplicationType,
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
            lastCounterResetTime = m_lastCounterResetTime;

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var results = from x in context.ServerEndpoints
                          join y in context.Applications on x.ApplicationId equals y.Id
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
            Guid id = GetNodeIdGuid(applicationId);

            if (certificateTypeId.Equals(nameof(Opc.Ua.ObjectTypeIds.UserCredentialCertificateType), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var results = from x in context.Applications
                          where x.ApplicationId == id
                          select x;

            var result = results.SingleOrDefault();

            if (result == null)
            {
                return false;
            }

            if (certificateTypeId.Equals(nameof(Opc.Ua.ObjectTypeIds.HttpsCertificateType), StringComparison.OrdinalIgnoreCase))
            {
                result.HttpsCertificate = certificate;
            }
            else
            {
                result.Certificate = certificate;
            }

            context.SaveChanges();


            return true;
        }

        public override bool GetApplicationCertificate(
            NodeId applicationId,
            string certificateTypeId,
            out byte[]? certificate)
        {
            certificate = null;

            Guid id = GetNodeIdGuid(applicationId);

            List<byte[]> certificates = new();

            if (certificateTypeId.Equals(nameof(Opc.Ua.ObjectTypeIds.UserCredentialCertificateType), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var result = (from ii in context.Applications
                          where ii.ApplicationId == id
                          select ii).SingleOrDefault();

            if (result == null)
            {
                throw new ArgumentException("A record with the specified application id does not exist.", nameof(applicationId));
            }

            if (certificateTypeId.Equals(nameof(Opc.Ua.ObjectTypeIds.HttpsCertificateType), StringComparison.OrdinalIgnoreCase))
            {
                certificate = result.HttpsCertificate;
            }
            else
            {
                certificate = result.Certificate;
            }


            return certificate != null;
        }


        public override bool SetApplicationTrustLists(
            NodeId applicationId,
            string trustListId,
            string httpsTrustListId
            )
        {
            Guid id = GetNodeIdGuid(applicationId);
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var result = (from x in context.Applications where x.ApplicationId == id select x).SingleOrDefault();

            if (result == null)
            {
                return false;
            }

            result.TrustListId = null;
            result.HttpsTrustListId = null;

            if (trustListId != null)
            {
                var result2 = (from x in context.CertificateStores where x.Path == trustListId select x).SingleOrDefault();

                if (result2 != null)
                {
                    result.TrustListId = result2.Id;
                }
            }

            if (httpsTrustListId != null)
            {
                var result2 = (from x in context.CertificateStores where x.Path == httpsTrustListId select x).SingleOrDefault();

                if (result2 != null)
                {
                    result.HttpsTrustListId = result2.Id;
                }
            }

            context.SaveChanges();

            return true;
        }

        #endregion
    }
}
