using System;
using System.Data;
using Dapper;

namespace ORM_Dapper.Repositories
{
	public class DapperDepartmentRepository : IDepartmentRepository
	{
        private readonly IDbConnection _connection;

        public DapperDepartmentRepository(IDbConnection connection)
		{
            _connection = connection;
		}

        public IEnumerable<Department> GetAllDepartments()
        {
            return _connection.Query<Department>("SELECT * FROM Departments;");
        }

        public void InsertDepartment(string newDepartmentName)
        {
            _connection.Execute("INSERT INTO Departments (Name) VALUES (@name);",
                new { name = newDepartmentName });
        }

        public void UpdateDepartment(Department department, string newDepartmentName)
        {
            _connection.Execute($"UPDATE Departments SET Name = @newName WHERE DepartmentID = {department.DepartmentID};",
                new { newName = newDepartmentName });
        }

        public void UpdateDepartment(Department department, int newDepartmentID)
        {
            _connection.Execute($"UPDATE Departments SET DepartmentID = @newID WHERE DepartmentID = {department.DepartmentID};",
                new { newID = newDepartmentID });
        }

        public void DeleteDepartment(Department department)
        {
            _connection.Execute($"DELETE FROM Departments WHERE DepartmentID = {department.DepartmentID};");
        }
    }
}

