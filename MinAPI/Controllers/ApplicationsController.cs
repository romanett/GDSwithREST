
using GDSwithREST.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinAPI.Data;
using Opc.Ua.Gds;
using Opc.Ua.Gds.Server.Database;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Security.Cryptography.Xml;
using Opc.Ua;

namespace GDSwithREST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly GdsdbContext _context;
        private readonly IApplicationsDatabase _applicationsDatabase;

        public ApplicationsController(GdsdbContext context, IApplicationsDatabase applicationsDatabase)
        {
            _context = context;
            _applicationsDatabase = applicationsDatabase;
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
        [HttpPost("register")]
        public  async Task<ActionResult<Applications>> RegisterApplication(ApplicationRecordDataType application)
        {            
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
            var applications = _context.Applications.Single(x => x.ApplicationId == applicationID);

            if (applications == null)
            {
                return Problem("Application Registration failed.");
            }

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
            var nodeId = new NodeId(applications.ApplicationId);
            _applicationsDatabase.UnregisterApplication(nodeId);

            return NoContent();
        }

    }
}
