using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ORM_Dapper
{
    public class Program
    {
        static void Main(string[] args)
        {
            IDbConnection conn = OpenStore();

            Console.WriteLine("Welcome to Best Buy! \n" +
                "I am a virtual assistant. \n" +
                "How can I help you today?");
            Console.WriteLine();

            MainMenu(conn);
        }

        static void MainMenu(IDbConnection conn)
        {
            Console.WriteLine("1- Shop our products");
            Console.WriteLine("2- Review my purchase");
            Console.WriteLine("3- Make a return");
            Console.WriteLine("4- Exit");
            Console.WriteLine("5- Employee Login");
            Console.WriteLine();

            var validInputs = new string[] { "1", "2", "3", "4", "5" };
            string userInput = UserInput.ValidInput(validInputs, Console.ReadLine());

            switch (userInput)
            {
                case "1":
                    GoShopping(conn);
                    break;
                case "2":
                    Review(conn);
                    break;
                case "3":
                    Return(conn);
                    break;
                case "4":
                    break;
                case "5":
                    LogIn(conn);
                    break;
            }
        }

        static IEnumerable<Product> ProductFinder(IDbConnection conn)
        {
            var departments = GetDepartments(conn);
            var categories = GetCategories(conn);
            var products = GetProducts(conn);

            TablePrinter.DepartmentPrinter(departments);
            Console.WriteLine();
            Console.Write("Enter the Dept ID: ");

            var validInputs = new List<string>();
            foreach (var dept in departments)
            {
                validInputs.Add(dept.DepartmentID.ToString());
            }
            string userInput = UserInput.ValidInput(validInputs, Console.ReadLine());
            validInputs.Clear();

            var selectedDept = DepartmentByID(departments, int.Parse(userInput));
            var deptCategories = ListMaker.CategoriesByDepartment(categories, userInput);

            Console.WriteLine();
            Console.WriteLine($"Choose a category from the {selectedDept.Name} department:");
            TablePrinter.CategoryPrinter(deptCategories);
            Console.WriteLine();
            Console.Write("Enter the ID: ");

            userInput = UserInput.ValidInput(validInputs, Console.ReadLine());
            var catProducts = ListMaker.ProductByCategory(products, userInput);
            return catProducts;
        }

        static void GoShopping(IDbConnection conn)
        {
            Console.WriteLine("What department would you like to shop in today?");
            var selectedProducts = ProductFinder(conn);

            var validInputs = new List<string>();
            foreach (var prod in selectedProducts)
            {
                validInputs.Add(prod.ProductID.ToString());
            }

            Console.WriteLine();
            Console.WriteLine($"OK, here are some options for purchase:");
            TablePrinter.ProductPrinter(selectedProducts);
            Console.WriteLine();
            Console.WriteLine("Would you like to purchase one of these items? (Y/N)");
            if (UserInput.YesOrNo(Console.ReadLine()))
            {
                Console.WriteLine();
                int userID = ConfirmProduct(selectedProducts, validInputs);
                validInputs.Clear();

                Console.WriteLine();
                Console.WriteLine("Type 'PURCHASE' to confirm your purchase: ");
                if (UserInput.ConfirmInput("PURCHASE", Console.ReadLine()))
                {
                    Product purchased = ProductByID(selectedProducts, userID);
                    UserInput.SuccessMessage($"Success! I have debited ${purchased.Price} from your account.");

                    var repo = new Repositories.DapperProductRepository(conn);
                    repo.UpdateProduct(purchased, -1);
                }
                else
                {
                    UserInput.ErrorMessage("I'm sorry, I could not complete your purchase.");
                }
            }
            else
            {
                Console.WriteLine("OK, can I help you find something else? (Y/N)");
                if (UserInput.YesOrNo(Console.ReadLine()))
                {
                    GoShopping(conn);
                }
            }
            Console.WriteLine();
            Console.WriteLine("Is there anything else I can help you with today? (Y/N)");
            if (UserInput.YesOrNo(Console.ReadLine()))
            {
                Console.WriteLine();
                MainMenu(conn);
            }
            else Console.WriteLine("OK, have a great rest of your day!");
        }

        static int ConfirmProduct(IEnumerable<Product> products, List<string> validInputs)
        {
            Console.Write("Enter the ID of the product you wish to select: ");
            var userInput = UserInput.ValidInput(validInputs, Console.ReadLine());
            var userID = int.Parse(userInput);

            Console.WriteLine();
            Console.WriteLine("You have chosen:");
            UserInput.SuccessMessage(ProductByID(products, userID).Name);
            Console.WriteLine();
            Console.WriteLine("Is that correct? (Y/N)");
            if (!UserInput.YesOrNo(Console.ReadLine()))
            {
                Console.WriteLine("I'm sorry.");
                ConfirmProduct(products, validInputs);
            }
            return userID;
        }

        static void Review(IDbConnection conn)
        {
            Console.WriteLine("What department did you purchase your item from?");
            var selectedProducts = ProductFinder(conn);

            var validInputs = new List<string>();
            foreach (var prod in selectedProducts) validInputs.Add(prod.ProductID.ToString());

            Console.WriteLine();
            Console.WriteLine("Choose a product to review:");
            TablePrinter.ProductPrinter(selectedProducts);
            Console.WriteLine();
            Console.WriteLine("Is your product listed here? (Y/N)");
            if (!UserInput.YesOrNo(Console.ReadLine())) Review(conn);

            int prodID = ConfirmProduct(selectedProducts, validInputs);
            validInputs.Clear();

            Console.WriteLine();
            Console.WriteLine("How would you rate:");
            UserInput.SuccessMessage(ProductByID(selectedProducts, prodID).Name);
            Console.WriteLine("on a scale from 1 to 5? ");

            validInputs.Clear();
            for (int x = 1; x <= 5; x++) validInputs.Add(x.ToString());
            var input = UserInput.ValidInput(validInputs, Console.ReadLine());
            var rating = int.Parse(input);

            Console.WriteLine("Please leave a brief comment on why you chose this rating.");
            var comment = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("Thank you for your feedback! \n" +
                "What is your name?");
            var reviewer = Console.ReadLine();

            var repo = new Repositories.DapperReviewRepository(conn);
            repo.NewReview(prodID, reviewer, rating, comment);

            Console.WriteLine($"Thanks for taking the time to review our products, {reviewer}!");
            Console.WriteLine();
            Console.WriteLine("Is there anything else I can help you with today? (Y/N)");
            if (UserInput.YesOrNo(Console.ReadLine()))
            {
                Console.WriteLine();
                MainMenu(conn);
            }
            else Console.WriteLine("OK, have a great rest of your day!");
        }

        static void Return(IDbConnection conn)
        {
            Console.WriteLine("I'm sorry it didn't work for you!");
            Console.WriteLine("What department did you purchase this product from?");
            var selectedProducts = ProductFinder(conn);

            var validInputs = new List<string>();
            foreach (var prod in selectedProducts) validInputs.Add(prod.ProductID.ToString());

            Console.WriteLine();
            Console.WriteLine("Choose a product:");
            TablePrinter.ProductPrinter(selectedProducts);
            Console.WriteLine();
            Console.WriteLine("Is your product listed here? (Y/N)");
            if (!UserInput.YesOrNo(Console.ReadLine())) Return(conn);

            int productID = ConfirmProduct(selectedProducts, validInputs);

            Console.WriteLine();
            Console.Write("Type 'RETURN' to complete this return: ");
            if (UserInput.ConfirmInput("RETURN", Console.ReadLine()))
            {
                Product returned = ProductByID(selectedProducts, productID);
                UserInput.SuccessMessage($"Success! I have credited ${returned.Price} to your account.");

                var productRepository = new Repositories.DapperProductRepository(conn);
                productRepository.UpdateProduct(returned, 1);

                Console.WriteLine("Would you like to review this product for future shoppers?");
                if (UserInput.YesOrNo(Console.ReadLine()))
                {
                    Console.WriteLine();
                    Console.WriteLine("How would you rate:");
                    UserInput.SuccessMessage(returned.Name);
                    Console.WriteLine("on a scale from 1 to 5? ");

                    validInputs.Clear();
                    for (int x = 1; x <= 5; x++) validInputs.Add(x.ToString());
                    var input = UserInput.ValidInput(validInputs, Console.ReadLine());
                    var rating = int.Parse(input);

                    Console.WriteLine("Please leave a brief comment on why you chose this rating.");
                    var comment = Console.ReadLine();
                    Console.WriteLine();
                    Console.WriteLine("Thank you for your feedback! \n" +
                        "What is your name?");
                    var reviewer = Console.ReadLine();

                    var reviewRepository = new Repositories.DapperReviewRepository(conn);
                    reviewRepository.NewReview(productID, reviewer, rating, comment);

                    Console.WriteLine($"Thanks for taking the time to review our products, {reviewer}!");
                }
            }
            else
            {
                UserInput.ErrorMessage("I'm sorry, I could not complete your return.");
            }
            Console.WriteLine();
            Console.WriteLine("Is there anything else I can help you with today? (Y/N)");
            if (UserInput.YesOrNo(Console.ReadLine()))
            {
                Console.WriteLine();
                MainMenu(conn);
            }
            else Console.WriteLine("OK, have a great rest of your day!");
        }

        static void LogIn(IDbConnection conn)
        {
            var employees = GetEmployees(conn);

            var validInputs = new List<string>();
            foreach (var e in employees)
            {
                validInputs.Add(e.EmployeeID.ToString());
            }

            Console.Write("Enter your employee ID to log in: ");
            var employeeID = int.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
            var loggedInEmployee = EmployeeByID(employees, employeeID);

            Console.WriteLine();
            Console.Write("Welcome back, ");
            UserInput.SuccessMessage(loggedInEmployee.FullName + "!");

            EmployeeMenu(conn);
        }

        static void EmployeeMenu(IDbConnection conn)
        {
            Console.WriteLine();
            Console.WriteLine("EMPLOYEE MENU:");
            Console.WriteLine("1- Edit departments");
            Console.WriteLine("2- Edit categories");
            Console.WriteLine("3- Edit products");
            Console.WriteLine("4- Exit");
            Console.WriteLine();


            var validInputs = new List<string>();
            for (int x = 1; x <= 4; x++) validInputs.Add(x.ToString());

            Console.Write("Enter selection: ");
            var menuSelection = UserInput.ValidInput(validInputs, Console.ReadLine());
            Console.WriteLine();

            switch (menuSelection)
            {
                case "1":
                    DepartmentBackEnd(conn);
                    break;
                case "2":
                    CategoryBackEnd(conn);
                    break;
                case "3":
                    ProductBackEnd(conn);
                    break;
            }
        }

        static void DepartmentBackEnd(IDbConnection conn)
        {
            var departments = GetDepartments(conn);
            var repo = new Repositories.DapperDepartmentRepository(conn);

            TablePrinter.DepartmentPrinter(departments);
            Console.WriteLine();

            Console.WriteLine("BACK-END MENU");
            Console.WriteLine("1- Add new department");
            Console.WriteLine("2- Edit existing department");
            Console.WriteLine("3- Delete a department");
            Console.WriteLine();

            Console.Write("Enter selection: ");
            var validInputs = new List<string>() { "1", "2", "3" };
            var menuSelection = UserInput.ValidInput(validInputs, Console.ReadLine());
            validInputs.Clear();

            switch (menuSelection)
            {
                case "1":
                    Console.Write("New department name: ");
                    var newDeptName = Console.ReadLine();

                    repo.InsertDepartment(newDeptName);
                    departments = GetDepartments(conn);
                    Console.WriteLine();
                    Console.WriteLine("UPDATED DEPT LIST:");
                    TablePrinter.DepartmentPrinter(departments);
                    break;
                case "2":
                    foreach (var d in departments) validInputs.Add(d.DepartmentID.ToString());

                    Console.Write("Enter department ID to edit: ");
                    var deptID = int.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
                    var selectedDept = DepartmentByID(departments, deptID);
                    validInputs.Clear();
                    Console.WriteLine();

                    Console.Write("You are about to edit: ");
                    UserInput.SuccessMessage(selectedDept.Name);
                    Console.Write("Is that correct? (Y/N): ");
                    if (UserInput.YesOrNo(Console.ReadLine()))
                    {
                        Console.Write("Updated department name: ");
                        newDeptName = Console.ReadLine();
                        Console.Write("Type 'UPDATE' to confirm: ");
                        if (UserInput.ConfirmInput("UPDATE", Console.ReadLine()))
                        {
                            repo.UpdateDepartment(selectedDept, newDeptName);
                            departments = GetDepartments(conn);
                            Console.WriteLine();
                            Console.WriteLine("UPDATED DEPT LIST:");
                            TablePrinter.DepartmentPrinter(departments);
                        }
                        else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    }
                    else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    break;
                case "3":
                    foreach (var d in departments) validInputs.Add(d.DepartmentID.ToString());

                    Console.Write("Enter the ID of the department to delete: ");
                    deptID = int.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
                    selectedDept = DepartmentByID(departments, deptID);

                    Console.Write("You are about to delete: ");
                    UserInput.SuccessMessage(selectedDept.Name);
                    Console.Write("Is that correct? (Y/N): ");
                    if (UserInput.YesOrNo(Console.ReadLine()))
                    {
                        Console.WriteLine();
                        Console.Write("Type 'DELETE' to confirm: ");
                        if (UserInput.ConfirmInput("DELETE", Console.ReadLine()))
                        {
                            repo.DeleteDepartment(selectedDept);
                            departments = GetDepartments(conn);
                            Console.WriteLine();
                            Console.WriteLine("UPDATED DEPT LIST:");
                            TablePrinter.DepartmentPrinter(departments);
                        }
                        else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    }
                    else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    break;
            }
            Console.WriteLine();
            Console.WriteLine("Is there anything else I can help you with today? (Y/N)");
            if (UserInput.YesOrNo(Console.ReadLine()))
            {
                Console.WriteLine();
                EmployeeMenu(conn);
            }
            else Console.WriteLine("OK, have a great rest of your day!");
        }

        static void CategoryBackEnd(IDbConnection conn)
        {
            var categories = GetCategories(conn);
            var departments = GetDepartments(conn);
            var repo = new Repositories.DapperCategoryRepository(conn);

            Console.WriteLine("BACK-END MENU");
            Console.WriteLine("1- Add new category");
            Console.WriteLine("2- Edit existing category");
            Console.WriteLine("3- Delete a category");
            Console.WriteLine();

            Console.Write("Enter selection: ");
            var validInputs = new List<string>() { "1", "2", "3" };
            var menuSelection = UserInput.ValidInput(validInputs, Console.ReadLine());
            validInputs.Clear();
            Console.WriteLine();

            switch (menuSelection)
            {
                case "1":
                    Console.Write("New category name: ");
                    var newCatName = Console.ReadLine();
                    Console.WriteLine();
                    TablePrinter.DepartmentPrinter(departments);
                    Console.WriteLine();

                    foreach (var d in departments) validInputs.Add(d.DepartmentID.ToString());
                    Console.Write("Enter the ID of the department for this category: ");
                    var newDeptID = int.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
                    validInputs.Clear();

                    repo.InsertCategory(newCatName, newDeptID);
                    categories = GetCategories(conn);
                    Console.WriteLine();
                    UserInput.SuccessMessage($"SUCCESS: added category {newCatName}");
                    Console.WriteLine();
                    Console.WriteLine("UPDATED CATEGORY LIST:");
                    TablePrinter.BackEndCategoryPrinter(categories);
                    break;
                case "2":
                    TablePrinter.BackEndCategoryPrinter(categories);
                    Console.WriteLine();

                    foreach (var c in categories) validInputs.Add(c.CategoryID.ToString());

                    Console.Write("Enter category ID to edit: ");
                    var catID = int.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
                    var selectedCategory = CategoryByID(categories, catID);
                    validInputs.Clear();
                    Console.WriteLine();

                    Console.Write("You are about to edit: ");
                    UserInput.SuccessMessage(selectedCategory.Name);
                    Console.Write("Is that correct? (Y/N): ");
                    if (UserInput.YesOrNo(Console.ReadLine()))
                    {
                        Console.WriteLine("Which would you like to edit?");
                        Console.WriteLine("1- Category Name");
                        Console.WriteLine("2- Department");
                        validInputs.Add("1");
                        validInputs.Add("2");
                        menuSelection = UserInput.ValidInput(validInputs, Console.ReadLine());
                        validInputs.Clear();
                        Console.WriteLine();

                        switch (menuSelection)
                        {
                            case "1":
                                Console.Write("New category name: ");
                                newCatName = Console.ReadLine();
                                Console.Write("Type 'UPDATE' to confirm: ");
                                if (UserInput.ConfirmInput("UPDATE", Console.ReadLine()))
                                {
                                    repo.UpdateCategory(selectedCategory, newCatName);
                                    categories = GetCategories(conn);
                                    Console.WriteLine();
                                    UserInput.SuccessMessage($"SUCCESS: updated category {newCatName}");
                                    Console.WriteLine();
                                    Console.WriteLine("UPDATED CATEGORY LIST:");
                                    TablePrinter.BackEndCategoryPrinter(categories);
                                }
                                else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                                break;
                            case "2":
                                TablePrinter.DepartmentPrinter(departments);
                                Console.WriteLine();

                                foreach (var d in departments) validInputs.Add(d.DepartmentID.ToString());
                                Console.Write("Enter the ID of the new department for this category: ");
                                newDeptID = int.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
                                validInputs.Clear();

                                Console.Write("Type 'UPDATE' to confirm: ");
                                if (UserInput.ConfirmInput("UPDATE", Console.ReadLine()))
                                {
                                    repo.UpdateCategory(selectedCategory, newDeptID);
                                    categories = GetCategories(conn);
                                    selectedCategory = CategoryByID(categories, catID);
                                    Console.WriteLine();
                                    UserInput.SuccessMessage($"SUCCESS: updated category {selectedCategory.CategoryID} to {selectedCategory.Department} department");
                                    Console.WriteLine();
                                    Console.WriteLine("UPDATED CATEGORY LIST:");
                                    TablePrinter.BackEndCategoryPrinter(categories);
                                }
                                else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                                break;
                        }
                    }
                    else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    break;
                case "3":
                    TablePrinter.BackEndCategoryPrinter(categories);
                    Console.WriteLine();

                    foreach (var c in categories) validInputs.Add(c.CategoryID.ToString());

                    Console.Write("Enter the ID of the category to delete: ");
                    catID = int.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
                    selectedCategory = CategoryByID(categories, catID);

                    Console.Write("You are about to delete: ");
                    UserInput.SuccessMessage(selectedCategory.Name);
                    Console.Write("Is that correct? (Y/N): ");
                    if (UserInput.YesOrNo(Console.ReadLine()))
                    {
                        Console.WriteLine();
                        Console.Write("Type 'DELETE' to confirm: ");
                        if (UserInput.ConfirmInput("DELETE", Console.ReadLine()))
                        {
                            repo.DeleteCategory(selectedCategory);

                            categories = GetCategories(conn);
                            Console.WriteLine();
                            Console.WriteLine("UPDATED CATEGORY LIST:");
                            TablePrinter.BackEndCategoryPrinter(categories);
                        }
                        else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    }
                    else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    break;
            }
            Console.WriteLine();
            Console.WriteLine("Is there anything else I can help you with today? (Y/N)");
            if (UserInput.YesOrNo(Console.ReadLine()))
            {
                Console.WriteLine();
                EmployeeMenu(conn);
            }
            else Console.WriteLine("OK, have a great rest of your day!");
        }

        static void ProductBackEnd(IDbConnection conn)
        {
            var products = GetProducts(conn);
            var categories = GetCategories(conn);
            var repo = new Repositories.DapperProductRepository(conn);
            var selectedProduct = new Product();

            Console.WriteLine("BACK-END MENU");
            Console.WriteLine("1- Add new product");
            Console.WriteLine("2- Edit existing product");
            Console.WriteLine("3- Delete a product");
            Console.WriteLine();

            Console.Write("Enter selection: ");
            var validInputs = new List<string>() { "1", "2", "3" };
            var menuSelection = UserInput.ValidInput(validInputs, Console.ReadLine());
            validInputs.Clear();
            Console.WriteLine();

            switch (menuSelection)
            {
                case "1":
                    Console.Write("New product name: ");
                    var newName = Console.ReadLine();
                    Console.WriteLine();
                    TablePrinter.BackEndCategoryPrinter(categories);
                    Console.WriteLine();

                    foreach (var c in categories) validInputs.Add(c.CategoryID.ToString());
                    Console.Write("Enter the ID of the category for this product: ");
                    var newCatID = float.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
                    validInputs.Clear();

                    Console.Write("Enter a price for this product in USD: $");
                    var newPrice = UserInput.ParseNumber(Console.ReadLine());

                    Console.Write("How many do we have in stock?: ");
                    var newStock = Convert.ToInt32(UserInput.ParseNumber(Console.ReadLine()));

                    repo.InsertProduct(newName, newPrice, newCatID, newStock);
                    Console.WriteLine();
                    UserInput.SuccessMessage($"SUCCESS: added product {newName}");
                    Console.WriteLine();
                    var catProducts = ListMaker.ProductByCategory(GetProducts(conn), selectedProduct.CategoryID.ToString());
                    Console.WriteLine("UPDATED PRODUCT LIST:");
                    TablePrinter.BackEndProductPrinter(catProducts);
                    break;
                case "2":
                    selectedProduct = BackEndProductFinder(conn, "edit");
                    if (selectedProduct == null) break;
                    Console.Write("You are about to edit: ");
                    UserInput.SuccessMessage(selectedProduct.Name);
                    Console.Write("Is that correct? (Y/N): ");
                    if (UserInput.YesOrNo(Console.ReadLine()))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Which would you like to edit?");
                        Console.WriteLine("1- Product name");
                        Console.WriteLine("2- Category");
                        Console.WriteLine("3- Stock Level");
                        Console.WriteLine("4- Price");
                        Console.WriteLine();

                        for (int i = 1; i <= 4; i++) validInputs.Add(i.ToString());
                        menuSelection = UserInput.ValidInput(validInputs, Console.ReadLine());
                        validInputs.Clear();
                        switch (menuSelection)
                        {
                            case "1":
                                Console.Write("Enter new product name: ");
                                newName = Console.ReadLine();

                                Console.Write("Type 'UPDATE' to confirm: ");
                                if (UserInput.ConfirmInput("UPDATE", Console.ReadLine()))
                                {
                                    repo.UpdateProduct(selectedProduct, newName);
                                    Console.WriteLine();
                                    UserInput.SuccessMessage($"SUCCESS: updated product #{selectedProduct.ProductID}");
                                    Console.WriteLine();

                                    catProducts = ListMaker.ProductByCategory(GetProducts(conn), selectedProduct.CategoryID.ToString());
                                    Console.WriteLine("UPDATED PRODUCT LIST:");
                                    TablePrinter.BackEndProductPrinter(catProducts);
                                }
                                else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                                break;
                            case "2":
                                TablePrinter.BackEndCategoryPrinter(categories);
                                Console.WriteLine();

                                foreach (var c in categories) validInputs.Add(c.CategoryID.ToString());
                                Console.Write("Enter the ID of the new category for this product: ");
                                newCatID = float.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
                                validInputs.Clear();

                                Console.Write("Type 'UPDATE' to confirm: ");
                                if (UserInput.ConfirmInput("UPDATE", Console.ReadLine()))
                                {
                                    repo.UpdateProduct(selectedProduct, newCatID);
                                    var selectedProdID = selectedProduct.ProductID;
                                    catProducts = ListMaker.ProductByCategory(GetProducts(conn), newCatID.ToString());
                                    selectedProduct = ProductByID(catProducts, selectedProdID);
                                    Console.WriteLine();
                                    UserInput.SuccessMessage($"SUCCESS: updated product #{selectedProdID} to category {selectedProduct.Category}");
                                    Console.WriteLine();
                                    Console.WriteLine("UPDATED PRODUCT LIST:");
                                    TablePrinter.BackEndProductPrinter(catProducts);
                                }
                                else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                                break;
                            case "3":
                                Console.WriteLine($"Product stock is currently listed at {selectedProduct.StockLevel} units.");
                                Console.Write("Enter new stock level: ");
                                newStock = Convert.ToInt32(UserInput.ParseNumber(Console.ReadLine()));
                                Console.WriteLine();

                                Console.Write("Type 'UPDATE' to confirm: ");
                                if (UserInput.ConfirmInput("UPDATE", Console.ReadLine()))
                                {
                                    repo.UpdateProduct(selectedProduct, newStock);
                                    Console.WriteLine();
                                    UserInput.SuccessMessage($"SUCCESS: updated product #{selectedProduct.ProductID} to {newStock} units");
                                    Console.WriteLine();

                                    catProducts = ListMaker.ProductByCategory(GetProducts(conn), selectedProduct.CategoryID.ToString());
                                    Console.WriteLine("UPDATED PRODUCT LIST:");
                                    TablePrinter.BackEndProductPrinter(catProducts);
                                }
                                else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                                break;
                            case "4":
                                Console.WriteLine($"Product is currently {(selectedProduct.OnSale ? "on sale" : "listed")} for ${selectedProduct.Price}");
                                Console.Write("Enter new price: ");
                                newPrice = UserInput.ParseNumber(Console.ReadLine());
                                Console.WriteLine();
                                Console.Write("Should this be listed as a SALE price? (Y/N): ");
                                var newSale = UserInput.YesOrNo(Console.ReadLine());
                                Console.WriteLine();

                                Console.Write("Type 'UPDATE' to confirm: ");
                                if (UserInput.ConfirmInput("UPDATE", Console.ReadLine()))
                                {
                                    repo.UpdateProduct(selectedProduct, newPrice);
                                    repo.UpdateProduct(selectedProduct, newSale);
                                    Console.WriteLine();
                                    UserInput.SuccessMessage($"SUCCESS: updated pricing for product #{selectedProduct.ProductID}");
                                    Console.WriteLine();

                                    catProducts = ListMaker.ProductByCategory(GetProducts(conn), selectedProduct.CategoryID.ToString());
                                    Console.WriteLine("UPDATED PRODUCT LIST:");
                                    TablePrinter.BackEndProductPrinter(catProducts);
                                }
                                else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                                break;
                        }
                    }
                    else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    break;
                case "3":
                    selectedProduct = BackEndProductFinder(conn, "delete");
                    if (selectedProduct == null) break;
                    Console.Write("You are about to delete: ");
                    UserInput.SuccessMessage(selectedProduct.Name);
                    Console.Write("Is that correct? (Y/N): ");
                    if (UserInput.YesOrNo(Console.ReadLine()))
                    {
                        Console.WriteLine();
                        Console.Write("Type 'DELETE' to confirm: ");
                        if (UserInput.ConfirmInput("DELETE", Console.ReadLine()))
                        {
                            repo.DeleteProduct(selectedProduct);
                            Console.WriteLine();
                            catProducts = ListMaker.ProductByCategory(GetProducts(conn), selectedProduct.CategoryID.ToString());
                            Console.WriteLine("UPDATED PRODUCT LIST:");
                            TablePrinter.BackEndProductPrinter(catProducts);
                        }
                        else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    }
                    else UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    break;
            }
            if (selectedProduct == null) return;
            Console.WriteLine();
            Console.WriteLine("Is there anything else I can help you with today? (Y/N)");
            if (UserInput.YesOrNo(Console.ReadLine()))
            {
                Console.WriteLine();
                EmployeeMenu(conn);
            }
            else Console.WriteLine("OK, have a great rest of your day!");
        }

        static Product BackEndProductFinder(IDbConnection conn, string action)
        {
            var categories = GetCategories(conn);
            var products = GetProducts(conn);
            var validInputs = new List<string>();
            var selectedProduct = new Product();

            Console.Write($"Do you know the ID of the product you wish to {action}? (Y/N): ");
            if (UserInput.YesOrNo(Console.ReadLine()))
            {
                Console.WriteLine();
                foreach (var p in products) validInputs.Add(p.ProductID.ToString());
                Console.Write("Enter the Product ID: ");
                var prodID = int.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
                validInputs.Clear();
                selectedProduct = ProductByID(products, prodID);
            }
            else
            {
                Console.WriteLine();
                TablePrinter.BackEndCategoryPrinter(categories);

                Console.WriteLine();
                foreach (var c in categories) validInputs.Add(c.CategoryID.ToString());
                Console.Write("Enter a Category ID to narrow down the products list: ");
                var catProducts = ListMaker.ProductByCategory(products, UserInput.ValidInput(validInputs, Console.ReadLine()));
                validInputs.Clear();

                Console.WriteLine();
                TablePrinter.BackEndProductPrinter(catProducts);

                Console.WriteLine();
                Console.Write("Is the product listed here? (Y/N): ");
                if (UserInput.YesOrNo(Console.ReadLine()))
                {
                    foreach (var p in catProducts) validInputs.Add(p.ProductID.ToString());
                    Console.Write("Enter the product ID: ");
                    var prodID = int.Parse(UserInput.ValidInput(validInputs, Console.ReadLine()));
                    validInputs.Clear();
                    selectedProduct = ProductByID(products, prodID);
                }
                else
                {
                    UserInput.ErrorMessage("I'm sorry, I could not complete your request.");
                    EmployeeMenu(conn);
                    return null;
                }
            }
            Console.WriteLine();
            return selectedProduct;
        }

        static IDbConnection OpenStore()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string connString = config.GetConnectionString("DefaultConnection");
            return new MySqlConnection(connString);
        }

        static IEnumerable<Department> GetDepartments(IDbConnection conn)
        {
            var repo = new Repositories.DapperDepartmentRepository(conn);
            return repo.GetAllDepartments();
        }

        static IEnumerable<Category> GetCategories(IDbConnection conn)
        {
            var repo = new Repositories.DapperCategoryRepository(conn);
            return repo.GetAllCategories();
        }

        static IEnumerable<Product> GetProducts(IDbConnection conn)
        {
            var repo = new Repositories.DapperProductRepository(conn);
            return repo.GetAllProducts();
        }

        static IEnumerable<Review> GetReviews(IDbConnection conn)
        {
            var repo = new Repositories.DapperReviewRepository(conn);
            return repo.GetAllReviews();
        }

        static IEnumerable<Employee> GetEmployees(IDbConnection conn)
        {
            var repo = new Repositories.DapperEmployeeRepository(conn);
            return repo.GetAllEmployees();
        }

        static Department DepartmentByID(IEnumerable<Department> departments, int deptID)
        {
            Department dept = new Department();
            foreach (var d in departments)
            {
                if (d.DepartmentID == deptID) dept = d;
            }
            return dept;
        }

        static Product ProductByID(IEnumerable<Product> products, int prodID)
        {
            Product prod = new Product();
            foreach (var p in products)
            {
                if (p.ProductID == prodID) prod = p;
            }
            return prod;
        }

        static Category CategoryByID(IEnumerable<Category> categories, int catID)
        {
            Category cat = new Category();
            foreach (var c in categories)
            {
                if (c.CategoryID == catID) cat = c;
            }
            return cat;
        }

        static Employee EmployeeByID(IEnumerable<Employee> employees, int employeeID)
        {
            Employee employee = new Employee();
            foreach (var e in employees)
            {
                if (e.EmployeeID == employeeID) employee = e;
            }
            return employee;
        }
    }
}