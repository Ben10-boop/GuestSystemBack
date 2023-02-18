using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuestSystemBack.Data;
using GuestSystemBack.Models;
using Azure.Core;

namespace GuestSystemBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitableEmployeesController : ControllerBase
    {
        private readonly GuestSystemContext _context;

        public VisitableEmployeesController(GuestSystemContext context)
        {
            _context = context;
        }

        // GET: api/VisitableEmployees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisitableEmployee>>> GetVisitableEmployees()
        {
            return await _context.VisitableEmployees.ToListAsync();
        }

        // GET: api/VisitableEmployees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VisitableEmployee>> GetVisitableEmployee(int id)
        {
            var visitableEmployee = await _context.VisitableEmployees.FindAsync(id);

            if (visitableEmployee == null)
            {
                return NotFound();
            }

            return visitableEmployee;
        }

        // PUT: api/VisitableEmployees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchVisitableEmployee(int id, VisitableEmployee request)
        {
            var oldEmployee = await _context.VisitableEmployees.FindAsync(id);
            if (oldEmployee == null) return BadRequest("Can't find the Admin with given ID");

            if (request.Name != String.Empty) oldEmployee.Name = request.Name;
            if (request.Email != String.Empty) oldEmployee.Email = request.Email;
            await _context.SaveChangesAsync();

            return Ok(oldEmployee);
        }

        // POST: api/VisitableEmployees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VisitableEmployee>> PostVisitableEmployee(VisitableEmployee visitableEmployee)
        {
            _context.VisitableEmployees.Add(visitableEmployee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVisitableEmployee", new { id = visitableEmployee.Id }, visitableEmployee);
        }

        // DELETE: api/VisitableEmployees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisitableEmployee(int id)
        {
            var visitableEmployee = await _context.VisitableEmployees.FindAsync(id);
            if (visitableEmployee == null)
            {
                return NotFound();
            }

            _context.VisitableEmployees.Remove(visitableEmployee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VisitableEmployeeExists(int id)
        {
            return _context.VisitableEmployees.Any(e => e.Id == id);
        }
    }
}
