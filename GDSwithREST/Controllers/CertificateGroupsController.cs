
using GDSwithREST.Data.Models;
using GDSwithREST.Data.Models.ApiModels;
using GDSwithREST.Services.GdsBackgroundService.Databases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinAPI.Data;
using Opc.Ua;
using Opc.Ua.Gds.Server;
using System.Security.Cryptography.X509Certificates;

namespace GDSwithREST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CertificateGroupsController : ControllerBase
    {
        private readonly ICertificateGroupDb _certificatesDatabase;

        public CertificateGroupsController(ICertificateGroupDb certificates)
        {
            _certificatesDatabase = certificates;
        }

        // GET: /CertificateGroups
        [HttpGet]
        public ActionResult<IEnumerable<CertificateGroup>> GetCertificateGroups()
        {
            var certificateGroups =
                from certificateGroup in _certificatesDatabase.CertificateGroups
                select new CertificateGroupApiModel(certificateGroup);
            return Ok(certificateGroups);
        }

        // GET: /CertificateGroups/5/ca
        [HttpGet("{id:Guid}/ca")]
        public ActionResult<X509Certificate2> GetCertificateGroupCA(Guid id)
        {
            if (_certificatesDatabase == null)
            {
                return NotFound();
            }
            var certificateGroup = _certificatesDatabase.CertificateGroups.SingleOrDefault(x => x.Id == id);

            if (certificateGroup == null)
            {
                return NotFound();
            }

            return certificateGroup.Certificate;
        }

        // GET: /CertificateGroups/5/trustlist
        [HttpGet("{id:Guid}/trustlist")]
        public ActionResult<TrustListState> GetCertificateGroupTrustList(Guid id)
        {
            if (_certificatesDatabase == null)
            {
                return NotFound();
            }
            var certificateGroup = _certificatesDatabase.CertificateGroups.SingleOrDefault(x => x.Id == id);

            if (certificateGroup == null)
            {
                return NotFound();
            }

            return certificateGroup.DefaultTrustList;
        }

        // POST: /CertificateGroup/5/ca
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id:Guid}/ca")]
        public async Task<ActionResult<Applications>> PostCertificateGroupCA(Guid id, [FromBody] string subjectName)
        {
            if (_certificatesDatabase == null)
            {
                return NotFound();
            }
            var certificateGroup = _certificatesDatabase.CertificateGroups.SingleOrDefault(x => x.Id == id);
            if (certificateGroup == null)
            {
                return NotFound();
            }
            await certificateGroup.CreateCACertificateAsync(subjectName);

            return CreatedAtAction("GetApplications", new { id = certificateGroup.Id }, certificateGroup.Certificate);
        }

        // DELETE: /CertificateGroup/5/cert
        [HttpDelete("{id:Guid}/cert")]
        public async Task<IActionResult> DeleteCertificateGroupCert(Guid id, [FromBody] X509Certificate2 cert)
        {
            if (_certificatesDatabase == null)
            {
                return NotFound();
            }
            var certificateGroup = _certificatesDatabase.CertificateGroups.SingleOrDefault(x => x.Id == id);
            if (certificateGroup == null)
            {
                return NotFound();
            }
            await certificateGroup.RevokeCertificateAsync(cert);

            return NoContent();
        }

    }
}
