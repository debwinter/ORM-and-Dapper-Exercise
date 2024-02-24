using System;
namespace ORM_Dapper
{
	public class Employee : SQLTable
	{
		public int EmployeeID { get; set; }
		public string FirstName { get; set; }
		public string MiddleInitial { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public string Title { get; set; }
		public string FullName
		{
			get
			{
				return (MiddleInitial == null) ? $"{FirstName} {LastName}" : $"{FirstName} {MiddleInitial}. {LastName}";
			}
		}
	}
}

