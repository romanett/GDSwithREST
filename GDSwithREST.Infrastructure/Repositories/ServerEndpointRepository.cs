using GDSwithREST.Domain.Entities;
using GDSwithREST.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GDSwithREST.Domain.Repositories
{
    public class ServerEndpointRepository: IServerEndpointRepository
    {
        private readonly GdsDbContext _context;
        public ServerEndpointRepository(GdsDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ServerEndpoint>> GetAllServerEndpoints()
        {
            return await _context.ServerEndpoints.ToListAsync();
        }
        public async Task<IEnumerable<ServerEndpoint>> GetServerEndpointsByApplicationId(int id)
        {
            return await _context.ServerEndpoints.Where(x =>  x.ApplicationId == id).ToListAsync();
        }
        public void RemoveServerEndpoints(ServerEndpoint[] serverEndpoints)
        {
            _context.ServerEndpoints.RemoveRange(serverEndpoints);
            _context.SaveChanges();
        }
        public ServerEndpoint AddServerEndpoint(ServerEndpoint serverEndpoint)
        {
            var entity = _context.ServerEndpoints.Add(serverEndpoint).Entity;
            _context.SaveChanges();
            return entity;
        }
    }
}
