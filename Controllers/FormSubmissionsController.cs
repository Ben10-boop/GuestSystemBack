﻿using System;
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
using GuestSystemBack.Interfaces;

namespace GuestSystemBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormSubmissionsController : ControllerBase
    {
        private readonly IFormSubmissionRepo _formSubRepo;
        private readonly IVisitableEmployeeRepo _employeeRepo;
        private readonly IEmailService _emailService;
        private readonly ICiscoApiService _ciscoApiService;

        public FormSubmissionsController(IEmailService emailService, IFormSubmissionRepo formSubRepo,
            IVisitableEmployeeRepo employeeRepo, ICiscoApiService ciscoApiService)
        {
            _emailService = emailService;
            _formSubRepo = formSubRepo;
            _employeeRepo = employeeRepo;
            _ciscoApiService = ciscoApiService;
        }

        // GET: api/FormSubmissions
        [HttpGet, Authorize(Roles = "super, regular")]
        public async Task<ActionResult<IEnumerable<FormSubmission>>> GetFormSubmissions()
        {
            return await _formSubRepo.GetForms();
        }

        // GET: api/FormSubmissions/5
        [HttpGet("{id}"), Authorize(Roles = "super, regular")]
        public async Task<ActionResult<FormSubmission>> GetFormSubmission(int id)
        {
            var formSubmission = await _formSubRepo.GetForm(id);

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
            var oldSubmission = await _formSubRepo.GetForm(id);
            string errors = "";
            if (oldSubmission == null) return NotFound("Form submission with given ID does not exist");

            if (request.Name != String.Empty) oldSubmission.Name = request.Name;
            if (request.Email != String.Empty) oldSubmission.Email = request.Email;
            if (request.VisitPurpose != String.Empty) oldSubmission.VisitPurpose = request.VisitPurpose;
            if (request.EntranceTime != null) oldSubmission.EntranceTime = (DateTime)request.EntranceTime;
            if (request.DepartureTime != null) oldSubmission.DepartureTime = (DateTime)request.DepartureTime;
            if (request.VisiteeId != -1)
            {
                var updatedVisitee = await _employeeRepo.GetEmployee(request.VisiteeId);
                if (updatedVisitee != null)
                {
                    oldSubmission.Visitee = updatedVisitee;
                    oldSubmission.VisiteeId = updatedVisitee.Id;
                }
                else
                {
                    errors += "Failed to update visitee, object with given ID not found.";
                }
            }
            if (request.WifiAccessStatus == "granted" && oldSubmission.WifiAccessStatus == "not requested") 
            {
                if (request.Email == null)
                {
                    return BadRequest("Visit registration failed! Email address is required to gain Wifi access");
                }

                //grant Wifi access (To Be Implemented)
                string wifiCredentials = "credentials, yeah.";

                //Send Wifi credential email to form submitter
                _emailService.SendEmail(request.Email, "Your office guest WiFi credentials",
                    $"Hello {request.Name},<br> <br> Here are your guest wifi network credentials: " +
                    wifiCredentials + "<br> <br>" +
                    "Kind regards, <br> Guest entrance system");

                oldSubmission.WifiAccessStatus = "granted";
            }

            await _formSubRepo.UpdateForm(oldSubmission);

            return Ok( "Updated successfully. " + errors );
        }

        [HttpPatch("{id}/EndVisit")]
        public async Task<IActionResult> UpdateFormSubmissionDepartureTime(int id)
        {
            if (!_formSubRepo.FormsExist())
            {
                return Problem("Entity set 'DataContext.FormSubmissions'  is null.");
            }

            var formSub = await _formSubRepo.GetForm(id);
            if (formSub == null) return NotFound("Form submission with given ID does not exist");

            formSub.DepartureTime = DateTime.Now;
            await _formSubRepo.UpdateForm(formSub);

            return Ok("Departure time updated successfully");
        }

        // POST: api/FormSubmissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FormSubmission>> PostFormSubmission(FormSubmissionDTO request)
        {
            if (!_formSubRepo.FormsExist())
            {
                return Problem("Entity set 'DataContext.FormSubmissions'  is null.");
            }

            var submissionVisitee = await _employeeRepo.GetEmployee(request.VisiteeId);
            if (submissionVisitee == null) return NotFound("Visitee with given ID does not exist");

            //Send notification email to VisitableEmployee
            _emailService.SendEmail(submissionVisitee.Email, "There is a visitor waiting for you!",
                $"Hello {submissionVisitee.Name},<br> <br> {request.Name} has arrived to the office to visit you!" +
                " Please come to the office entrance to meet them.<br> <br>" +
                "Kind regards, <br> Guest entrance system");

            if (request.WifiAccessStatus == "granted")
            {
                if(request.Email == null)
                {
                    return BadRequest("Visit registration failed! Email address is required to gain Wifi access");
                }

                //grant Wifi access (To Be Implemented)
                string wifiCredentials = "credentials, yeah.";

                //Send Wifi credential email to form submitter
                _emailService.SendEmail(request.Email, "Your office guest WiFi credentials",
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
            await _formSubRepo.AddForm(newSubmission);

            if (request.Signature != String.Empty && request.Signature != null) 
            {
                await _formSubRepo.AddDocumentsToForm(newSubmission);
            }

            return CreatedAtAction("GetFormSubmission", new { id = newSubmission.Id }, newSubmission);
        }

        [HttpGet("{id}/Documents")]
        public async Task<ActionResult<IEnumerable<ExtraDocument>>> GetFormSubmissionDocuments(int id)
        {
            return await _formSubRepo.GetFormDocuments(id);
        }

        // DELETE: api/FormSubmissions/5
        [HttpDelete("{id}"), Authorize(Roles = "super")]
        public async Task<IActionResult> DeleteFormSubmission(int id)
        {
            var formSubmission = await _formSubRepo.GetForm(id);
            if (formSubmission == null)
            {
                return NotFound("Form submission with given ID does not exist");
            }

            await _formSubRepo.RemoveDocumentsFromForm(id);
            await _formSubRepo.DeleteForm(formSubmission);

            return NoContent();
        }

        [HttpGet("ActiveGuests"), Authorize(Roles = "super, regular")]
        public async Task<ActionResult<IEnumerable<GuestUser>>> GetActiveGuests()
        {
            /*
            _ciscoApiService.PostWifiUser(new()
            {
                GuestUser = new GuestUser
                {
                    name = "BensTestUser",
                    guestType = "Contractor (default)",
                    guestInfo = new GuestInfo
                    {
                        password = "aaa111222333"
                    },
                    guestAccessInfo = new GuestAccessInfo
                    {
                        validDays = 1
                    },
                    portalId = "f10871e0-7159-11e7-a355-005056aba474"
                }
            });*/
            return Ok(_ciscoApiService.GetCurrentWifiUsers());
        }

        [HttpGet("Recent")]
        public async Task<ActionResult<IEnumerable<FormSubmission>>> GetRecentForms()
        {
            return await _formSubRepo.GetRecentForms();
        }

        [HttpGet("Active"), Authorize(Roles = "super, regular")]
        public async Task<ActionResult<IEnumerable<FormSubmission>>> GetActiveForms()
        {
            return await _formSubRepo.GetActiveForms();
        }

        [HttpPost("Active/Alarm"), Authorize(Roles = "super, regular")]
        public async Task<IActionResult> SendAlarmEmails(AlarmEmailDTO request)
        {
            foreach(FormSubmission activeGuest in await _formSubRepo.GetActiveForms())
            {
                if(activeGuest.Email != null && activeGuest.Email != String.Empty)
                {
                    _emailService.SendEmail(activeGuest.Email, "Alarm!",
                    request.Message + "<br> <br>" + "Kind regards, <br> Guest entrance system");
                }
            }
            return Ok("Emails sent successfully");
        }
    }
}
