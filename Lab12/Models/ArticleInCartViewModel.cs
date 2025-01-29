namespace Lab12.Models
{
    public class ArticleInCart
    {
        public int ArticleId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }        
        public string? ImagePath { get; set; }
        public int CategoryId { get; set; }
    }
}
