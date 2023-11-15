using GDSwithREST.Domain.ApiModels;
using GDSwithREST.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace GDSwithREST.Controllers
{
    /// <summary>
    /// API Route to return Information about Certificate Groups
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CertificateGroupsController : ControllerBase
    {
        private readonly ICertificateGroupService _certificatesDatabase;

        public CertificateGroupsController(ICertificateGroupService certificates)
        {
            _certificatesDatabase = certificates;
        }
        /// <summary>
        /// Returns all Certificate Groups of the GDS
        /// </summary>
        /// <returns></returns>
        // GET: /CertificateGroups
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CertificateGroupApiModel[]))]
        public ActionResult<CertificateGroupApiModel[]> GetCertificateGroups()
        {
            var certificateGroups =
                from certificateGroup in _certificatesDatabase.CertificateGroups
                select new CertificateGroupApiModel(certificateGroup);
            return Ok(certificateGroups.ToArray());
        }
        /// <summary>
        /// Returns the CA Certificate of the specified Certificate Group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /CertificateGroups/5/ca
        [HttpGet("{id:int}/ca")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(X509CertificateApiModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// <summary>
        /// Returns the TrustList of the specified certificate Group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /CertificateGroups/5/trustlist
        [HttpGet("{id:int}/trustlist")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(X509CertificateApiModel[]))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<X509CertificateApiModel[]>> GetCertificateGroupTrustList(uint id)
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
            return  Ok(trustList.ToArray());
        }
        /// <summary>
        /// Regenerate the CA Certificate of the specified Certificate Group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: /CertificateGroup/5/ca
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id:int}/ca")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(X509CertificateApiModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<X509CertificateApiModel>> PostCertificateGroupCA(uint id)
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
            await certificateGroup.CreateCACertificateAsync(certificateGroup.Configuration.SubjectName);

            return Ok(new X509CertificateApiModel(certificateGroup.Certificate));
        }
        /// <summary>
        /// revoke the specified Certifice in the specified Certificate Group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="certPemRaw"></param>
        /// <returns></returns>
        // DELETE: /CertificateGroup/5/cert
        [HttpDelete("{id:int}/cert/revoke")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevokeCertificateGroupCert(uint id, [FromBody] JsonElement certPemRaw)
        {
            var certPem = certPemRaw.ToString();
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
