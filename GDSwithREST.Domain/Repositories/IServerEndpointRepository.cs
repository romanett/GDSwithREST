using GDSwithREST.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDSwithREST.Domain.Repositories
{
    public interface IServerEndpointRepository
    {
        public Task<IEnumerable<ServerEndpoint>> GetAllServerEndpoints();
        public Task<IEnumerable<ServerEndpoint>> GetServerEndpointsByApplicationId(int id);
        public void RemoveServerEndpoints(ServerEndpoint[] serverEndpoints);
        public ServerEndpoint AddServerEndpoint(ServerEndpoint serverEndpoint);
    }
}
