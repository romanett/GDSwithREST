
using GDSwithREST.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinAPI.Data;

namespace GDSwithREST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly GdsdbContext _context;

        public ApplicationsController(GdsdbContext context)
        {
            _context = context;
        }

        // GET: /Applications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Applications>>> GetApplications()
        {
          if (_context.Applications == null)
          {
              return NotFound();
          }
            return await _context.Applications.ToListAsync();
        }

        // GET: /Applications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Applications>> GetApplications(int id)
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

        // POST: /Applications/register
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<Applications>> PostApplications(Applications applications)
        {
          if (_context.Applications == null)
          {
              return Problem("Entity set 'GdsdbContext.Applications'  is null.");
          }
            _context.Applications.Add(applications);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplications", new { id = applications.Id }, applications);
        }

        // DELETE: /Applications/5
        [HttpDelete("{id}/unregister")]
        public async Task<IActionResult> DeleteApplications(int id)
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

        private bool ApplicationsExists(int id)
        {
            return (_context.Applications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
