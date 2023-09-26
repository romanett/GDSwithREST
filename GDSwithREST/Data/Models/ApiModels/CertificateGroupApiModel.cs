using Opc.Ua.Gds.Server;
using Opc.Ua;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;

namespace GDSwithREST.Data.Models.ApiModels
{
    public class CertificateGroupApiModel
    {
        NodeId? Id { get; set; }
        NodeId? CertificateType { get; set; }
        X509Certificate2? Certificate { get; set; }
        TrustListState? DefaultTrustList { get; set; }
        bool UpdateRequired { get; set; }

        //configuration to get TrustList
    }
}
