using GDSwithREST.Domain.Entities;
using GDSwithREST.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithREST.Domain.Repositories
{
    public class CertificateStoreRepository:ICertificateStoreRepository
    {
        private readonly GdsDbContext _context;
        public CertificateStoreRepository(GdsDbContext context)
        {
            _context = context;
        }
        public async Task<CertificateStore?> GetCertificateStoreByPath(string path)
        {
            return await _context.CertificateStores.SingleOrDefaultAsync(x => x.Path == path);
        }
    }
}
