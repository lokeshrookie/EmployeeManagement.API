//using EmployeeManagement.API.Models;
//using EmployeeManagement.Repositories;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace EmployeeManagement.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EmployeesController : ControllerBase
//    {
//        private readonly IEmployeeRepository _repository;

//        public EmployeesController(IEmployeeRepository repository)
//        {
//            _repository = repository;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
//        {
//            var employees = await _repository.GetEmployees();
//            if(employees == null)
//            {
//                return NotFound();
//            }
//            return Ok(employees);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<Employee>> GetEmployee(int id)
//        {
//            var employee = await _repository.GetEmployeeById(id);
//            if (employee == null)
//            {
//                return NotFound();
//            }
//            return Ok(employee);
//        }

//        [HttpPost]
//        public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
//        {
//            if (employee == null)
//            {
//                return BadRequest();
//            }
//            var id = await _repository.AddEmployee(employee);
//            employee.Id = id;
//            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
//        {
//            if (id != employee.Id)
//            {
//                return BadRequest();
//            }
//            await _repository.UpdateEmployee(id, employee);
//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteEmployee(int id)
//        {
//            var employee = await _repository.GetEmployeeById(id);
//            if (employee == null)
//            {
//                return NotFound();
//            }
//            await _repository.DeleteEmployee(id);
//            return NoContent();
//        }
//    }
//}
