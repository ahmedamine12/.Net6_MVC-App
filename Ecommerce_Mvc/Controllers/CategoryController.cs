using Ecommerce_Mvc.Data;
using Ecommerce_Mvc.Models;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce_Mvc.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categoryList = _context.Categories;
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {               
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["SuccessMsg"] = "Category (" + category.CategoryName + ") added successfully.";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int? categoryId)
        {            
            var category = _context.Categories.Find(categoryId);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["SuccessMsg"] = "Category (" + category.CategoryName + ") updated successfully.";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int? categoryId)
        {
            var category = _context.Categories.Find(categoryId);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int? categoryId)
        {
            var category = _context.Categories.Find(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["SuccessMsg"] = "Category (" + category.CategoryName + ") deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
