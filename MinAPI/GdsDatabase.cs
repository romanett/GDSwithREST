using Microsoft.EntityFrameworkCore;
using MinAPI.Data;
using Opc.Ua;
using Opc.Ua.Gds;
using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;
using System.Reflection;

namespace MinAPI
{
    public class GdsDatabase : ApplicationsDatabaseBase, ICertificateRequest
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GdsDatabase(IServiceScopeFactory serviceScopeFactory)
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
        #endregion
        #region ICertifiateRequest
        public void AcceptRequest(NodeId requestId, byte[] certificate)
        {
            throw new NotImplementedException();
        }

        public void ApproveRequest(NodeId requestId, bool isRejected)
        {
            throw new NotImplementedException();
        }

        public CertificateRequestState FinishRequest(NodeId applicationId, NodeId requestId, out string certificateGroupId, out string certificateTypeId, out byte[] signedCertificate, out byte[] privateKey)
        {
            throw new NotImplementedException();
        }

        public CertificateRequestState ReadRequest(NodeId applicationId, NodeId requestId, out string certificateGroupId, out string certificateTypeId, out byte[] certificateRequest, out string subjectName, out string[] domainNames, out string privateKeyFormat, out string privateKeyPassword)
        {
            throw new NotImplementedException();
        }

        public NodeId StartNewKeyPairRequest(NodeId applicationId, string certificateGroupId, string certificateTypeId, string subjectName, string[] domainNames, string privateKeyFormat, string privateKeyPassword, string authorityId)
        {
            throw new NotImplementedException();
        }

        public NodeId StartSigningRequest(NodeId applicationId, string certificateGroupId, string certificateTypeId, byte[] certificateRequest, string authorityId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
