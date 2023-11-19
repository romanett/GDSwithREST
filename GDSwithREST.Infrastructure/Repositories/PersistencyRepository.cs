using GDSwithREST.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithREST.Infrastructure.Repositories
{
    public class PersistencyRepository:IPersistencyRepository
    {
        private readonly GdsDbContext _context;
        public PersistencyRepository(GdsDbContext context)
        {
            _context = context;
        }

        public void MigrateDatabase()
        {
            _context.Database.Migrate();
        }
    }
}
