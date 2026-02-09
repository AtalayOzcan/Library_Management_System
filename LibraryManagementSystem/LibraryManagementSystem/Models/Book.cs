namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int PublishYear { get; set; }
        public int PageCount { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int StockCount { get; set; }
        public bool IsAvailable { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
