using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class MembersController : Controller
    {
        private readonly Context _context;

        public MembersController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var members = _context.Members.ToList();
            return View(members);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Member member)
        {
            member.RegistrationDate = DateTime.UtcNow;
            var randomNumber = new Random();
            member.MemberNumber = randomNumber.Next(100000, 400000).ToString();
            _context.Members.Add(member);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var member = _context.Members.Find(id);
            _context.Remove(member);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Search(string memberNumber, string name, string email)
        {
            ViewBag.MemberNumber = memberNumber;
            ViewBag.Name = name;
            ViewBag.Email = email;

            if (string.IsNullOrEmpty(memberNumber) && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email))
            {
                return View(new List<Member>());
            }

            var query = _context.Members.AsQueryable();

            if (!string.IsNullOrEmpty(memberNumber))
            {
                query = query.Where(x => x.MemberNumber.ToLower().Contains(memberNumber));
            }

            if (!string.IsNullOrEmpty(name)) 
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(name));
            }
            if (!string.IsNullOrEmpty(email)) 
            {
                query = query.Where(x => x.Email.ToLower().Contains(email));
            }
            var resultMembers = query.ToList();

            return View(resultMembers);
        }
    }
}
