
using GDSwithREST.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinAPI.Data;

namespace GDSwithREST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CertificateGroupsController : ControllerBase
    {
        private readonly GdsdbContext _context;

        public CertificateGroupsController(GdsdbContext context)
        {
            _context = context;
        }

        // GET: /CertificateGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Applications>>> GetCertificateGroups()
        {
          if (_context.Applications == null)
          {
              return NotFound();
          }
            return await _context.Applications.ToListAsync();
        }

        // GET: /CertificateGroups/5/ca
        [HttpGet("{id}/ca")]
        public async Task<ActionResult<Applications>> GetCertificateGroupCA(int id)
        {
          if (_context.Applications == null)
          {
              return NotFound();
          }
            var applications = await _context.Applications.FindAsync(id);

            if (applications == null)
            {
                return NotFound();
            }

            return applications;
        }

        // GET: /CertificateGroups/5/trustlist
        [HttpGet("{id}/trustlist")]
        public async Task<ActionResult<Applications>> GetCertificateGroupTrustList(int id)
        {
            if (_context.Applications == null)
            {
                return NotFound();
            }
            var applications = await _context.Applications.FindAsync(id);

            if (applications == null)
            {
                return NotFound();
            }

            return applications;
        }

        // POST: /CertificateGroup/5/ca
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}/ca")]
        public async Task<ActionResult<Applications>> PostCertificateGroupCA(int id, Applications applications)
        {
          if (_context.Applications == null)
          {
              return Problem("Entity set 'GdsdbContext.Applications'  is null.");
          }
            _context.Applications.Add(applications);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplications", new { id = applications.Id }, applications);
        }

        // DELETE: /CertificateGroup/5/cert
        [HttpDelete("{id}/cert")]
        public async Task<IActionResult> DeleteCertificateGroupCert(int id)
        {
            if (_context.Applications == null)
            {
                return NotFound();
            }
            var applications = await _context.Applications.FindAsync(id);
            if (applications == null)
            {
                return NotFound();
            }

            _context.Applications.Remove(applications);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
