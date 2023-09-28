
using GDSwithREST.Data.Models;
using GDSwithREST.Data.Models.ApiModels;
using GDSwithREST.Services.GdsBackgroundService.Databases;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

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
        public ActionResult<IEnumerable<CertificateGroupApiModel>> GetCertificateGroups()
        {
            var certificateGroups =
                from certificateGroup in _certificatesDatabase.CertificateGroups
                select new CertificateGroupApiModel(certificateGroup);
            return Ok(certificateGroups);
        }
        
        // GET: /CertificateGroups/5/ca
        [HttpGet("{id:int}/ca")]
        public ActionResult<X509CertificateApiModel> GetCertificateGroupCA(uint id)
        {
            if (_certificatesDatabase == null)
            {
                return NotFound();
            }
            var certificateGroup = _certificatesDatabase.CertificateGroups.SingleOrDefault(x => (uint)x.Id.Identifier == id);

            if (certificateGroup == null)
            {
                return NotFound();
            }

            return new X509CertificateApiModel(certificateGroup.Certificate);
        }
        
        // GET: /CertificateGroups/5/trustlist
        [HttpGet("{id:int}/trustlist")]
        public async Task<ActionResult<IEnumerable<X509CertificateApiModel>>> GetCertificateGroupTrustList(uint id)
        {
            if (_certificatesDatabase == null)
            {
                return NotFound();
            }
            var certificateGroup = _certificatesDatabase.CertificateGroups.SingleOrDefault(x => (uint)x.Id.Identifier == id);

            if (certificateGroup == null)
            {
                return NotFound();
            }
            var trustedCertificatesCollection = await _certificatesDatabase.GetTrustList(certificateGroup);
            var trustList =
                from cert in trustedCertificatesCollection
                select new X509CertificateApiModel(cert);
            return  Ok(trustList);
        }
        
        // POST: /CertificateGroup/5/ca
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id:int}/ca")]
        public async Task<ActionResult<Applications>> PostCertificateGroupCA(uint id, [FromBody] JsonElement subjectNameRaw)
        {
            var subjectName = subjectNameRaw.GetRawText();
            if (_certificatesDatabase == null)
            {
                return NotFound();
            }
            var certificateGroup = _certificatesDatabase.CertificateGroups.SingleOrDefault(x => (uint)x.Id.Identifier == id);
            if (certificateGroup == null)
            {
                return NotFound();
            }
            await certificateGroup.CreateCACertificateAsync(subjectName);

            return CreatedAtAction("RecreatedCA", new { id = certificateGroup.Id.Identifier }, new X509CertificateApiModel( certificateGroup.Certificate));
        }
        
        // DELETE: /CertificateGroup/5/cert
        [HttpDelete("{id:int}/cert")]
        public async Task<IActionResult> RevokeCertificateGroupCert(uint id, [FromBody] JsonElement certPemRaw)
        {
            var certPem = certPemRaw.GetRawText();
            if (_certificatesDatabase == null)
            {
                return NotFound();
            }
            var certificateGroup = _certificatesDatabase.CertificateGroups.SingleOrDefault(x => (uint)x.Id.Identifier == id);
            if (certificateGroup == null)
            {
                return NotFound();
            }
            await certificateGroup.RevokeCertificateAsync(X509Certificate2.CreateFromPem(certPem));

            return NoContent();
        }
    }
}
