
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace InventoryManagement.Data
{
    public class InventoryDatabase
    {
        private readonly string _connectionString;

        public InventoryDatabase(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=InventoryDB.db";
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public async Task InitializeAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Products (
                    ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductName TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Price REAL NOT NULL,
                    Quantity INTEGER NOT NULL
                );";

            await command.ExecuteNonQueryAsync();
        }
    }
}
