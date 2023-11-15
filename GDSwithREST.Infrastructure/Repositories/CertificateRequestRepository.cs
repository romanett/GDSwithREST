using GDSwithREST.Domain.Entities;
using GDSwithREST.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GDSwithREST.Domain.Repositories
{
    public class CertificateRequestRepository : ICertificateRequestRepository
    {
        private readonly GdsDbContext _context;
        public CertificateRequestRepository(GdsDbContext context)
        {
            _context = context;
        }
        public void RemoveCertificateRequests(CertificateRequest[] certificateRequests)
        {
            _context.CertificateRequests.RemoveRange(certificateRequests);
            _context.SaveChanges();
        }

        public async Task<CertificateRequest?> GetCertificateRequestById(Guid id)
        {
            return await _context.CertificateRequests.SingleOrDefaultAsync(x => x.RequestId == id);
        }

        /// <summary>
        /// persists the changes made to an CeritificateRequest instance
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
