using System;
using System.Collections.Generic;

namespace GDSwithREST.Data.Models
{
    public partial class ServerEndpoints
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string DiscoveryUrl { get; set; } = null!;

        public Applications Application { get; set; } = null!;
    }
}
