using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Samples.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? StockQuantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateProductDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? StockQuantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? Manufacturer { get; set; }
    }

    public class ProductDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? StockQuantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
