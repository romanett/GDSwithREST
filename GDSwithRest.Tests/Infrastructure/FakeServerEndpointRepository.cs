using GDSwithREST.Domain.Entities;
using GDSwithREST.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithRest.Tests.Infrastructure
{
    internal class FakeServerEndpointRepository : IServerEndpointRepository
    {
        public ServerEndpoint AddServerEndpoint(ServerEndpoint serverEndpoint)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ServerEndpoint>> GetAllServerEndpoints()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ServerEndpoint>> GetServerEndpointsByApplicationId(int id)
        {
            //Todo Implement
            return Task.Run(() =>
            {
                return (IEnumerable<ServerEndpoint>)new List<ServerEndpoint>();
            });
        }

        public void RemoveServerEndpoints(ServerEndpoint[] serverEndpoints)
        {
            //Todo Implement
            return;
        }
    }
}
