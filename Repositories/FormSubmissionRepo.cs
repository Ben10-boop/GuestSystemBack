using GuestSystemBack.Data;
using GuestSystemBack.Interfaces;
using GuestSystemBack.Models;
using Microsoft.EntityFrameworkCore;

namespace GuestSystemBack.Repositories
{
    public class FormSubmissionRepo : IFormSubmissionRepo
    {
        private readonly GuestSystemContext _context;

        public FormSubmissionRepo(GuestSystemContext context)
        {
            _context = context;
        }

        public Task<int> AddForm(FormSubmission formSub)
        {
            _context.Add(formSub);
            return _context.SaveChangesAsync();
        }

        public Task<int> DeleteForm(FormSubmission formSub)
        {
            _context.Remove(formSub);
            return _context.SaveChangesAsync();
        }

        public bool FormsExist()
        {
            return _context.FormSubmissions == null;
        }

        public async Task<FormSubmission?> GetForm(int id)
        {
            return await _context.FormSubmissions.FindAsync(id);
        }

        public async Task<List<FormSubmission>> GetForms()
        {
            return await _context.FormSubmissions.ToListAsync();
        }

        public Task<int> UpdateForm(FormSubmission formSub)
        {
            _context.Update(formSub);
            return _context.SaveChangesAsync();
        }

        public Task<int> AddDocumentsToForm(FormSubmission formSub)
        {
            foreach (ExtraDocument doc in _context.ExtraDocuments)
            {
                _context.FormDocuments.Add(new()
                {
                    FormId = formSub.Id,
                    Form = formSub,
                    DocumentId = doc.Id,
                    Document = doc
                });
            }
            return _context.SaveChangesAsync();
        }

        public Task<int> RemoveDocumentsFromForm(int formSubId)
        {
            foreach (FormDocument formDoc in _context.FormDocuments)
            {
                if (formDoc.FormId == formSubId)
                {
                    _context.FormDocuments.Remove(formDoc);
                }
            }
            return _context.SaveChangesAsync();
        }
    }
}
