namespace KitapSatis.Api.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
