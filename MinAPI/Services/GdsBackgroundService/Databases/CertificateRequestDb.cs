using GDSwithREST.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinAPI.Data;
using Newtonsoft.Json;
using Opc.Ua;
using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;

namespace GDSwithREST.Services.GdsBackgroundService.Databases
{
    public class CertificateRequestDb : ICertificateRequest
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;
        public CertificateRequestDb(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Initialize()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            context.Database.Migrate();
        }
        public ushort NamespaceIndex { get; set; }

        public NodeId StartSigningRequest(
                        NodeId applicationId,
                        string certificateGroupId,
                        string certificateTypeId,
                        byte[] certificateRequest,
                        string authorityId)
        {
            Guid id = GetNodeIdGuid(applicationId);

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var application = (from x in context.Applications where x.ApplicationId == id select x).SingleOrDefault();

            if (application == null)
            {
                throw new ServiceResultException(Opc.Ua.StatusCodes.BadNodeIdUnknown);
            }

            var request = (from x in application.CertificateRequests where x.AuthorityId == authorityId select x).SingleOrDefault();

            bool isNew = false;

            if (request == null)
            {
                request = new CertificateRequests() { RequestId = Guid.NewGuid(), AuthorityId = authorityId };
                isNew = true;
            }

            request.State = (int)CertificateRequestState.New;
            request.CertificateGroupId = certificateGroupId;
            request.CertificateTypeId = certificateTypeId;
            request.SubjectName = null;
            request.DomainNames = null;
            request.PrivateKeyFormat = null;
            request.PrivateKeyPassword = null;
            request.CertificateSigningRequest = certificateRequest;

            if (isNew)
            {
                application.CertificateRequests.Add(request);
            }

            context.SaveChanges();

            return new NodeId(request.RequestId, NamespaceIndex);

        }

        public NodeId StartNewKeyPairRequest(
            NodeId applicationId,
            string certificateGroupId,
            string certificateTypeId,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword,
            string authorityId)
        {
            Guid id = GetNodeIdGuid(applicationId);

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var application = (from x in context.Applications where x.ApplicationId == id select x).SingleOrDefault();

            if (application == null)
            {
                throw new ServiceResultException(Opc.Ua.StatusCodes.BadNodeIdUnknown);
            }

            var request = (from x in application.CertificateRequests where x.AuthorityId == authorityId select x).SingleOrDefault();

            bool isNew = false;

            if (request == null)
            {
                request = new CertificateRequests() { RequestId = Guid.NewGuid(), AuthorityId = authorityId };
                isNew = true;
            }

            request.State = (int)CertificateRequestState.New;
            request.CertificateGroupId = certificateGroupId;
            request.CertificateTypeId = certificateTypeId;
            request.SubjectName = subjectName;
            request.DomainNames = JsonConvert.SerializeObject(domainNames);
            request.PrivateKeyFormat = privateKeyFormat;
            request.PrivateKeyPassword = privateKeyPassword;
            request.CertificateSigningRequest = null;

            if (isNew)
            {
                application.CertificateRequests.Add(request);
            }

            context.SaveChanges();

            return new NodeId(request.RequestId, NamespaceIndex);

        }

        public void ApproveRequest(
            NodeId requestId,
        bool isRejected
            )
        {
            Guid id = GetNodeIdGuid(requestId);
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var request = (from x in context.CertificateRequests where x.RequestId == id select x).SingleOrDefault();

            if (request == null)
            {
                throw new ServiceResultException(Opc.Ua.StatusCodes.BadNodeIdUnknown);
            }

            if (isRejected)
            {
                request.State = (int)CertificateRequestState.Rejected;
                // erase information which is ot required anymore
                request.CertificateSigningRequest = null;
                request.PrivateKeyPassword = null;
            }
            else
            {
                request.State = (int)CertificateRequestState.Approved;
            }

            context.SaveChanges();

        }

        public void AcceptRequest(
            NodeId requestId,
            byte[] certificate)
        {
            Guid id = GetNodeIdGuid(requestId);
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var request = (from x in context.CertificateRequests where x.RequestId == id select x).SingleOrDefault();

            if (request == null)
            {
                throw new ServiceResultException(Opc.Ua.StatusCodes.BadNodeIdUnknown);
            }

            request.State = (int)CertificateRequestState.Accepted;

            // erase information which is ot required anymore
            request.CertificateSigningRequest = null;
            request.PrivateKeyPassword = null;

            context.SaveChanges();

        }


