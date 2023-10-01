﻿using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace GDSwithREST.Data.Models.ApiModels
{
    public class ApplicationApiModel
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationUri { get; set; } = null!;
        public string ApplicationName { get; set; } = null!;
        public int ApplicationType { get; set; }
        public string ProductUri { get; set; } = null!;

        public string? Certificate { get;set; }

        //Certificate?
        public ApplicationApiModel(Applications application)
        {
            ApplicationId = application.ApplicationId;
            ApplicationUri = application.ApplicationUri;
            ApplicationName = application.ApplicationName;
            ProductUri = application.ProductUri;
            ApplicationType = application.ApplicationType;
            Certificate = new X509Certificate2(application.Certificate).ExportCertificatePem();
        }
        [JsonConstructor]
        public ApplicationApiModel(Guid applicationId, string applicationUri, string applicationName, int applicationType, string productUri)
        {
            ApplicationId = applicationId;
            ApplicationUri = applicationUri;
            ApplicationName = applicationName;
            ApplicationType = applicationType;
            ProductUri = productUri;
        }
    }
}
