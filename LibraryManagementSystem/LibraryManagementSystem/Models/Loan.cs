namespace LibraryManagementSystem.Models
{
    public class Loan
    {
        public int LoanId { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; } 
        public int MemberId { get; set; }
        public virtual Member Member { get; set; } 
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public int LateDays { get; set; }
        public decimal Fine { get; set; }
        public string Notes { get; set; }

    }
}
