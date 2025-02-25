﻿using EmployeeManagement.Repositories;
using EmployeeManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public ValuesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesAsync()
        {
            try
            {
                var employees = await _employeeRepository.GetEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Employee>> GetEmployeeByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Employee Id should be grater than 1.");
            }
            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(id);

                if (employee == null)
                {
                    return NotFound("Employee not found.");
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Add")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> AddEmployeeAsync([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Enter Valid Employee Details");
            }
            if (employee == null )
            {
                return BadRequest("Employee object is null.");
            }

            try
            {
                var newEmployeeId = await _employeeRepository.AddEmployeeAsync(employee);
                return CreatedAtAction(nameof(GetEmployeeByIdAsync), new { id = newEmployeeId }, newEmployeeId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Add Bulk Data 
        [HttpPost("AddBulk")]
        [AllowAnonymous]
        public async Task<IActionResult> AddEmployeesBulkAsync([FromBody] List<Employee> employees)
        {
            if (employees == null || employees.Count == 0)
            {
                return BadRequest("Employee list is null or empty.");
            }

            try
            {
                await _employeeRepository.AddEmployeesToDatabaseAsync(employees);
                return Ok("Employees added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeDto updateEmployeeDto)
        {
            if (updateEmployeeDto == null)
            {
                return BadRequest("Employee Details should not null");
            }
            try
            {
                Employee existingEmployee = await _employeeRepository.GetEmployeeByIdAsync((int) updateEmployeeDto.Id);
                if (existingEmployee == null)
                {
                    return NotFound();
                }

                #region Validations
                // dto do not contains validations.
                // Date will be validated here. (only the updated fields)
                if (!String.IsNullOrEmpty(updateEmployeeDto.Name))
                {
                    if (string.IsNullOrWhiteSpace(updateEmployeeDto.Name))
                    {
                        ModelState.AddModelError("Name", "Name is required.");
                    }
                    else if (updateEmployeeDto.Name.Length > 100)
                    {
                        ModelState.AddModelError("Name", "Name cannot be longer than 100 characters.");
                    }
                }

                if (!String.IsNullOrEmpty(updateEmployeeDto.Designation))
                {
                    if (string.IsNullOrWhiteSpace(updateEmployeeDto.Designation))
                    {
                        ModelState.AddModelError("Designation", "Designation is required.");
                    }
                    else if (updateEmployeeDto.Designation.Length > 100)
                    {
                        ModelState.AddModelError("Designation", "Designation cannot be longer than 100 characters.");
                    }
                }

                if (updateEmployeeDto.Salary != null)
                {
                    if (updateEmployeeDto.Salary < 0 || updateEmployeeDto.Salary > 9999999.99M)
                    {
                        ModelState.AddModelError("Salary", "Salary must be a positive value and less than 9999999.99.");
                    }
                }


                //|| updateEmployeeDto.DateOfBirth != null
                if (!string.IsNullOrEmpty(updateEmployeeDto.DateOfBirth.ToString()))
                {
                    if(updateEmployeeDto.DateOfBirth > DateTime.Now)
                    {
                        ModelState.AddModelError("DateOfBirth", "Date of birth cannot be in future");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Update the fields
                if (!String.IsNullOrEmpty(updateEmployeeDto.Name))
                {
                    existingEmployee.Name = updateEmployeeDto.Name;
                }
                if (!String.IsNullOrEmpty(updateEmployeeDto.Designation))
                {
                    existingEmployee.Designation = updateEmployeeDto.Designation;
                }
                // salary
                if (updateEmployeeDto.Salary > 0)
                {
                    existingEmployee.Salary = updateEmployeeDto.Salary.Value;
                }
                if(!String.IsNullOrEmpty(updateEmployeeDto.DateOfBirth.ToString())  && updateEmployeeDto.DateOfBirth > DateTime.MinValue)
                {
                    existingEmployee.DateOfBirth = updateEmployeeDto.DateOfBirth.Value;
                }


                var responseDto = new UpdateEmployeeDto
                {
                    Id = existingEmployee.Id,
                    Name = existingEmployee.Name,
                    Designation = existingEmployee.Designation,
                    Salary = existingEmployee.Salary,
                    DateOfBirth = existingEmployee.DateOfBirth
                };
                #endregion Validations
                await _employeeRepository.UpdateEmployeeAsync(responseDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        /**
         * 
         * In the request body, All values should be null except the fields that we want to update
         */
        [HttpPut("bulk-update")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateEmployeesBulk([FromBody] List<UpdateEmployeeDto> employees)
        {
            if (employees == null || employees.Count == 0)
            {
                return BadRequest("Employee list is null or empty.");
            }

            try
            {
                await _employeeRepository.UpdateEmployeesInDatabaseAsync(employees);
                return Ok("Employees updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        ///



        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteEmployeeAsync(int id)
        {
            try
            {
                Employee employee = await _employeeRepository.GetEmployeeByIdAsync(id);
                if(employee == null)
                {
                    return NotFound("Employee Not Found");
                }

                await _employeeRepository.DeleteEmployeeAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

