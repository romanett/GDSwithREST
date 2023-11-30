using GDSwithREST.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace GDSwithRest.Tests.Infrastructure
{
    internal static class DIService
    {
        public static IServiceScopeFactory GetServiceScopeFactory(IApplicationRepository inject)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped(sp => inject);
            var serviceScopeFactory = serviceCollection.BuildServiceProvider().GetService<IServiceScopeFactory>();

            if (serviceScopeFactory is null)
                throw new TestCanceledException("Could not intialite DI Service");

            return serviceScopeFactory;
        }
        public static IServiceScopeFactory GetServiceScopeFactory(IApplicationRepository inject, IServerEndpointRepository inject2, IApplicationNameRepository inject3)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped(sp => inject);
            serviceCollection.AddScoped(sp => inject2);
            serviceCollection.AddScoped(sp => inject3);
            var serviceScopeFactory = serviceCollection.BuildServiceProvider().GetService<IServiceScopeFactory>();

            if (serviceScopeFactory is null)
                throw new TestCanceledException("Could not intialite DI Service");

            return serviceScopeFactory;
        }
        public static IServiceScopeFactory GetServiceScopeFactory(IApplicationRepository inject, IServerEndpointRepository inject2, 
            IApplicationNameRepository inject3, ICertificateRequestRepository inject4)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped(sp => inject);
            serviceCollection.AddScoped(sp => inject2);
            serviceCollection.AddScoped(sp => inject3);
            serviceCollection.AddScoped(sp => inject4);
            var serviceScopeFactory = serviceCollection.BuildServiceProvider().GetService<IServiceScopeFactory>();

            if (serviceScopeFactory is null)
                throw new TestCanceledException("Could not intialite DI Service");

            return serviceScopeFactory;
        }
    }
}
