using System;
using System.ComponentModel.DataAnnotations;

namespace SCIProductsAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The product name is required.")]
        [StringLength(200, ErrorMessage = "The product name must not exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "The description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "The price must be greater than or equal to 0.")]
        public decimal Price { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }
    }
}
