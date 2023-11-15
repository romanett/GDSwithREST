using GDSwithREST.Domain.Entities;

namespace GDSwithREST.Domain.Repositories
{
    public interface ICertificateRequestRepository
    {
        public void RemoveCertificateRequests(CertificateRequest[] certificateRequests);

        public Task<CertificateRequest?> GetCertificateRequestById(Guid id);

        /// <summary>
        /// persists the changes made to an CeritificateRequest instance
        /// </summary>
        public void SaveChanges();
    }
}
