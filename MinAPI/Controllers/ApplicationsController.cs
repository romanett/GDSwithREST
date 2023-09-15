using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinAPI.Data;
using MinAPI.Models;

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

        // GET: api/Applications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Applications>>> GetApplications()
        {
          if (_context.Applications == null)
          {
              return NotFound();
          }
            return await _context.Applications.ToListAsync();
        }

        // GET: api/Applications/5
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

        // PUT: api/Applications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplications(int id, Applications applications)
        {
            if (id != applications.Id)
            {
                return BadRequest();
            }

            _context.Entry(applications).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Applications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
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

        // DELETE: api/Applications/5
        [HttpDelete("{id}")]
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
