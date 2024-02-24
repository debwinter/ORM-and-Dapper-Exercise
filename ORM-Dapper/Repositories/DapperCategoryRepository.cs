using System;
using System.Data;
using Dapper;

namespace ORM_Dapper.Repositories
{
	public class DapperCategoryRepository : ICategoryRepository
	{
        private readonly IDbConnection _connection;

        public DapperCategoryRepository(IDbConnection connection)
		{
            _connection = connection;
		}

        public IEnumerable<Category> GetAllCategories()
        {
            return _connection.Query<Category>("SELECT c.CategoryID, c.Name, c.DepartmentID, d.Name AS Department FROM Categories as c LEFT JOIN Departments as d ON c.DepartmentID = d.DepartmentID ORDER BY Department, CategoryID;");
        }

        public void InsertCategory(string newCategoryName, int newDeptID)
        {
            _connection.Execute("INSERT INTO Categories (Name, DepartmentID) VALUES (@newName, @newDept);",
                new { newName = newCategoryName, newDept = newDeptID });
        }

        public void UpdateCategory(Category category, string newCategoryName)
        {
            _connection.Execute($"UPDATE Categories SET Name = @newName WHERE CategoryID = {category.CategoryID};",
                new { newName = newCategoryName });
        }

        public void UpdateCategory(Category category, int newDepartmentID)
        {
            _connection.Execute($"UPDATE Categories SET DepartmentID = @newDeptID WHERE CategoryID = {category.CategoryID};",
                new { newDeptID = newDepartmentID });
        }

        public void DeleteCategory(Category category)
        {
            _connection.Execute($"DELETE FROM Categories WHERE CategoryID = {category.CategoryID};");
        }
    }
}

