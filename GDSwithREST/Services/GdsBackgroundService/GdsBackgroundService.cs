namespace GDSwithREST.Services.GdsBackgroundService
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
