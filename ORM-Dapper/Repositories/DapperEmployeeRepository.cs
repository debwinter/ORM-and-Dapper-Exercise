using System;
using System.Data;
using Dapper;

namespace ORM_Dapper.Repositories
{
	public class DapperEmployeeRepository
	{
		private readonly IDbConnection _connection;

		public DapperEmployeeRepository(IDbConnection connection)
		{
			_connection = connection;
		}

		public IEnumerable<Employee> GetAllEmployees()
		{
			return _connection.Query<Employee>("SELECT * FROM Employees;");
		}
	}
}

