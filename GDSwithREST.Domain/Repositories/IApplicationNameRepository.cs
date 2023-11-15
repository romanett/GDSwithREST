using GDSwithREST.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithREST.Domain.Repositories
{
    public interface IApplicationNameRepository
    {
        public Task<IEnumerable<ApplicationName>> GetAllApplicationNames();
        public Task<IEnumerable<ApplicationName>> GetApplicationNamesByApplicationId(int id);
        public void RemoveApplicationNames(ApplicationName[] applicationNames);
        public ApplicationName AddApplicationName(ApplicationName applicationName);
    }
}
