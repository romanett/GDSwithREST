using GDSwithREST.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithREST.Domain.Repositories
{
    public interface ITrustListRepository
    {
        public Task<IEnumerable<TrustList>> GetTrustListsByApplicationId(int id);
        public void RemoveTrustLists(TrustList[] trustLists);
        public TrustList AddTrustList(TrustList trustList);
        public void SaveChanges(TrustList trustList);
    }
}
