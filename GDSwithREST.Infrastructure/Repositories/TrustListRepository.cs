using GDSwithREST.Domain.Entities;
using GDSwithREST.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GDSwithREST.Domain.Repositories
{
    public class TrustListRepository:ITrustListRepository
    {
        private readonly GdsDbContext _context;
        public TrustListRepository(GdsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrustList>> GetTrustListsByApplicationId(int id)
        {
            return await _context.TrustLists.Where(x => x.ApplicationId == id).ToListAsync();
        }

        public void RemoveTrustLists(TrustList[] trustLists)
        {
            _context.TrustLists.RemoveRange(trustLists);
            _context.SaveChanges();
        }

        public TrustList AddTrustList(TrustList trustList)
        {
            var entity = _context.TrustLists.Add(trustList).Entity;
            _context.SaveChanges();
            return entity;
        }


        public void SaveChanges(TrustList trustList)
        {
            _context.TrustLists.Update(trustList);
            _context.SaveChanges();
        }
    }
}
