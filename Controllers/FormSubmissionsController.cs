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
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System.Configuration;

namespace GuestSystemBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormSubmissionsController : ControllerBase
    {
        private readonly GuestSystemContext _context;
        private readonly IConfiguration _configuration;

        public FormSubmissionsController(GuestSystemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/FormSubmissions
        [HttpGet, Authorize(Roles = "super, regular")]
        public async Task<ActionResult<IEnumerable<FormSubmission>>> GetFormSubmissions()
        {
            return await _context.FormSubmissions.ToListAsync();
        }

        // GET: api/FormSubmissions/5
        [HttpGet("{id}"), Authorize(Roles = "super, regular")]
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
        [HttpPatch("{id}"), Authorize(Roles = "super, regular")]
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

        [HttpPatch("{id}/EndVisit")]
        public async Task<IActionResult> UpdateFormSubmissionDepartureTime(int id)
        {
            if (_context.FormSubmissions == null)
            {
                return Problem("Entity set 'DataContext.FormSubmissions'  is null.");
            }

            var formSub = await _context.FormSubmissions.FindAsync(id);
            if (formSub == null) return NotFound("Form submission with given ID does not exist");

            formSub.DepartureTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok("Departure time updated successfully");
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

            //Send notification email to VisitableEmployee
            SendEmail(submissionVisitee.Email, "There is a visitor waiting for you!",
                $"Hello {submissionVisitee.Name},<br> <br> {request.Name} has arrived to the office to visit you!" +
                " Please come to the office entrance to meet them.<br> <br>" +
                "Kind regards, <br> Guest entrance system");

            if (request.WifiAccessStatus == "granted")
            {
                if(request.Email == null)
                {
                    return BadRequest("Visit registration failed! Email is required to gain Wifi access");
                }

                //grant Wifi access
                string wifiCredentials = "credentials, yeah.";

                //Send Wifi credential email to form submitter
                SendEmail(request.Email, "Yuor office guest WiFi credentials",
                $"Hello {request.Name},<br> <br> Here are your guest wifi network credentials: " +
                wifiCredentials + "<br> <br>" +
                "Kind regards, <br> Guest entrance system");
            }

            FormSubmission newSubmission = new()
            {
                Name = request.Name,
                Email = request.Email,
                VisitPurpose = request.VisitPurpose,
                Signature = request.Signature,
                EntranceTime = request.EntranceTime != null ? (DateTime)request.EntranceTime : DateTime.Now,
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
        [HttpDelete("{id}"), Authorize(Roles = "super")]
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

        private void SendEmail(string recipientAddress, string emailSubject, string emailBody)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("guestservice@something.com"));
            email.To.Add(MailboxAddress.Parse(recipientAddress));
            email.Subject = emailSubject;
            email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("AppSettings:EmailHost").Value,
                int.Parse(_configuration.GetSection("AppSettings:EmailPort").Value),
                MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("AppSettings:EmailUsername").Value,
                _configuration.GetSection("AppSettings:EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
