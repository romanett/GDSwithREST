using GDSwithREST.Domain.Entities;
using GDSwithREST.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GDSwithREST.Domain.Repositories
{
    public class ApplicationNameRepository:IApplicationNameRepository
    {
        private readonly GdsDbContext _context;
        public ApplicationNameRepository(GdsDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ApplicationName>> GetAllApplicationNames()
        {
            return await _context.ApplicationNames.ToListAsync();
        }
        public async Task<IEnumerable<ApplicationName>> GetApplicationNamesByApplicationId(int id)
        {
            return await _context.ApplicationNames.Where(x => x.ApplicationId == id).ToListAsync();
        }
        public void RemoveApplicationNames(ApplicationName[] applicationNames)
        {
            _context.ApplicationNames.RemoveRange(applicationNames);
            _context.SaveChanges();
        }
        public ApplicationName AddApplicationName(ApplicationName applicationName)
        {
            var entity = _context.ApplicationNames.Add(applicationName).Entity;
            _context.SaveChanges();
            return entity;
        }
    }
}
