using System;
namespace ORM_Dapper.Repositories
{
	public interface ICategoryRepository
	{
		public IEnumerable<Category> GetAllCategories();
		public void InsertCategory(string newCategoryName, int newDeptID);
		public void UpdateCategory(Category category, string newCategoryName);
		public void UpdateCategory(Category category, int newDepartmentID);
		public void DeleteCategory(Category category);
	}
}

