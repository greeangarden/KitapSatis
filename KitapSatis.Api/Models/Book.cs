namespace KitapSatis.Api.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public string? ISBN { get; set; }
        public string? Language { get; set; }
        public string? Dimensions { get; set; }
        public int? PublicationYear { get; set; }
        public int? PageCount { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public decimal? ShippingFee { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
