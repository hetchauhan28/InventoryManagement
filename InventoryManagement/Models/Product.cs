
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Range(0.01, 1000000, ErrorMessage = "Enter a valid price")]
        public decimal Price { get; set; }

        [Range(0, 1000000, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }
    }
}
