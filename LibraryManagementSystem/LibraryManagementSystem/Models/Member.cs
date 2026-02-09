namespace LibraryManagementSystem.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public string MemberNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
    }
}
