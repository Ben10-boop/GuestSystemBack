using GuestSystemBack.Models;

namespace GuestSystemBack.Interfaces
{
    public interface IExtraDocumentRepo
    {
        public Task<List<ExtraDocument>> GetDocuments();
        public Task<List<ExtraDocument>> GetActiveDocuments();
        public Task<ExtraDocument?> GetDocument(int id);
        public Task<int> AddDocument(ExtraDocument extraDoc);
        public Task<int> UpdateDocument(ExtraDocument extraDoc);
        public Task<int> DeleteDocument(ExtraDocument extraDoc);
        public bool DocumentsExist();
        public bool HasBeenSigned(int id);
    }
}
