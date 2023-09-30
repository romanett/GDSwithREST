using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace GDSwithREST.Data.Models.ApiModels
{
    public class X509CertificateApiModel
    {
        public string Subject { get; set; }
        public string Thumbprint { get; set; }

        public string SerialNumber { get; set; }
        public DateTime? NotBefore { get; set; }

        public DateTime? NotAfter { get; set; }

        public string Certificate { get; set; }

        public X509CertificateApiModel(X509Certificate2 certificate)
        {
            Certificate = certificate.ExportCertificatePem();
            Thumbprint = certificate.Thumbprint;
            SerialNumber = certificate.SerialNumber;
            NotBefore = certificate.NotBefore;
            NotAfter = certificate.NotAfter;
            Subject = certificate.Subject;
        }

        public X509Certificate2 ToServiceModel()
        {
            return X509Certificate2.CreateFromPem(Certificate);
        }
    }
}
