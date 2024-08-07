using EmployeeManagement.API.Data;
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
        
        public async Task AddEmployeesToDatabase(List<Employee> employees)
        {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var employee in employees)
                            {
                                var command = new SqlCommand("InsertEmployee", connection, transaction)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };
                                command.Parameters.AddWithValue("@Name", employee.Name);
                                command.Parameters.AddWithValue("@Designation", employee.Designation);
                                command.Parameters.AddWithValue("@Salary", employee.Salary);
                                command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                                var idParameter = new SqlParameter("@Id", SqlDbType.Int)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                command.Parameters.Add(idParameter);

                                await command.ExecuteNonQueryAsync();
                                employee.Id = (int)idParameter.Value;  // Capture the new ID if needed
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                }
            }
        }
        public async Task UpdateEmployeeAsync(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                var updateCommand = new SqlCommand("UpdateEmployee", connection)
                {
                    // SqlCommand.CommandType.
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

        #region Update Employee 

        //public async Task UpdateEmployeeAsync(int id, Employee employee)
        //{


        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        // Check if employee with id exists
        //        var checkCommand = new SqlCommand("CheckEmployeeExistence", connection)
        //        {
        //            CommandType = System.Data.CommandType.StoredProcedure
        //        };
        //        checkCommand.Parameters.AddWithValue("@Id", id);
        //        // Add row count as output parameter
        //        var countParamter = new SqlParameter("@RowCount", System.Data.SqlDbType.Int)
        //        {
        //            Direction = System.Data.ParameterDirection.Output
        //        };

        //        checkCommand.Parameters.Add(countParamter);
        //        await checkCommand.ExecuteNonQueryAsync();
        //        if ((int)countParamter.Value > 0)
        //        {

        //            var updateCommand = new SqlCommand("UpdateEmployee", connection)
        //            {
        //                CommandType = System.Data.CommandType.StoredProcedure
        //            };
        //            updateCommand.Parameters.AddWithValue("@Id", employee.Id);
        //            updateCommand.Parameters.AddWithValue("@Name", employee.Name);
        //            updateCommand.Parameters.AddWithValue("@Designation", employee.Designation);
        //            updateCommand.Parameters.AddWithValue("@Salary", employee.Salary);

        //            await updateCommand.ExecuteNonQueryAsync();
        //        }
        //    }
        //}
        #endregion Update Employee

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

    }
}
