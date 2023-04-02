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
using Microsoft.AspNetCore.Authorization;
using System.Data;
using GuestSystemBack.Interfaces;

namespace GuestSystemBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IExtraDocumentRepo _documentRepo;

        public DocumentsController(IExtraDocumentRepo repo)
        {
            _documentRepo = repo;
        }

        // GET: api/Documents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExtraDocument>>> GetExtraDocuments()
        {
            return await _documentRepo.GetDocuments();
        }

        [HttpGet("Active")]
        public async Task<ActionResult<IEnumerable<ExtraDocument>>> GetActiveExtraDocuments()
        {
            return await _documentRepo.GetActiveDocuments();
        }

        // GET: api/Documents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExtraDocument>> GetExtraDocument(int id)
        {
            var extraDocument = await _documentRepo.GetDocument(id);

            if (extraDocument == null)
            {
                return NotFound("Document with given ID does not exist");
            }

            return extraDocument;
        }

        // PUT: api/Documents/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}"), Authorize(Roles = "super")]
        public async Task<IActionResult> PatchExtraDocument(int id, ExtraDocumentDTO request)
        {
            var oldDocument = await _documentRepo.GetDocument(id);
            if (oldDocument == null) return NotFound("Document with given ID does not exist");

            if (request.Title != String.Empty) oldDocument.Title = request.Title;
            if (request.Content != String.Empty) oldDocument.Content = request.Content;
            if (request.Status != String.Empty) oldDocument.Status = request.Status;
            await _documentRepo.UpdateDocument(oldDocument);

            return Ok(oldDocument);
        }

        // POST: api/Documents
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = "super")]
        public async Task<ActionResult<ExtraDocument>> PostExtraDocument(ExtraDocumentDTO request)
        {
            if (!_documentRepo.DocumentsExist())
            {
                return Problem("Entity set 'DataContext.ExtraDocuments'  is null.");
            }

            if (request.Content == String.Empty)
            {
                return BadRequest("There was no file attached");
            }
            
            ExtraDocument newDocument = new()
            {
                Title = request.Title,
                Content = request.Content,
                Status = request.Status
            };
            await _documentRepo.AddDocument(newDocument);

            return CreatedAtAction("GetExtraDocument", new { id = newDocument.Id }, newDocument);
        }

        // DELETE: api/Documents/5
        [HttpDelete("{id}"), Authorize(Roles = "super")]
        public async Task<IActionResult> DeleteExtraDocument(int id)
        {
            var extraDocument = await _documentRepo.GetDocument(id);
            if (extraDocument == null)
            {
                return NotFound("Document with given ID does not exist");
            }

            if (_documentRepo.HasBeenSigned(id))
            {
                extraDocument.Status = "removed";
                return Ok(extraDocument);
            }
            await _documentRepo.DeleteDocument(extraDocument);

            return NoContent();
        }
    }
}
