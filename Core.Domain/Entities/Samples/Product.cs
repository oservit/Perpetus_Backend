using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities.Samples;

[Table("products")]
public class Product
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("name")]
    public string Name { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("unit_price")]
    public decimal? UnitPrice { get; set; }

    [Column("stock_quantity")]
    public int? StockQuantity { get; set; }

    [Column("unit_of_measure")]
    public string? UnitOfMeasure { get; set; }

    [Column("manufacturer")]
    public string? Manufacturer { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}