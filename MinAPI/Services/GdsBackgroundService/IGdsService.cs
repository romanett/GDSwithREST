using Opc.Ua;
using System.Collections.ObjectModel;

namespace MinAPI.Services.GdsBackgroundService
{
    public interface IGdsService
    {
        /// <summary>
        /// returns the Endpoints of the OPC UA GDS Server
        /// </summary>
        /// <returns cref="EndpointDescriptionCollection"><<returns>
        public ReadOnlyCollection<EndpointDescription> GetEndpoints();

        public Task StartServer(CancellationToken stoppingToken);

        public void StopServer();
    }
}
