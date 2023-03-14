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
using GuestSystemBack.Interfaces;

namespace GuestSystemBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitableEmployeesController : ControllerBase
    {
        private readonly IVisitableEmployeeRepo _employeeRepo;

        public VisitableEmployeesController(IVisitableEmployeeRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        // GET: api/VisitableEmployees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisitableEmployee>>> GetVisitableEmployees()
        {
            return await _employeeRepo.GetEmployees();
        }

        // GET: api/VisitableEmployees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VisitableEmployee>> GetVisitableEmployee(int id)
        {
            var visitableEmployee = await _employeeRepo.GetEmployee(id);

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
            var oldEmployee = await _employeeRepo.GetEmployee(id);
            if (oldEmployee == null) return NotFound("Employee with given ID does not exist");

            if (request.Name != String.Empty) oldEmployee.Name = request.Name;
            if (request.Email != String.Empty) oldEmployee.Email = request.Email;
            await _employeeRepo.UpdateEmployee(oldEmployee);

            return Ok(oldEmployee);
        }

        // POST: api/VisitableEmployees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = "super, regular")]
        public async Task<ActionResult<VisitableEmployee>> PostVisitableEmployee(VisitableEmployeeDTO request)
        {
            if (!_employeeRepo.EmployeesExist())
            {
                return Problem("Entity set 'DataContext.VisitableEmployees'  is null.");
            }

            if(_employeeRepo.EmployeeWithEmailExists(request.Email))
                return BadRequest("Employee with this email already exists");

            VisitableEmployee newEmployee = new()
            {
                Name = request.Name,
                Email = request.Email
            };
            await _employeeRepo.AddEmployee(newEmployee);

            return CreatedAtAction("GetVisitableEmployee", new { id = newEmployee.Id }, newEmployee);
        }

        // DELETE: api/VisitableEmployees/5
        [HttpDelete("{id}"), Authorize(Roles = "super, regular")]
        public async Task<IActionResult> DeleteVisitableEmployee(int id)
        {
            var visitableEmployee = await _employeeRepo.GetEmployee(id);
            if (visitableEmployee == null)
            {
                return NotFound("Employee with given ID does not exist");
            }

            if (_employeeRepo.EmployeeHasBeenVisited(id))
            {
                visitableEmployee.Status = "unvisitable";
                await _employeeRepo.UpdateEmployee(visitableEmployee);
                return Ok(visitableEmployee);
            }

            await _employeeRepo.DeleteEmployee(visitableEmployee);
            return NoContent();
        }
    }
}
