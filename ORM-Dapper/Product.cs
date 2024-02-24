using System;
namespace ORM_Dapper
{
	public class Product : SQLTable
	{
		public int ProductID { get; set; }
		public string Name { get; set; }
		public double Price { get; set; }
		public float CategoryID { get; set; }
		public bool OnSale { get; set; }
		public int StockLevel { get; set; }
		public string Category { get; set; }
		public double Rating { get; set; }
    }
}

