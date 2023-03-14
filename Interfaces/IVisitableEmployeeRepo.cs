using GuestSystemBack.Models;

namespace GuestSystemBack.Interfaces
{
    public interface IVisitableEmployeeRepo
    {
        public Task<List<VisitableEmployee>> GetEmployees();
        public Task<VisitableEmployee?> GetEmployee(int id);
        public Task<int> AddEmployee(VisitableEmployee employee);
        public Task<int> UpdateEmployee(VisitableEmployee employee);
        public Task<int> DeleteEmployee(VisitableEmployee employee);
        public bool EmployeeWithEmailExists(string email);
        public bool EmployeesExist();
        public bool EmployeeHasBeenVisited(int id);
    }
}
