using GDSwithREST.Domain.Entities;
using GDSwithREST.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Application = GDSwithREST.Domain.Entities.Application;

namespace GDSwithRest.Tests.Infrastructure
{
    internal class FakeApplicationRepository : IApplicationRepository
    {
        private readonly List<Application> _applications = new(){
            new Application() {ApplicationId= Guid.NewGuid(), ApplicationName = "TestApplication1", ApplicationType= 1 },
            new Application() {ApplicationId= Guid.NewGuid(), ApplicationName = "TestApplication2", ApplicationType= 2 },
            new Application() {ApplicationId= Guid.NewGuid(), ApplicationName = "TestApplication3", ApplicationType= 0 }
        };

        public List<Application> Applications => _applications;

        public Application AddApplication(Application application)
        {
             Applications.Add(application);
            return application;
        }

        public Task<IEnumerable<Application>> GetAllApplications()
        {
            return Task.FromResult(Applications.AsEnumerable());
        }

        public Task<Application?> GetApplicationById(Guid id)
        {
            return Task.FromResult(Applications.SingleOrDefault(y => y.ApplicationId == id));
        }

        public IQueryable<Application> GetApplicationsByUri(string ApplicationUri)
        {
            return Applications.Where(y => y.ApplicationUri == ApplicationUri).AsQueryable();
        }

        public void RemoveApplication(Application application)
        {
            Applications.Remove(application);
        }

        public void SaveChanges()
        {
            return;
        }
    }
}
