using Opc.Ua;
using System.Collections.ObjectModel;

namespace MinAPI.Services.GdsBackgroundService
{
    public interface IGdsBackgroundService
    {
        /// <summary>
        /// returns the Endpoints of the OPC UA GDS Server
        /// </summary>
        /// <returns cref="EndpointDescriptionCollection"><<returns>
        public ReadOnlyCollection<EndpointDescription> GetEndpoints();
    }
}
