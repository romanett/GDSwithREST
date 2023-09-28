
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
    [Route("[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly GdsDbContext _context;
        private readonly IApplicationsDatabase _applicationsDatabase;
        private readonly ICertificateGroupDb _certificatesDatabase;

        public ApplicationsController(GdsDbContext context, IApplicationsDatabase applicationsDatabase, ICertificateGroupDb certificatesDatabase)
        {
            _context = context;
            _applicationsDatabase = applicationsDatabase;
            _certificatesDatabase = certificatesDatabase;
        }

        // GET: /Applications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationApiModel>>> GetApplications()
        {
          if (_context.Applications == null)
          {
              return NotFound();
          }
          var applications = await _context.Applications.ToListAsync();
            var applicationsAsApiModel =
               from application in applications
               select new ApplicationApiModel(application);
            return Ok(applicationsAsApiModel);
        }

        // GET: /Applications/5
        [HttpGet("{id:Guid}")]
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

        // POST: /Applications/register
        [HttpPost("register")]
        public async Task<ActionResult<Applications>> RegisterApplication([FromBody] ApplicationApiModel applicationRaw)
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
                ApplicationType = (ApplicationType)applicationRaw.ApplicationType,
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

        // DELETE: /Applications/5
        [HttpDelete("{id:Guid}/unregister")]
        public async Task<IActionResult> DeleteApplications(Guid id)
        {
            if (_context.Applications == null || _applicationsDatabase == null)
            {
                return NotFound();
            }
            try
            {
                byte[] certificate;
                if (_applicationsDatabase.GetApplicationCertificate(id, nameof(Opc.Ua.ObjectTypeIds.ApplicationCertificateType), out certificate))
                {
                    if (certificate != null && certificate.Length > 0)
                    {
                            ICertificateGroup certificateGroup = new CertificateGroup();
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
                                }
                                catch (Exception)
                                {
                                    //failed to delete certificate
                                }
                            }
                    }
                }
            }
            catch
            {
                //failed to delete certificate
            }
            //ToDo Revoke Certificate
            if (_context.Applications == null || _applicationsDatabase == null)
            {
                return NotFound();
            }
            var applications =  await _context.Applications.SingleOrDefaultAsync(x => x.ApplicationId == id);
            if (applications == null)
            {
                return NotFound();
            }
            var nodeId = new NodeId(applications.ApplicationId);
            _applicationsDatabase.UnregisterApplication(nodeId);

            return NoContent();
        }

    }
}
