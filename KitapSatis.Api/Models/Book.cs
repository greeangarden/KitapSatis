namespace KitapSatis.Api.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
