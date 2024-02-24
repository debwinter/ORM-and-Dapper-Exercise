using System;
namespace ORM_Dapper
{
	public static class TablePrinter
	{
        public static void DepartmentPrinter(IEnumerable<Department> departments)
        {
            // Define table headers
            string[] headers = { "Dept ID", "Dept Name" };

            // Determine column widths
            int[] columnWidths = new int[headers.Length];
            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = headers[i].Length;
                foreach (var dept in departments)
                {
                    string value = "";
                    switch (i)
                    {
                        case 0:
                            value = dept.DepartmentID.ToString();
                            break;
                        case 1:
                            value = dept.Name;
                            break;
                    }
                    if (value.Length > columnWidths[i])
                        columnWidths[i] = value.Length;
                }
            }

            // Print headers
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < headers.Length; i++)
            {
                Console.Write("| " + headers[i].PadRight(columnWidths[i] + 1));
            }
            Console.WriteLine();
            Console.ResetColor();

            // Print rows
            foreach (var dept in departments)
            {
                Console.Write("| " + dept.DepartmentID.ToString().PadRight(columnWidths[0] + 1));
                Console.Write("| " + dept.Name.PadRight(columnWidths[1] + 1));
                Console.WriteLine();
                Thread.Sleep(100);
            }
        }

        public static void CategoryPrinter(IEnumerable<Category> categories)
        {
            // Define table headers
            string[] headers = { "ID", "Category" };

            // Determine column widths
            int[] columnWidths = new int[headers.Length];
            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = headers[i].Length;
                foreach (var cat in categories)
                {
                    string value = "";
                    switch (i)
                    {
                        case 0:
                            value = cat.CategoryID.ToString();
                            break;
                        case 1:
                            value = cat.Name;
                            break;
                    }
                    if (value.Length > columnWidths[i])
                        columnWidths[i] = value.Length;
                }
            }

            // Print headers
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < headers.Length; i++)
            {
                Console.Write("| " + headers[i].PadRight(columnWidths[i] + 1));
            }
            Console.WriteLine();
            Console.ResetColor();

            // Print rows
            foreach (var cat in categories)
            {
                Console.Write("| " + cat.CategoryID.ToString().PadRight(columnWidths[0] + 1));
                Console.Write("| " + cat.Name.PadRight(columnWidths[1] + 1));
                Console.WriteLine();
                Thread.Sleep(100);
            }
        }

        public static void ProductPrinter(IEnumerable<Product> products)
        {
            // Define table headers
            string[] headers = { "ID", "Product", "Price", "" };

            // Determine column widths
            int[] columnWidths = new int[headers.Length];
            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = headers[i].Length;
                foreach (var prod in products)
                {
                    string value = "";
                    switch (i)
                    {
                        case 0:
                            value = prod.ProductID.ToString();
                            break;
                        case 1:
                            columnWidths[i] = 25;
                            break;
                        case 2:
                            value = $"${prod.Price.ToString()}";
                            break;
                        case 3:
                            columnWidths[i] = 4;
                            break;
                    }
                    if (value.Length > columnWidths[i])
                        columnWidths[i] = value.Length;
                }
            }

            // Print headers
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < headers.Length; i++)
            {
                Console.Write("| " + headers[i].PadRight(columnWidths[i] + 1));
            }
            Console.WriteLine();
            Console.ResetColor();

            // Print rows
            foreach (var prod in products)
            {
                Console.Write("| " + prod.ProductID.ToString().PadRight(columnWidths[0] + 1));
                if (prod.Name.Length <= 20)
                {
                    Console.Write("| " + prod.Name.PadRight(26));
                }
                else
                {
                    Console.Write("| " + prod.Name.Substring(0, 20) + ". . . ");
                }
                Console.Write("| $" + prod.Price.ToString().PadRight(columnWidths[2]));
                Console.Write("| " + ((prod.OnSale == true) ? "SALE" : ""));
                Console.WriteLine();
                Thread.Sleep(50);
            }
        }

        public static void BackEndCategoryPrinter(IEnumerable<Category> categories)
        {
            // Define table headers
            string[] headers = { "ID", "Category", "Department" };

            // Determine column widths
            int[] columnWidths = new int[headers.Length];
            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = headers[i].Length;
                foreach (var cat in categories)
                {
                    string value = "";
                    switch (i)
                    {
                        case 0:
                            value = cat.CategoryID.ToString();
                            break;
                        case 1:
                            value = cat.Name;
                            break;
                        case 2:
                            value = cat.Department;
                            break;
                    }
                    if (value.Length > columnWidths[i])
                        columnWidths[i] = value.Length;
                }
            }

            // Print headers
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < headers.Length; i++)
            {
                Console.Write("| " + headers[i].PadRight(columnWidths[i] + 1));
            }
            Console.WriteLine();
            Console.ResetColor();

            // Print rows
            foreach (var cat in categories)
            {
                Console.Write("| " + cat.CategoryID.ToString().PadRight(columnWidths[0] + 1));
                Console.Write("| " + cat.Name.PadRight(columnWidths[1] + 1));
                Console.Write("| " + cat.Department.PadRight(columnWidths[2] + 1));
                Console.WriteLine();
                Thread.Sleep(100);
            }
        }

        public static void BackEndProductPrinter(IEnumerable<Product> products)
        {
            // Define table headers
            string[] headers = { "ID", "Product", "Category", "Price", "" };

            // Determine column widths
            int[] columnWidths = new int[headers.Length];
            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = headers[i].Length;
                foreach (var prod in products)
                {
                    string value = "";
                    switch (i)
                    {
                        case 0:
                            value = prod.ProductID.ToString();
                            break;
                        case 1:
                            columnWidths[i] = 25;
                            break;
                        case 2:
                            value = prod.Category;
                            break;
                        case 3:
                            value = $"${prod.Price.ToString()}";
                            break;
                        case 4:
                            columnWidths[i] = 4;
                            break;
                    }
                    if (value.Length > columnWidths[i])
                        columnWidths[i] = value.Length;
                }
            }

            // Print headers
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < headers.Length; i++)
            {
                Console.Write("| " + headers[i].PadRight(columnWidths[i] + 1));
            }
            Console.WriteLine();
            Console.ResetColor();

            // Print rows
            foreach (var prod in products)
            {
                Console.Write("| " + prod.ProductID.ToString().PadRight(columnWidths[0] + 1));
                if (prod.Name.Length <= 20)
                {
                    Console.Write("| " + prod.Name.PadRight(26));
                }
                else
                {
                    Console.Write("| " + prod.Name.Substring(0, 20) + ". . . ");
                }
                Console.Write("| " + prod.Category.PadRight(columnWidths[2] + 1));
                Console.Write("| $" + prod.Price.ToString().PadRight(columnWidths[3]));
                Console.Write("| " + ((prod.OnSale == true) ? "SALE" : ""));
                Console.WriteLine();
                Thread.Sleep(50);
            }
        }
    }
}

