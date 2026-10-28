
using InventoryManagement.Data;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace InventoryManagement.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryDatabase _database;

        public InventoryController(InventoryDatabase database)
        {
            _database = database;
        }

        // READ: Display all products
        public async Task<IActionResult> Index()
        {
            var products = new List<Product>();

            using var connection = _database.GetConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT ProductId, ProductName, Category, Price, Quantity FROM Products ORDER BY ProductId DESC";

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    ProductId = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Category = reader.GetString(2),
                    Price = Convert.ToDecimal(reader.GetDouble(3)),
                    Quantity = reader.GetInt32(4)
                });
            }

            return View(products);
        }

        // CREATE: Display the form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE: Save a new product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            using var connection = _database.GetConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Products (ProductName, Category, Price, Quantity)
                VALUES ($name, $category, $price, $quantity)";

            command.Parameters.AddWithValue("$name", product.ProductName);
            command.Parameters.AddWithValue("$category", product.Category);
            command.Parameters.AddWithValue("$price", product.Price);
            command.Parameters.AddWithValue("$quantity", product.Quantity);

            await command.ExecuteNonQueryAsync();

            return RedirectToAction(nameof(Index));
        }

        // UPDATE: Display the edit form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await FindProduct(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // UPDATE: Save changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(product);

            using var connection = _database.GetConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Products
                SET ProductName = $name,
                    Category = $category,
                    Price = $price,
                    Quantity = $quantity
                WHERE ProductId = $id";

            command.Parameters.AddWithValue("$name", product.ProductName);
            command.Parameters.AddWithValue("$category", product.Category);
            command.Parameters.AddWithValue("$price", product.Price);
            command.Parameters.AddWithValue("$quantity", product.Quantity);
            command.Parameters.AddWithValue("$id", product.ProductId);

            await command.ExecuteNonQueryAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE: Confirm deletion
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await FindProduct(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // DELETE: Remove the product
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var connection = _database.GetConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Products WHERE ProductId = $id";
            command.Parameters.AddWithValue("$id", id);

            await command.ExecuteNonQueryAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<Product?> FindProduct(int id)
        {
            using var connection = _database.GetConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT ProductId, ProductName, Category, Price, Quantity
                FROM Products
                WHERE ProductId = $id";

            command.Parameters.AddWithValue("$id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Product
            {
                ProductId = reader.GetInt32(0),
                ProductName = reader.GetString(1),
                Category = reader.GetString(2),
                Price = Convert.ToDecimal(reader.GetDouble(3)),
                Quantity = reader.GetInt32(4)
            };
        }
    }
}
