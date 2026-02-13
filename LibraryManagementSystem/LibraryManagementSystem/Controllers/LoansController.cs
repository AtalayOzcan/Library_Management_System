using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class LoansController : Controller
    {
        private readonly Context _context;

        public LoansController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Lend()
        {

            ViewBag.Members = _context.Members
                .Where(m => m.IsActive == true).ToList();

            ViewBag.Books = _context.Books
                .Where(b => b.StockCount > 0).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Lend(int MemberId, int BookId)
        {
            // Kitabı veritabanından bul
            var book = _context.Books.Find(BookId);

            if (book == null || book.StockCount <= 0)
            {
                TempData["Error"] = "This book is out of stock!";
                return RedirectToAction("Lend");
            }

            // Kural 1: Üyenin kaç aktif ödüncü var kontrol et
            int activeLoanCount = _context.Loans
                .Count(l => l.MemberId == MemberId && l.IsReturned == false);// Bütün geçmiş kaydı saymamak için false olanlar yani daha dönmeyenler sayısı

            if (activeLoanCount > 3)
            {
                TempData["Error"] = "This member already has 3 active loans. Cannot borrow more!";
                return RedirectToAction("Lend");
            }

            // Kural 2: Üyenin 14 günden fazla gecikmiş kitabı var mı kontrol et
            bool hasOverDue = _context.Loans
                .Any(l => l.MemberId == MemberId
                && l.IsReturned == false
                && l.DueDate < DateTime.UtcNow.AddDays(-14));

            if (hasOverDue == true)
            {
                TempData["Error"] = "This member has an overdue book (14+ days). Must return it first!";
                return RedirectToAction("Lend");
            }

            var loan = new Loan();
                loan.BookId = BookId;
                loan.MemberId = MemberId;
                loan.LoanDate = DateTime.UtcNow;
                loan.DueDate = DateTime.UtcNow.AddDays(14);
                loan.IsReturned = false;
                loan.LateDays = 0;
                loan.Fine = 0;
                loan.Notes = "";

            // Kitap stokunu 1 azalt,
            book.StockCount = book.StockCount - 1;

            if (book.StockCount == 0)
            {
                book.IsAvailable = false;
            }

            _context.Loans.Add(loan);
            _context.SaveChanges();

            TempData["Success"] = "Book succesfully loaned!";
            return RedirectToAction("Active");
        }

        public IActionResult Active()
        {
            var loans = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.IsReturned == false)
                .ToList();
            return View(loans);
        }

        [HttpGet]
        public IActionResult Return()
        {
            var loans = _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.IsReturned == false)
            .ToList();

            return View(loans);
        }

        [HttpPost]
        public IActionResult Return(int LoanId)
        {
            var loan = _context.Loans
                .Include(l => l.Book)
                .FirstOrDefault(l => l.LoanId == LoanId);

            if(loan == null)
            {
                TempData["Error"] = "Loan record not found!";
                return RedirectToAction("Return");
            }

            // İade tarihini bugün olarak kaydet
            loan.ReturnDate = DateTime.UtcNow;
            loan.IsReturned = true;

            // Gecikme hesapla (bugün iade tarihini geçtiyse)
            if (DateTime.UtcNow > loan.DueDate)
            {
                loan.LateDays = (int)(DateTime.UtcNow - loan.DueDate).TotalDays;

                loan.Fine = loan.LateDays * 100;
            }

            loan.Book.StockCount = loan.Book.StockCount + 1;

            loan.Book.IsAvailable = true;

            _context.SaveChanges();

            TempData["Success"] = "Book successfully returned!";
            return RedirectToAction("Active");
        }

        public IActionResult Overdue()
        {
            
            var loans = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.IsReturned == false && l.DueDate < DateTime.UtcNow)
                .ToList();

            // Her biri için güncel gecikme ve cezayı hesapla
            foreach (var l in loans)
            {
                l.LateDays = (int)(DateTime.UtcNow - l.DueDate).TotalDays;
                l.Fine = l.LateDays * 100;
            }

            return View(loans);
        }

        public IActionResult History(string dateFilter, string memberFilter)
        {
            // Tüm ödünç kayıtlarını getir
            var loans = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .AsQueryable();

            // Üye adına göre filtrele
            if (!string.IsNullOrEmpty(memberFilter))
            {
                loans = loans.Where(l => l.Member.FirstName.Contains(memberFilter) || l.Member.LastName.Contains(memberFilter));
            }

            // Tarihe göre filtrele (örn: "2024-01" gibi yıl-ay formatı)
            if (!string.IsNullOrEmpty(dateFilter))
            {
                // Kullanıcının girdiği tarihi parse et
                if (DateTime.TryParse(dateFilter, out DateTime parsedDate))
                {
                    loans = loans.Where(l => l.LoanDate.Year == parsedDate.Year && l.LoanDate.Month == parsedDate.Month);
                }
            }

            // Filtreleri tekrar view'e gönder (input'ta kalsın diye)
            ViewBag.DateFilter = dateFilter;
            ViewBag.MemberFilter = memberFilter;

            return View(loans.ToList());
        }
    }
}
