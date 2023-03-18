using GuestSystemBack.Models;

namespace GuestSystemBack.Interfaces
{
    public interface IFormSubmissionRepo
    {
        public Task<List<FormSubmission>> GetForms();
        public Task<FormSubmission?> GetForm(int id);
        public Task<int> AddForm(FormSubmission formSub);
        public Task<int> UpdateForm(FormSubmission formSub);
        public Task<int> DeleteForm(FormSubmission formSub);
        public bool FormsExist();
        public Task<int> AddDocumentsToForm(FormSubmission formSub);
        public Task<int> RemoveDocumentsFromForm(int formSubId);
    }
}
