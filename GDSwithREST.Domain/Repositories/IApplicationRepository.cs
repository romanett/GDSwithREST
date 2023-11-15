using GDSwithREST.Domain.Entities;

namespace GDSwithREST.Domain.Repositories
{
    public interface IApplicationRepository
    {
        public Task<IEnumerable<Application>> GetAllApplications();
        public Task<Application?> GetApplicationById(Guid id);
        public IQueryable<Application> GetApplicationsByUri(string ApplicationUri);
        public void RemoveApplication(Application application);
        public Application AddApplication(Application application);

        /// <summary>
        /// persists the changes made to an Application instance
        /// </summary>
        public void SaveChanges();
    }
}
