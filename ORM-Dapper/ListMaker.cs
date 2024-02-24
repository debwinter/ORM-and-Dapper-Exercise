using System;
namespace ORM_Dapper
{
	public static class ListMaker
	{
        public static IEnumerable<Category> CategoriesByDepartment(IEnumerable<Category> categories, string departmentID)
        {
            var list = new List<Category>();
            foreach (var cat in categories)
            {
                if (cat.DepartmentID == int.Parse(departmentID)) list.Add(cat);
            }
            return list;
        }

        public static IEnumerable<Product> ProductByCategory(IEnumerable<Product> products, string categoryID)
        {
            var list = new List<Product>();
            foreach (var prod in products)
            {
                if (prod.CategoryID == int.Parse(categoryID)) list.Add(prod);
            }
            return list;
        }


    }
}

