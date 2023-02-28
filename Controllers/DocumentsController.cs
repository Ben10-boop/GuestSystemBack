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

namespace GuestSystemBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly GuestSystemContext _context;

        public DocumentsController(GuestSystemContext context)
        {
            _context = context;
        }

        // GET: api/Documents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExtraDocument>>> GetExtraDocuments()
        {
            return await _context.ExtraDocuments.ToListAsync();
        }

        // GET: api/Documents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExtraDocument>> GetExtraDocument(int id)
        {
            var extraDocument = await _context.ExtraDocuments.FindAsync(id);

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
            var oldDocument = await _context.ExtraDocuments.FindAsync(id);
            if (oldDocument == null) return NotFound("Document with given ID does not exist");

            if (request.Title != String.Empty) oldDocument.Title = request.Title;
            if (request.Content != String.Empty) oldDocument.Content = request.Content;
            if (request.Status != String.Empty) oldDocument.Status = request.Status;
            await _context.SaveChangesAsync();

            return Ok(oldDocument);
        }

        // POST: api/Documents
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = "super")]
        public async Task<ActionResult<ExtraDocument>> PostExtraDocument(ExtraDocumentDTO request)
        {
            if (_context.ExtraDocuments == null)
            {
                return Problem("Entity set 'DataContext.ExtraDocuments'  is null.");
            }

            ExtraDocument newDocument = new()
            {
                Title = request.Title,
                Content = request.Content,
                Status = request.Status
            };
            _context.ExtraDocuments.Add(newDocument);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExtraDocument", new { id = newDocument.Id }, newDocument);
        }

        // DELETE: api/Documents/5
        [HttpDelete("{id}"), Authorize(Roles = "super")]
        public async Task<IActionResult> DeleteExtraDocument(int id)
        {
            var extraDocument = await _context.ExtraDocuments.FindAsync(id);
            if (extraDocument == null)
            {
                return NotFound("Document with given ID does not exist");
            }

            bool hasBeenSigned = false;
            foreach (FormDocument formDoc in _context.FormDocuments)
            {
                if (formDoc.DocumentId == id)
                {
                    hasBeenSigned = true;
                    break;
                }
            }

            if (hasBeenSigned)
            {
                extraDocument.Status = "inactive";
            }
            else
            {
                _context.ExtraDocuments.Remove(extraDocument);
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
