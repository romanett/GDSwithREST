using Opc.Ua;

namespace GDSwithREST.Services.GdsBackgroundService
{
    public interface IGdsService
    {
        /// <summary>
        /// returns the Endpoints of the OPC UA GDS Server
        /// </summary>
        /// <returns cref="EndpointDescriptionCollection"><<returns>
        public IEnumerable<String> GetEndpointURLs();

        public Task StartServer(CancellationToken stoppingToken);

        public void StopServer();
    }
}
