using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace LibraryManagementSystem.Controllers
{
    public class CategoryController : Controller
    {
        private readonly Context _context;

        public CategoryController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categorys.ToList();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            category.CreatedDate = DateTime.UtcNow;
            _context.Categorys.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categorys.Find(id);
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            var categoryToUpdate = _context.Categorys.Find(category.CategoryId);

            if (categoryToUpdate != null)
            {
                categoryToUpdate.CategoryName = category.CategoryName;
                categoryToUpdate.Description = category.Description;

                _context.SaveChanges();
            }

                return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Categorys.Find(id);
            _context.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
