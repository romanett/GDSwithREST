using Opc.Ua;

namespace GDSwithREST.Domain.Services
{
    public interface IGdsService
    {
        /// <summary>
        /// returns the Endpoints of the OPC UA GDS Server
        /// </summary>
        public IEnumerable<string> GetEndpointURLs();

        public Task StartServer(CancellationToken stoppingToken);

        public void StopServer();
    }
}
