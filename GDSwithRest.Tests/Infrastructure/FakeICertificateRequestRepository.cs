using GDSwithREST.Domain.Entities;
using GDSwithREST.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithRest.Tests.Infrastructure
{
    internal class FakeICertificateRequestRepository : ICertificateRequestRepository
    {
        public Task<CertificateRequest?> GetCertificateRequestById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void RemoveCertificateRequests(CertificateRequest[] certificateRequests)
        {
            //ToDo Implement
            return;
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
