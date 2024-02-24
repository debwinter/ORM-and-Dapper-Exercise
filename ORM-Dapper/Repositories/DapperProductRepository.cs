using System;
using System.Data;
using Dapper;

namespace ORM_Dapper.Repositories
{
	public class DapperProductRepository : IProductRepository
	{
        private readonly IDbConnection _connection;

		public DapperProductRepository(IDbConnection connection)
		{
            _connection = connection;
		}

        public IEnumerable<Product> GetAllProducts()
        {
            return _connection.Query<Product>("SELECT p.ProductID, p.Name, p.Price, p.CategoryID, p.OnSale, p.StockLevel, c.Name AS Category, r.Rating FROM Products as p LEFT JOIN Categories as c ON p.CategoryID = c.CategoryID LEFT JOIN Reviews as r ON p.ProductID = r.ProductID ORDER BY Category, Name;");
        }

        public void InsertProduct(string newProductName, double price, float categoryID, int stockLevel)
        {
            _connection.Execute("INSERT INTO Products (Name, Price, CategoryID, StockLevel) VALUES (@newName, @newPrice, @newCatID, @newStockLevel);",
                new { newName = newProductName, newPrice = price, newCatID = categoryID, newStockLevel = stockLevel });
        }

        public void UpdateProduct(Product product, string newProductName)
        {
            _connection.Execute($"UPDATE Products SET Name = @newName WHERE ProductID = {product.ProductID};",
                new { newName = newProductName });
        }

        public void UpdateProduct(Product product, double price)
        {
            _connection.Execute($"UPDATE Products SET Price = @newPrice WHERE ProductID = {product.ProductID};",
                new { newPrice = double.Parse(price.ToString("0.00")) });
        }

        public void UpdateProduct(Product product, float categoryID)
        {
            _connection.Execute($"UPDATE Products SET CategoryID = @newCatID WHERE ProductID = {product.ProductID};",
                new { newCatID = int.Parse(categoryID.ToString()) });
        }

        public void UpdateProduct(Product product, int stockChange)
        {
            _connection.Execute($"UPDATE Products SET StockLevel = StockLevel + @stockAdjust WHERE ProductID = {product.ProductID};",
                new { stockAdjust = stockChange });
        }

        public void UpdateProduct(Product product, bool onSale)
        {
            _connection.Execute($"UPDATE Products SET OnSale = @saleChange WHERE ProductID = {product.ProductID};",
                new { saleChange = onSale });
        }

        public void DeleteProduct(Product product)
        {
            _connection.Execute($"DELETE FROM Products WHERE ProductID = {product.ProductID};");
        }
    }
}

