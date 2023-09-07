﻿using System;
using System.Collections.Generic;

namespace MinAPI.Models
{
    public partial class ServerEndpoints
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string DiscoveryUrl { get; set; }

        public Applications Application { get; set; }
    }
}
