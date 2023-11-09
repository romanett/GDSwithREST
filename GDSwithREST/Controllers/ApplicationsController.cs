
using GDSwithREST.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GDSwithREST.Data;
using Opc.Ua.Gds;
using Opc.Ua.Gds.Server.Database;
using Opc.Ua;
using GDSwithREST.Services.GdsBackgroundService.Databases;
using Opc.Ua.Gds.Server;
using System.Security.Cryptography.X509Certificates;
using GDSwithREST.Data.Models.ApiModels;

namespace GDSwithREST.Controllers
{
    /// <summary>
    /// API Rout for getting Applications registered at the GDS
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly GdsDbContext _context;
        private readonly IApplicationsDatabase _applicationsDatabase;
        private readonly ICertificateGroupDb _certificatesDatabase;
        /// <summary>
        /// Controller for all Endpoints having to do with registered OPC UA Applications of the GDS
        /// </summary>
        /// <param name="context"></param>
        /// <param name="applicationsDatabase"></param>
        /// <param name="certificatesDatabase"></param>
        public ApplicationsController(GdsDbContext context, IApplicationsDatabase applicationsDatabase, ICertificateGroupDb certificatesDatabase)
        {
            _context = context;
            _applicationsDatabase = applicationsDatabase;
            _certificatesDatabase = certificatesDatabase;
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
          if (_context.Applications == null)
          {
              return NotFound();
          }
          var applications = await _context.Applications.ToListAsync();
            var applicationsAsApiModel =
               from application in applications
               select new ApplicationApiModel(application);
            return Ok(applicationsAsApiModel.ToArray());
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
            if (_context.Applications == null)
            {
                return NotFound();
            }
            var application = await _context.Applications.SingleOrDefaultAsync(x => x.ApplicationId == id);

            if (application == null)
            {
                return NotFound();
            }

            return new ApplicationApiModel(application);
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
            if (_applicationsDatabase == null)
            {
                return Problem("Application Registration failed.");
            }
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
            if (nodeId == null)
            {
                return Problem("Application Registration failed.");
            }
            var applicationID = new Guid(nodeId.Identifier.ToString()!);
            if (_context.Applications == null)
            {
                return Problem("Application Registration failed.");
            }
            var applications = await _context.Applications.SingleOrDefaultAsync(x => x.ApplicationId == applicationID);

            if (applications == null)
            {
                return Problem("Application Registration failed.");
            }

            return CreatedAtAction("GetApplications", new { id = applications.ApplicationId }, new ApplicationApiModel(applications));
        }
        /// <summary>
        /// unregister an exisiting Application from the OPC UA GDS
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: /Applications/5
        [HttpDelete("{id:Guid}/unregister")]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteApplications(Guid id)
        {
            if (_context.Applications == null || _applicationsDatabase == null)
            {
                return NotFound();
            }
            var certificateDeleted = false;
            try
            {
                byte[] certificate;
                if (_applicationsDatabase.GetApplicationCertificate(new NodeId(id), nameof(Opc.Ua.ObjectTypeIds.ApplicationCertificateType), out certificate))
                {
                    if (certificate != null && certificate.Length > 0)
                    {
                            ICertificateGroup? certificateGroup = null;
                                var x509 = new X509Certificate2(certificate);

                                foreach (var certificateGroups in _certificatesDatabase.CertificateGroups)
                                {
                                    if (X509Utils.CompareDistinguishedName(certificateGroups.Certificate.Subject, x509.Issuer))
                                    {
                                        certificateGroup = certificateGroups;
                                    }
                                }

                        if (certificateGroup != null)
                            {
                                try
                                {
                                    
                                    await _certificatesDatabase.RevokeCertificateAsync(x509).ConfigureAwait(false);
                                    certificateDeleted = true;
                            }
                                catch
                                {
                                certificateDeleted = false;
                                }
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e, "Failed to revoke Application Certificate");
                certificateDeleted = false;
            }
            var applications =  await _context.Applications.SingleOrDefaultAsync(x => x.ApplicationId == id);
            if (applications == null)
            {
                return NotFound();
            }
            var nodeId = new NodeId(applications.ApplicationId);
            _applicationsDatabase.UnregisterApplication(nodeId);

            return Ok("Certificate revoked:" + certificateDeleted);
        }

    }
}
