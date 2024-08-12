using EmployeeManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<int> AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(UpdateEmployeeDto employee);
        Task DeleteEmployeeAsync(int id);
        Task AddEmployeesToDatabaseAsync(List<Employee> employees);

        Task UpdateEmployeesInDatabaseAsync(List<UpdateEmployeeDto> employees);
    }
}

