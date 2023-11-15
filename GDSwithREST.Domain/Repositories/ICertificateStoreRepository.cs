using GDSwithREST.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithREST.Domain.Repositories
{
    public interface ICertificateStoreRepository
    {
        public Task<CertificateStore?> GetCertificateStoreByPath(string path);
    }
}
