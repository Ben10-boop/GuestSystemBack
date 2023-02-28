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
using GuestSystemBack.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Data;

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
                return NotFound("Employee with given ID does not exist");
            }

            return visitableEmployee;
        }

        // PUT: api/VisitableEmployees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}"), Authorize(Roles = "super, regular")]
        public async Task<IActionResult> PatchVisitableEmployee(int id, VisitableEmployeeDTO request)
        {
            var oldEmployee = await _context.VisitableEmployees.FindAsync(id);
            if (oldEmployee == null) return NotFound("Employee with given ID does not exist");

            if (request.Name != String.Empty) oldEmployee.Name = request.Name;
            if (request.Email != String.Empty) oldEmployee.Email = request.Email;
            await _context.SaveChangesAsync();

            return Ok(oldEmployee);
        }

        // POST: api/VisitableEmployees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = "super, regular")]
        public async Task<ActionResult<VisitableEmployee>> PostVisitableEmployee(VisitableEmployeeDTO request)
        {
            if (_context.VisitableEmployees == null)
            {
                return Problem("Entity set 'DataContext.VisitableEmployees'  is null.");
            }

            foreach (VisitableEmployee employee in _context.VisitableEmployees)
            {
                if (employee.Email == request.Email)
                {
                    return BadRequest("Employee with this email already exists");
                }
            }
            VisitableEmployee newEmployee = new()
            {
                Name = request.Name,
                Email = request.Email
            };
            _context.VisitableEmployees.Add(newEmployee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVisitableEmployee", new { id = newEmployee.Id }, newEmployee);
        }

        // DELETE: api/VisitableEmployees/5
        [HttpDelete("{id}"), Authorize(Roles = "super, regular")]
        public async Task<IActionResult> DeleteVisitableEmployee(int id)
        {
            var visitableEmployee = await _context.VisitableEmployees.FindAsync(id);
            if (visitableEmployee == null)
            {
                return NotFound("Employee with given ID does not exist");
            }

            bool hasBeenVisited = false;
            foreach(FormSubmission formSub in _context.FormSubmissions)
            {
                if (formSub.VisiteeId == id)
                {
                    hasBeenVisited = true;
                    break;
                }
            }

            if (hasBeenVisited)
            {
                visitableEmployee.Status = "unvisitable";
            }
            else
            {
                _context.VisitableEmployees.Remove(visitableEmployee);
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
