using System;
namespace ORM_Dapper.Repositories
{
	public interface IProductRepository
	{
		public IEnumerable<Product> GetAllProducts();
		public void InsertProduct(string newProductName, double price, float categoryID, int stockLevel);
		public void UpdateProduct(Product product, string newProductName);
		public void UpdateProduct(Product product, double price);
		public void UpdateProduct(Product product, float categoryID);
		public void UpdateProduct(Product product, int stockLevel);
		public void DeleteProduct(Product product);
	}
}

