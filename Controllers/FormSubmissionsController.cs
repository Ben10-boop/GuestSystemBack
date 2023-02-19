using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuestSystemBack.Data;
using GuestSystemBack.Models;
using GuestSystemBack.DTOs;

namespace GuestSystemBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormSubmissionsController : ControllerBase
    {
        private readonly GuestSystemContext _context;

        public FormSubmissionsController(GuestSystemContext context)
        {
            _context = context;
        }

        // GET: api/FormSubmissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FormSubmission>>> GetFormSubmissions()
        {
            return await _context.FormSubmissions.ToListAsync();
        }

        // GET: api/FormSubmissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FormSubmission>> GetFormSubmission(int id)
        {
            var formSubmission = await _context.FormSubmissions.FindAsync(id);

            if (formSubmission == null)
            {
                return NotFound("Form submission with given ID does not exist");
            }

            return formSubmission;
        }

        // PUT: api/FormSubmissions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchFormSubmission(int id, FormSubmissionDTO request)
        {
            var oldSubmission = await _context.FormSubmissions.FindAsync(id);
            string errors = "";
            if (oldSubmission == null) return NotFound("Form submission with given ID does not exist");

            if (request.Name != String.Empty) oldSubmission.Name = request.Name;
            if (request.Email != String.Empty) oldSubmission.Email = request.Email;
            if (request.VisitPurpose != String.Empty) oldSubmission.VisitPurpose = request.VisitPurpose;
            if (request.EntranceTime != null) oldSubmission.EntranceTime = (DateTime)request.EntranceTime;
            if (request.DepartureTime != null) oldSubmission.DepartureTime = (DateTime)request.DepartureTime;
            if (request.VisiteeId != -1)
            {
                var updatedVisitee = await _context.VisitableEmployees.FindAsync(request.VisiteeId);
                if (updatedVisitee != null)
                {
                    oldSubmission.Visitee = updatedVisitee;
                    oldSubmission.VisiteeId = updatedVisitee.Id;
                }
                else
                {
                    errors += "Failed to update visitee, object with given ID not found";
                }
            }
            if (request.WifiAccessStatus == "granted") 
            {
                //grant Wifi access

                oldSubmission.WifiAccessStatus = "granted";
            }

            await _context.SaveChangesAsync();

            return Ok( new{ errors, oldSubmission } );
        }

        // POST: api/FormSubmissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FormSubmission>> PostFormSubmission(FormSubmissionDTO request)
        {
            if (_context.FormSubmissions == null)
            {
                return Problem("Entity set 'DataContext.FormSubmissions'  is null.");
            }

            var submissionVisitee = await _context.VisitableEmployees.FindAsync(request.VisiteeId);
            if (submissionVisitee == null) return NotFound("Visitee with given ID does not exist");

            if(request.WifiAccessStatus == "granted")
            {
                //grant Wifi access
            }

            FormSubmission newSubmission = new()
            {
                Name = request.Name,
                Email = request.Email,
                VisitPurpose = request.VisitPurpose,
                Signature = request.Signature,
                EntranceTime = request.EntranceTime == null ? (DateTime)request.EntranceTime : DateTime.Now,
                DepartureTime = request.DepartureTime,
                VisiteeId = request.VisiteeId,
                Visitee = submissionVisitee,
                WifiAccessStatus = request.WifiAccessStatus
            };
            _context.FormSubmissions.Add(newSubmission);
            await _context.SaveChangesAsync();

            foreach (ExtraDocument doc in _context.ExtraDocuments)
            {
                _context.FormDocuments.Add(new()
                {
                    FormId = newSubmission.Id,
                    Form = newSubmission,
                    DocumentId = doc.Id,
                    Document = doc
                });
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFormSubmission", new { id = newSubmission.Id }, newSubmission);
        }

        // DELETE: api/FormSubmissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormSubmission(int id)
        {
            var formSubmission = await _context.FormSubmissions.FindAsync(id);
            if (formSubmission == null)
            {
                return NotFound();
            }

            foreach(FormDocument formDoc in _context.FormDocuments)
            {
                if(formDoc.FormId == id)
                {
                    _context.FormDocuments.Remove(formDoc);
                }
            }
            _context.FormSubmissions.Remove(formSubmission);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
