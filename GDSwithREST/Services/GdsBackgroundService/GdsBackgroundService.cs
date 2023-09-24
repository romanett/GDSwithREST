using Microsoft.Extensions.DependencyInjection;
using MinAPI.Data;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Gds.Server;
using System.Collections.ObjectModel;
using static System.Formats.Asn1.AsnWriter;

namespace MinAPI.Services.GdsBackgroundService
{
    public class GdsBackgroundService : BackgroundService
    {
        private readonly IGdsService _gdsService;
        public GdsBackgroundService(IGdsService gdsService)
        {
            _gdsService = gdsService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _gdsService.StartServer(stoppingToken);
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _gdsService.StopServer();

            await base.StopAsync(stoppingToken);
        }


    }
}
