using GDSwithREST.Domain.Entities;
using GDSwithREST.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithREST.Infrastructure.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly GdsDbContext _context;
        public ApplicationRepository(GdsDbContext context)
        {
            _context = context;
        }

        public Application AddApplication(Application application)
        {
            var entity =  _context.Applications.Add(application).Entity;
            _context.SaveChanges();
            return entity;
        }

        public async Task<IEnumerable<Application>> GetAllApplications()
        {
            return await _context.Applications.ToListAsync();
        }

        public async Task<Application?> GetApplicationById(Guid id)
        {
            return await _context.Applications.SingleOrDefaultAsync(x => x.ApplicationId == id);
        }

        public IQueryable<Application> GetApplicationsByUri(string applicationUri)
        {
            return _context.Applications.Where(x => x.ApplicationUri == applicationUri);
        }

        public void RemoveApplication(Application application)
        {
            _context.Applications.Remove(application);
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
