using Opc.Ua.Gds.Server;
using Opc.Ua;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using GDSwithREST.Services.GdsBackgroundService.Databases;
using System.Runtime.Serialization;

namespace GDSwithREST.Data.Models.ApiModels
{
    /// <summary>
    /// Certificate Group of the GDS
    /// </summary>
    public class CertificateGroupApiModel
    {
        public CertificateGroupApiModel(CertificateGroup certificateGroup)
        {
            try
            {
                Id = (CertificateGroupType)(uint)certificateGroup.Id.Identifier;
                UpdateRequired = certificateGroup.UpdateRequired;
                Ceritificate = new X509CertificateApiModel(certificateGroup.Certificate);
            }
            catch
            {
            }
        }
        public CertificateGroupApiModel(uint id, X509Certificate2 certificate, bool updateRequired)
        {
            try
            {
                Id = (CertificateGroupType)id;
                UpdateRequired = updateRequired;
                Ceritificate = new X509CertificateApiModel(certificate);
            }
            catch
            {
            }
        }

        public CertificateGroupType Id { get; set; }
        public bool UpdateRequired { get; set; }
        public X509CertificateApiModel? Ceritificate { get; set; }        
    }
    
    /// <summary>
    /// Type of the Certificate Group
    /// </summary>
    public enum CertificateGroupType
    {
        [EnumMember(Value = "DefaultApplicationGroup_615")]
        DefaultApplicationGroup = 615,
        [EnumMember(Value = "DefaultHttpsGroup_649")]
        DefaultHttpsGroup = 649,
        [EnumMember(Value = "DefaultUserTokenGroup_683")]
        DefaultUserTokenGroup = 683,
    }
}
