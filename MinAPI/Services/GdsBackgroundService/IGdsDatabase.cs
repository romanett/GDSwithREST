using Opc.Ua.Gds.Server;
using Opc.Ua.Gds.Server.Database;

namespace GDSwithREST.Services.GdsBackgroundService
{
    public interface IGdsDatabase: IApplicationsDatabase, ICertificateRequest
    {
        public new void Initialize();
    }
}
