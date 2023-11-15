using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithREST.Domain.Repositories
{
    public interface IPersistencyRepository
    {
        public Task MigrateDatabaseAsync();
    }
}
