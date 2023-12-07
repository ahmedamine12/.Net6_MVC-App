using Ecommerce_Mvc.Data;
using Ecommerce_Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Ecommerce_Mvc.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ApplicationDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                IEnumerable<Category> categoryList = _context.Categories;
                _logger.LogInformation("\x1b[34m**********List categories retrieved successfully**********\x1b[0m");
                return View(categoryList);
            }
            catch (Exception ex)
            {
                _logger.LogError("\x1b[34mError in Index action: {ErrorMessage}\x1b[0m", ex.Message);
                TempData["ErrorMsg"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Categories.Add(category);
                    _context.SaveChanges();
                    TempData["SuccessMsg"] = $"Category ({category.CategoryName}) added successfully.";
                    _logger.LogInformation($"\x1b[34m**********Category ({category.CategoryName}) added successfully.**********\x1b[0m");
                    return RedirectToAction("Index");
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError("\x1b[34m**********Error in Create action: {ErrorMessage}\x1b[0m**********", ex.Message);
                TempData["ErrorMsg"] = "An error occurred while creating the category.";
                return RedirectToAction("Create");
            }
        }

        public IActionResult Edit(int? categoryId)
        {
            try
            {
                var category = _context.Categories.Find(categoryId);

                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError("\x1b[34m**********Error in Edit action: {ErrorMessage}\x1b[0m**********", ex.Message);
                TempData["ErrorMsg"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Categories.Update(category);
                    _context.SaveChanges();
                    _logger.LogInformation($"\x1b[34m**********Category ({category.CategoryName}) updated successfully.**********\x1b[0m");
                    TempData["SuccessMsg"] = $"Category ({category.CategoryName}) updated successfully.";
                    return RedirectToAction("Index");
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError("\x1b[34m**********Error in Edit action: {ErrorMessage}\x1b[0m**********", ex.Message);
                TempData["ErrorMsg"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Delete(int? categoryId)
        {
            try
            {
                var category = _context.Categories.Find(categoryId);

                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError("\x1b[34m**********Error in Delete action: {ErrorMessage}\x1b[0m**********", ex.Message);
                TempData["ErrorMsg"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int? categoryId)
        {
            try
            {
                var category = _context.Categories.Find(categoryId);
                if (category == null)
                {
                    return NotFound();
                }
                _context.Categories.Remove(category);
                _context.SaveChanges();
                TempData["SuccessMsg"] = $"Category ({category.CategoryName}) deleted successfully.";
                _logger.LogInformation($"\x1b[34m**********Category ({category.CategoryName}) deleted successfully.**********\x1b[0m");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError("\x1b[34m**********Error in DeleteCategory action: {ErrorMessage}\x1b[0m**********", ex.Message);
                TempData["ErrorMsg"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
            }
        }
    }
}
