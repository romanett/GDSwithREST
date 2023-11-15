using System.Security.Cryptography.X509Certificates;

namespace GDSwithREST.Domain.ApiModels
{
    /// <summary>
    /// A X509 Certificate
    /// </summary>
    public sealed class X509CertificateApiModel
    {
        public string Subject { get; set; }
        public string Thumbprint { get; set; }

        public string SerialNumber { get; set; }
        public DateTime? NotBefore { get; set; }

        public DateTime? NotAfter { get; set; }

        public string Certificate { get; set; }

        public string Issuer { get; set; }

        public X509CertificateApiModel(X509Certificate2 certificate)
        {
            Certificate = certificate.ExportCertificatePem();
            Thumbprint = certificate.Thumbprint;
            SerialNumber = certificate.SerialNumber;
            NotBefore = certificate.NotBefore;
            NotAfter = certificate.NotAfter;
            Subject = certificate.Subject;
            Issuer = certificate.Issuer;
        }

        public X509Certificate2 ToServiceModel()
        {
            return X509Certificate2.CreateFromPem(Certificate);
        }
    }
}
