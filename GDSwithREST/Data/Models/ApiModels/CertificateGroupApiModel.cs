using Opc.Ua.Gds.Server;
using Opc.Ua;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using GDSwithREST.Services.GdsBackgroundService.Databases;

namespace GDSwithREST.Data.Models.ApiModels
{
    public class CertificateGroupApiModel
    {
        public CertificateGroupApiModel(CertificateGroup certificateGroup)
        {
            Id = certificateGroup.Id;
            CertificateType = certificateGroup.CertificateType;
            Certificate = certificateGroup.Certificate;
            DefaultTrustList = certificateGroup.DefaultTrustList;
            UpdateRequired = certificateGroup.UpdateRequired;
        }
        public CertificateGroupApiModel(NodeId? id, NodeId? certificateType, X509Certificate2? certificate, TrustListState? defaultTrustList, bool updateRequired)
        {
            Id = id;
            CertificateType = certificateType;
            Certificate = certificate;
            DefaultTrustList = defaultTrustList;
            UpdateRequired = updateRequired;
        }

        NodeId? Id { get; set; }
        NodeId? CertificateType { get; set; }
        X509Certificate2? Certificate { get; set; }
        TrustListState? DefaultTrustList { get; set; }
        bool UpdateRequired { get; set; }

        //configuration to get TrustList
    }
}
