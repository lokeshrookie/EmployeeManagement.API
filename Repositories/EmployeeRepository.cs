﻿using EmployeeManagement.API.Data;
using EmployeeManagement.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmployeeManagement.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public EmployeeRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            var employees = new List<Employee>();

            using(var connection  =  new SqlConnection(_connectionString)){
                await connection.OpenAsync();
                var command = new SqlCommand("GetEmployees", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                using(var reader = await command.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        employees.Add(new Employee
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Designation = reader.GetString(2),
                            Salary = reader.GetDecimal(3),
                            DateOfBirth = reader.GetDateTime(4)
                        });
                    }
                    await reader.CloseAsync();
                }
            }
            return employees;
            #region Comment
            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    await connection.OpenAsync();
            //    var command = new SqlCommand("GetEmployees", connection)
            //    {
            //        CommandType = System.Data.CommandType.StoredProcedure
            //    };

            //    using (var reader = await command.ExecuteReaderAsync())
            //    {
            //        while (await reader.ReadAsync())
            //        {
            //            employees.Add(new Employee
            //            {
            //                Id = reader.GetInt32(0),
            //                Name = reader.GetString(1),
            //                Designation = reader.GetString(2),
            //                Salary = reader.GetDecimal(3)
            //            });
            //        }
            //    }
            //}
            //return employees;
            #endregion Comment
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
               
            Employee employee = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("GetEmployeesById", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@empId", id));
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Designation = reader.GetString(2),
                            Salary = reader.GetDecimal(3),
                            DateOfBirth = reader.GetDateTime(4)
                        };
                    }
                    await reader.CloseAsync();
                }
            }
            return employee;

        }


        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            int id;
            
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("InsertEmployee", connection)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@Name", employee.Name);
                    command.Parameters.AddWithValue("@Designation", employee.Designation);
                    command.Parameters.AddWithValue("@Salary", employee.Salary);
                    command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);

                    // add Id paramter as output paramter.
                    var idParameter = new SqlParameter("@Id", System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    command.Parameters.Add(idParameter);

                    await command.ExecuteNonQueryAsync();

                    id = (int)idParameter.Value;
                }
            
            return id;
        }


        // Bulk
        
        public async Task AddEmployeesToDatabaseAsync(List<Employee> employees)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // insert using DataTable() instead of inserting one by one.
                        var dataTable = new DataTable();
                        dataTable.Columns.Add("Name", typeof(string));
                        dataTable.Columns.Add("Designation", typeof(string));
                        dataTable.Columns.Add("Salary", typeof(decimal));
                        dataTable.Columns.Add("DateOfBirth", typeof(DateTime));
                        foreach (var employee in employees)
                        {
                            dataTable.Rows.Add(employee.Name, employee.Designation, employee.Salary, employee.DateOfBirth);
                        }
                       
                        //Console.WriteLine(dataTable);
                        var command = new SqlCommand("InsertEmployeeTable", connection, transaction)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        var parameter = new SqlParameter
                        {
                            ParameterName = "@Employees",
                            SqlDbType = SqlDbType.Structured,
                            // Employee Table is User defined  table type (already created).
                            TypeName = "EmployeeTable",
                            Value = dataTable
                        };
                        command.Parameters.Add(parameter);
                        await command.ExecuteNonQueryAsync();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                #region forEach

                //try
                //{
                //    foreach (var employee in employees)
                //    {
                //        var command = new SqlCommand("InsertEmployee", connection, transaction)
                //        {
                //            CommandType = CommandType.StoredProcedure
                //        };
                //        command.Parameters.AddWithValue("@Name", employee.Name);
                //        command.Parameters.AddWithValue("@Designation", employee.Designation);
                //        command.Parameters.AddWithValue("@Salary", employee.Salary);
                //        command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                //        //var idParameter = new SqlParameter("@Id", SqlDbType.Int)
                //        //{
                //        //    Direction = ParameterDirection.Output
                //        //};
                //        //command.Parameters.Add(idParameter);

                //        await command.ExecuteNonQueryAsync();
                //        //employee.Id = (int)idParameter.Value;  // Capture the new ID if needed
                //    }

                //    transaction.Commit();
                //}
                //catch
                //{
                //    transaction.Rollback();
                //    throw;
                //}

                #endregion forEach
            }
        }
        public async Task UpdateEmployeeAsync(UpdateEmployeeDto employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var updateCommand = new SqlCommand("UpdateEmployee", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,

                };

                // SQL Parameter Collection.
                updateCommand.Parameters.AddWithValue("@Id", employee.Id);
                updateCommand.Parameters.AddWithValue("@Name", employee.Name);
                updateCommand.Parameters.AddWithValue("@Designation", employee.Designation);
                updateCommand.Parameters.AddWithValue("@Salary", employee.Salary);
                updateCommand.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                await updateCommand.ExecuteNonQueryAsync();
            }
        }

        #region UpdateEmployeesBulk

        //private async Task UpdateEmployeesBulkAsync(int id, UpdateEmployeeDto employee)
        //{
           
        //}


        #endregion UpdateEmployeesBulk


        public async Task DeleteEmployeeAsync(int id)
        {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("DeleteEmployee", connection)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@empId", id);
                    await command.ExecuteNonQueryAsync();
                }
        }



        public async  Task UpdateEmployeesInDatabaseAsync(List<UpdateEmployeeDto> employees)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("UpdateEmployeesBulk", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Create and populate the DataTable for the table-valued parameter
                    var dataTable = new DataTable();
                    dataTable.Columns.Add("Id", typeof(int));
                    dataTable.Columns.Add("Name", typeof(string));
                    dataTable.Columns.Add("Designation", typeof(string));
                    dataTable.Columns.Add("Salary", typeof(decimal));
                    dataTable.Columns.Add("DateOfBirth", typeof(DateTime));


                    foreach (var employee in employees)
                    {
                        dataTable.Rows.Add(employee.Id, employee.Name, employee.Designation, employee.Salary, employee.DateOfBirth);
                    }


                    #region comment

                    //foreach (var employee in employees)
                    //{
                    //    dataTable.Rows.Add(
                    //        employee.Id,
                    //        employee.Name,
                    //        employee.Designation,
                    //        employee.Salary,
                    //        employee.DateOfBirth
                    //    );
                    //}

                    //var parameter = new SqlParameter
                    //{
                    //    ParameterName = "@Employees",
                    //    SqlDbType = SqlDbType.Structured,
                    //    // Employee Table is User defined  table type (already created).
                    //    TypeName = "EmployeeTable",
                    //    Value = dataTable
                    //};
                    #endregion comment


                    
                    var parameter = new SqlParameter
                    {
                        ParameterName = "@EmployeeUpdates",
                        SqlDbType = SqlDbType.Structured,
                        TypeName = "EmployeeUpdateType",  // Must match the type name in SQL Server
                        Value = dataTable
                    };

                    command.Parameters.Add(parameter);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
