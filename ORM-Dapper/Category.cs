using System;
namespace ORM_Dapper
{
	public class Category : SQLTable
	{
		public int CategoryID { get; set; }
		public string Name { get; set; }
		public int DepartmentID { get; set; }
		public string Department { get; set; }
    }
}

