using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class PublishersController : Controller
    {
        private readonly Context _context;

        public PublishersController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var publishers = _context.Publishers.ToList();
            return View(publishers);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Publisher publisher)
        {
            _context.Publishers.Add(publisher);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var publishers = _context.Publishers.Find(id);
            _context.Remove(publishers);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var publisher = _context.Publishers.Find(id);
            return View(publisher);
        }
        [HttpPost]
        public IActionResult Edit(Publisher publisher)
        {
            var publisherToUpdate = _context.Publishers.Find(publisher.PublisherId);

            if(publisherToUpdate != null)
            {
                publisherToUpdate.PublisherName = publisher.PublisherName;

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
