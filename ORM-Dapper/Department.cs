using System;
namespace ORM_Dapper
{
	public class Department : SQLTable
	{
		public int DepartmentID { get; set; }
		public string Name { get; set; }
	}
}

