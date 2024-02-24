using System;
namespace ORM_Dapper.Repositories
{
	public interface IDepartmentRepository
	{
		IEnumerable<Department> GetAllDepartments();
		public void InsertDepartment(string newDepartmentName);
		public void UpdateDepartment(Department department, string newDepartmentName);
		public void DeleteDepartment(Department department);
    }
}

