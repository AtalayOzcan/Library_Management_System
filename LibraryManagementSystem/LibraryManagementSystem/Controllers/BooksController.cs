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
        public IActionResult Search(string isbn, string bookTitle, string author)
        {
            ViewBag.Isbn = isbn;
            ViewBag.BookTitle = bookTitle;
            ViewBag.Author = author;

            if (string.IsNullOrEmpty(isbn) && string.IsNullOrEmpty(bookTitle) && string.IsNullOrEmpty(author))
            {
                return View(new List<Book>()); // Boş liste gönderiyoruz
            }

            // 3. Sorguyu Hazırla
            var query = _context.Books
                .Include(x => x.Category)
                .Include(x => x.Publisher)
                .AsQueryable();

            // 4. Filtreleri Uygula
            if (!string.IsNullOrEmpty(isbn)) query = query.Where(x => x.ISBN.Contains(isbn));
            if (!string.IsNullOrEmpty(bookTitle)) query = query.Where(x => x.Title.Contains(bookTitle));
            if (!string.IsNullOrEmpty(author)) query = query.Where(x => x.Author.Contains(author));

            // 5. Sonuçları listeye çevir ve AYNI SAYFAYA model olarak gönder
            var resultBooks = query.ToList();
            return View(resultBooks);
        }
    }
}
