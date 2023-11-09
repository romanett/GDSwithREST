using Opc.Ua.Gds.Server;
using Opc.Ua;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using GDSwithREST.Services.GdsBackgroundService.Databases;

namespace GDSwithREST.Data.Models.ApiModels
{
    /// <summary>
    /// Certificate Group of the GDS
    /// </summary>
    public class CertificateGroupApiModel
    {
        public CertificateGroupApiModel(CertificateGroup certificateGroup)
        {
            try
            {
                Id = (uint)certificateGroup.Id.Identifier;
                UpdateRequired = certificateGroup.UpdateRequired;
                Ceritificate = new X509CertificateApiModel(certificateGroup.Certificate);
            }
            catch
            {
            }
        }
        public CertificateGroupApiModel(uint id, X509Certificate2 certificate, bool updateRequired)
        {
            try
            {
                Id = id;
                UpdateRequired = updateRequired;
                Ceritificate = new X509CertificateApiModel(certificate);
            }
            catch
            {
            }
        }

        public uint Id { get; set; }
        public bool UpdateRequired { get; set; }
        public X509CertificateApiModel? Ceritificate { get; set; }
        
    }
}
