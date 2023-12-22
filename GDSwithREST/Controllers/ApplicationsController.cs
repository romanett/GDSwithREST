using Microsoft.AspNetCore.Mvc;
using Opc.Ua.Gds;
using Opc.Ua.Gds.Server.Database;
using Opc.Ua;
using Opc.Ua.Gds.Server;
using System.Security.Cryptography.X509Certificates;
using GDSwithREST.Domain.Repositories;
using GDSwithREST.Domain.ApiModels;
using Microsoft.IdentityModel.Tokens;
using GDSwithREST.Domain.Services;
using GDSwithREST.Infrastructure.Repositories;

namespace GDSwithREST.Controllers
{
    /// <summary>
    /// API Rout for getting Applications registered at the GDS
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationsDatabase _applicationsDatabase;
        /// <summary>
        /// Controller for all Endpoints having to do with registered OPC UA Applications of the GDS
        /// </summary>
        /// <param name="applicationRepository"></param>
        /// <param name="applicationsDatabase"></param>
        public ApplicationsController(IApplicationRepository applicationRepository, IApplicationsDatabase applicationsDatabase)
        {
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            _applicationsDatabase = applicationsDatabase ?? throw new ArgumentNullException(nameof(applicationsDatabase));
        }
        /// <summary>
        /// Returns all registered applications
        /// </summary>
        /// <returns></returns>
        // GET: /Applications
        [HttpGet]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, Type = typeof(ApplicationApiModel[]))]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicationApiModel[]>> GetApplications()
        {
          var applications = await _applicationRepository.GetAllApplications();
          if (applications.IsNullOrEmpty())
            {
                return NotFound();
            }
            var applicationsAsApiModel = applications.Select(x => new ApplicationApiModel(x)).ToArray();
            return Ok(applicationsAsApiModel);
        }
        /// <summary>
        /// Returns the specified Application
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Applications/5
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, Type = typeof(ApplicationApiModel))]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApplicationApiModel>> GetApplications(Guid id)
        {
            var application = await _applicationRepository.GetApplicationById(id);

            if (application == null)
            {
                return NotFound();
            }

            return Ok(new ApplicationApiModel(application));
        }
        /// <summary>
        /// Register a new Application at the GDS
        /// </summary>
        /// <param name="applicationRaw"></param>
        /// <returns></returns>
        // POST: /Applications/register
        [HttpPost("register")]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status201Created, Type = typeof(ApplicationApiModel))]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApplicationApiModel>> RegisterApplication([FromBody] ApplicationApiModel applicationRaw)
        {
            ArgumentNullException.ThrowIfNull(applicationRaw);

            var applicationName = new LocalizedTextCollection { 
                                    new LocalizedText("en-US", applicationRaw.ApplicationName)};
            var application = new ApplicationRecordDataType()
            {
                ApplicationId = applicationRaw.ApplicationId,
                ApplicationUri = applicationRaw.ApplicationUri,
                ApplicationType = (Opc.Ua.ApplicationType)applicationRaw.ApplicationType,
                ApplicationNames = applicationName,
                ProductUri = applicationRaw.ProductUri
            };
            var nodeId = _applicationsDatabase.RegisterApplication(application);
            
            var applicationId = (Guid)nodeId.Identifier;

            var applications = await _applicationRepository.GetApplicationById(applicationId);

            if (applications == null)
            {
                return Problem("Application Registration failed.");
            }
            return CreatedAtAction("GetApplications", $"applications/{applicationId}", new ApplicationApiModel(applications));
        }
        /// <summary>
        /// unregister an exisiting Application from the OPC UA GDS
        /// </summary>
        /// <param name="id">Guid of the Application to delete</param>
        /// <returns></returns>
        // DELETE: /Applications/5
        [HttpDelete("{id:Guid}/unregister")]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public async Task<IActionResult> DeleteApplications(Guid id, [FromServices] ICertificateGroupService certificateGroupService)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
            ArgumentNullException.ThrowIfNull(certificateGroupService);

            if (_applicationRepository.GetApplicationById(id).Result is null)
                return NotFound();

            try
            {
                if (_applicationsDatabase.GetApplicationCertificate(new NodeId(id, _applicationsDatabase.NamespaceIndex), nameof(Opc.Ua.ObjectTypeIds.ApplicationCertificateType), out var certificate))
                    await RevokeApplicationCertificate(certificate, certificateGroupService);
                if (_applicationsDatabase.GetApplicationCertificate(new NodeId(id, _applicationsDatabase.NamespaceIndex), nameof(Opc.Ua.ObjectTypeIds.HttpsCertificateType), out var httpsCertificate))
                    await RevokeApplicationCertificate(httpsCertificate, certificateGroupService);
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Failed to revoke Application Certificate");
                return Problem("Failed to revoke Application Certificate \n" + e.ToString());
            }

            var nodeId = new NodeId(id, _applicationsDatabase.NamespaceIndex);
            _applicationsDatabase.UnregisterApplication(nodeId);

            return Ok();
        }

        private static async Task RevokeApplicationCertificate(byte[]? certificate, ICertificateGroupService certificateGroupService)
        {
                if (certificate != null && certificate.Length > 0)
                {
                    ICertificateGroup? certificateGroup = null;
                    var x509 = new X509Certificate2(certificate);

                    foreach (var certificateGroups in certificateGroupService.CertificateGroups)
                    {
                        if (X509Utils.CompareDistinguishedName(certificateGroups.Certificate.Subject, x509.Issuer))
                        {
                            certificateGroup = certificateGroups;
                        }
                    }
                    if (certificateGroup != null)
                    {
                       await certificateGroup.RevokeCertificateAsync(x509).ConfigureAwait(false);
                    }
                }
            }
    }
}
