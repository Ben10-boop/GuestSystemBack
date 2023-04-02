using GuestSystemBack.Data;
using GuestSystemBack.Interfaces;
using GuestSystemBack.Models;
using Microsoft.EntityFrameworkCore;

namespace GuestSystemBack.Repositories
{
    public class ExtraDocumentRepo : IExtraDocumentRepo
    {
        private readonly GuestSystemContext _context;
        public ExtraDocumentRepo(GuestSystemContext context)
        {
            _context = context;
        }
        public Task<int> AddDocument(ExtraDocument extraDoc)
        {
            _context.Add(extraDoc);
            return _context.SaveChangesAsync();
        }

        public Task<int> DeleteDocument(ExtraDocument extraDoc)
        {
            _context.Remove(extraDoc);
            return _context.SaveChangesAsync();
        }

        public bool DocumentsExist()
        {
            return _context.ExtraDocuments != null;
        }

        public async Task<ExtraDocument?> GetDocument(int id)
        {
            return await _context.ExtraDocuments.FindAsync(id);
        }

        public async Task<List<ExtraDocument>> GetDocuments()
        {
            return await _context.ExtraDocuments.ToListAsync();
        }

        public async Task<List<ExtraDocument>> GetActiveDocuments()
        {
            return await _context.ExtraDocuments.Where(doc => doc.Status == "active").ToListAsync();
        }

        public bool HasBeenSigned(int id)
        {
            foreach (FormDocument formDoc in _context.FormDocuments)
            {
                if (formDoc.DocumentId == id)
                {
                    return true;
                }
            }
            return false;
        }

        public Task<int> UpdateDocument(ExtraDocument extraDoc)
        {
            _context.Update(extraDoc);
            return _context.SaveChangesAsync();
        }
    }
}
