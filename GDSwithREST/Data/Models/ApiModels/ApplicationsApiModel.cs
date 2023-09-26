using System.Text.Json.Serialization;

namespace GDSwithREST.Data.Models.ApiModels
{
    public class ApplicationsApiModel
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationUri { get; set; } = null!;
        public string ApplicationName { get; set; } = null!;
        public int ApplicationType { get; set; }
        public string ProductUri { get; set; } = null!;

        //Certificate?
        public ApplicationsApiModel(Applications application)
        {
            ApplicationId = application.ApplicationId;
            ApplicationUri = application.ApplicationUri;
            ApplicationName = application.ApplicationName;
            ProductUri = application.ProductUri;
            ApplicationType = application.ApplicationType;
        }
        [JsonConstructor]
        public ApplicationsApiModel(Guid applicationId, string applicationUri, string applicationName, int applicationType, string productUri)
        {
            ApplicationId = applicationId;
            ApplicationUri = applicationUri;
            ApplicationName = applicationName;
            ApplicationType = applicationType;
            ProductUri = productUri;
        }
    }
}
