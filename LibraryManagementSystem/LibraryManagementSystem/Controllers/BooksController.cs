using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        private readonly Context _context;

        public BooksController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            var books = _context.Books
                .Include(x => x.Category)
                .Include(x => x.Publisher)
                .ToList();
            return View(books);
        }
        [HttpGet]
        public IActionResult Create()
        {
            List<SelectListItem> categoryValues = (from x in _context.Categorys.ToList()
                                                   select new SelectListItem
                                                   {
                                                       Text = x.CategoryName, 
                                                       Value = x.CategoryId.ToString() 
                                                   }).ToList();

            ViewBag.Categories = categoryValues;

            List<SelectListItem> publisherValues = (from x in _context.Publishers.ToList()
                                                    select new SelectListItem
                                                    {
                                                        Text = x.PublisherName,
                                                        Value = x.PublisherId.ToString()
                                                    }).ToList();

            ViewBag.Publishers = publisherValues;

            return View();
        }
        [HttpPost]
        public IActionResult Create(Book book)
        {
            book.CreatedDate = DateTime.UtcNow;
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var books = _context.Books.Find(id);
            _context.Remove(books);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Search(string isbn)
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return View();
            }

            // 2. Veritabanında ISBN'i ara (FirstOrDefault ilk bulduğunu getirir)
            var book = _context.Books.FirstOrDefault(x => x.ISBN == isbn);

            // 3. Kitap Bulunduysa -> Edit sayfasına yönlendir (veya Details)
            if (book != null)
            {
                TempData["Message"] = $"The book you are looking for has been found: {book.Title} - Stock Status: {book.StockCount}";
                return RedirectToAction("Index", new { id = book.BookId });       
            }

            // 4. Kitap Bulunamadıysa -> Hata mesajı ver ve sayfada kal
            ViewBag.Message = "No books were found with this ISBN number!";
            return View();
        }
    }
}
