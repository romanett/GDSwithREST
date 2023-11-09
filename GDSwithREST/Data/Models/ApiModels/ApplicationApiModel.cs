using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace GDSwithREST.Data.Models.ApiModels
{
    /// <summary>
    /// A GDS registered Application
    /// </summary>
    public class ApplicationApiModel
    {
        /// <summary>
        /// The Guid identifying the Application
        /// </summary>
        public Guid ApplicationId { get; set; }
        /// <summary>
        /// The Uri of the Application
        /// </summary>
        public string ApplicationUri { get; set; } = null!;
        /// <summary>
        /// The name of the Application
        /// </summary>
        public string ApplicationName { get; set; } = null!;
        public int ApplicationType { get; set; }
        public string ProductUri { get; set; } = null!;
        /// <summary>
        /// The gds signed certificate of the Application
        /// </summary>
        public X509CertificateApiModel? Certificate { get;set; }

        
        public ApplicationApiModel(Applications application)
        {
            ApplicationId = application.ApplicationId;
            ApplicationUri = application.ApplicationUri;
            ApplicationName = application.ApplicationName;
            ProductUri = application.ProductUri;
            ApplicationType = application.ApplicationType;
            if(application.Certificate.Length > 0)
                Certificate = new X509CertificateApiModel(new X509Certificate2(application.Certificate));
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