        public CertificateRequestState FinishRequest(
            NodeId applicationId,
            NodeId requestId,
            out string? certificateGroupId,
            out string? certificateTypeId,
            out byte[]? signedCertificate,
            out byte[]? privateKey)
        {
            certificateGroupId = null;
            certificateTypeId = null;
            signedCertificate = null;
            privateKey = null;
            Guid reqId = GetNodeIdGuid(requestId);
            Guid appId = GetNodeIdGuid(applicationId);

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var request = (from x in context.CertificateRequests where x.RequestId == reqId select x).SingleOrDefault();

            if (request == null)
            {
                throw new ServiceResultException(Opc.Ua.StatusCodes.BadInvalidArgument);
            }

            switch (request.State)
            {
                case (int)CertificateRequestState.New:
                    return CertificateRequestState.New;
                case (int)CertificateRequestState.Rejected:
                    return CertificateRequestState.Rejected;
                case (int)CertificateRequestState.Accepted:
                    return CertificateRequestState.Accepted;
                case (int)CertificateRequestState.Approved:
                    break;
                default:
                    throw new ServiceResultException(Opc.Ua.StatusCodes.BadInvalidArgument);
            }

            certificateGroupId = request.CertificateGroupId;
            certificateTypeId = request.CertificateTypeId;

            context.SaveChanges();
            return CertificateRequestState.Approved;

        }

        public CertificateRequestState ReadRequest(
            NodeId applicationId,
            NodeId requestId,
            out string? certificateGroupId,
            out string? certificateTypeId,
            out byte[]? certificateRequest,
            out string? subjectName,
            out string[]? domainNames,
            out string? privateKeyFormat,
            out string? privateKeyPassword)
        {
            certificateGroupId = null;
            certificateTypeId = null;
            certificateRequest = null;
            subjectName = null;
            domainNames = null;
            privateKeyFormat = null;
            privateKeyPassword = null;
            Guid reqId = GetNodeIdGuid(requestId);
            Guid appId = GetNodeIdGuid(applicationId);

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GdsdbContext>();
            var request = (from x in context.CertificateRequests where x.RequestId == reqId select x).SingleOrDefault();

            if (request == null)
            {
                throw new ServiceResultException(Opc.Ua.StatusCodes.BadInvalidArgument);
            }

            switch (request.State)
            {
                case (int)CertificateRequestState.New:
                    return CertificateRequestState.New;
                case (int)CertificateRequestState.Rejected:
                    return CertificateRequestState.Rejected;
                case (int)CertificateRequestState.Accepted:
                    return CertificateRequestState.Accepted;
                case (int)CertificateRequestState.Approved:
                    break;
                default:
                    throw new ServiceResultException(Opc.Ua.StatusCodes.BadInvalidArgument);
            }

            certificateGroupId = request.CertificateGroupId;
            certificateTypeId = request.CertificateTypeId;
            certificateRequest = request.CertificateSigningRequest;
            subjectName = request.SubjectName;
            domainNames = request.DomainNames != null ? JsonConvert.DeserializeObject<string[]>(request.DomainNames) : null;
            privateKeyFormat = request.PrivateKeyFormat;
            privateKeyPassword = request.PrivateKeyPassword;

            context.SaveChanges();
            return CertificateRequestState.Approved;

        }

        protected Guid GetNodeIdGuid(
            NodeId nodeId
            )
        {
            if (NodeId.IsNull(nodeId))
            {
                throw new ArgumentNullException(nameof(nodeId));
            }

            if (nodeId.IdType != IdType.Guid || NamespaceIndex != nodeId.NamespaceIndex)
            {
                throw new ServiceResultException(Opc.Ua.StatusCodes.BadNodeIdUnknown);
            }

            Guid? id = nodeId.Identifier as Guid?;

            if (id == null)
            {
                throw new ServiceResultException(Opc.Ua.StatusCodes.BadNodeIdUnknown);
            }
            return (Guid)id;
        }
    }
}
