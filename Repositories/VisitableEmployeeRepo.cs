using GuestSystemBack.Data;
using GuestSystemBack.Interfaces;
using GuestSystemBack.Models;
using Microsoft.EntityFrameworkCore;

namespace GuestSystemBack.Repositories
{
    public class VisitableEmployeeRepo : IVisitableEmployeeRepo
    {
        private readonly GuestSystemContext _context;
        public VisitableEmployeeRepo(GuestSystemContext context)
        {
            _context = context;
        }

        public Task<int> AddEmployee(VisitableEmployee employee)
        {
            _context.Add(employee);
            return _context.SaveChangesAsync();
        }

        public bool EmployeeWithEmailExists(string email)
        {
            foreach (VisitableEmployee employee in _context.VisitableEmployees)
            {
                if (employee.Email == email)
                {
                    return true;
                }
            }
            return false;
        }

        public Task<int> DeleteEmployee(VisitableEmployee employee)
        {
            _context.Remove(employee);
            return _context.SaveChangesAsync();
        }

        public bool EmployeesExist()
        {
            return _context.VisitableEmployees != null;
        }

        public async Task<VisitableEmployee?> GetEmployee(int id)
        {
            return await _context.VisitableEmployees.FindAsync(id);
        }

        public async Task<List<VisitableEmployee>> GetEmployees()
        {
            return await _context.VisitableEmployees.Where(o => o.Status == "visitable").ToListAsync();
        }
        public async Task<List<VisitableEmployee>> GetAllEmployees()
        {
            return await _context.VisitableEmployees.ToListAsync();
        }

        public Task<int> UpdateEmployee(VisitableEmployee employee)
        {
            _context.Update(employee);
            return _context.SaveChangesAsync();
        }
        public bool EmployeeHasBeenVisited(int id)
        {
            foreach (FormSubmission formSub in _context.FormSubmissions)
            {
                if (formSub.VisiteeId == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
