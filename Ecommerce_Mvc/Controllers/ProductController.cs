using System;
using System.Collections.Generic;
using System.Linq;
using Ecommerce_Mvc.Data;
using Ecommerce_Mvc.Models;
using Ecommerce_Mvc.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ecommerce_Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

         public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, string category)
        {
            IQueryable<Product> products = _context.Products.Include(p => p.Category);

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.CategoryName == category);
            }

            var productListViewModels = products
                .Select(p => new ProductListViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Color = p.Color,
                    Image = p.Image,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    Quantity = 0
                })
                .ToList();

            ViewBag.Categories = _context.Categories.ToList();

            var shoppingCart = HttpContext.Request.Cookies.ContainsKey("ShoppingCart")
                ? JsonConvert.DeserializeObject<ShoppingCart>(HttpContext.Request.Cookies["ShoppingCart"])
                : new ShoppingCart();
            var cartProductIds = shoppingCart.Items.Select(item => item.ProductId).ToList();

            // Retrieve products from the database based on the product IDs
            var cartProducts = _context.Products
                .Include(p => p.Category)
                .Where(p => cartProductIds.Contains(p.Id))
                .ToList();

            // Create a list of ProductListViewModel by joining products and shopping cart items
            var cartProductViewModels = cartProducts
                .Join(shoppingCart.Items,
                    product => product.Id,
                    cartItem => cartItem.ProductId,
                    (product, cartItem) => new ProductListViewModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Color = product.Color,
                        Image = product.Image,
                        CategoryId = product.CategoryId,
                        Quantity = cartItem.Quantity,
                        CategoryName = product.Category.CategoryName
                    })
                .ToList();

            // Set the CartProducts to ViewBag
            ViewBag.CartProducts = cartProductViewModels;

            HttpContext.Session.SetString("ShoppingCart", JsonConvert.SerializeObject(shoppingCart));

            return View(productListViewModels);
        }

        public IActionResult Create()
        {
            ProductViewModel productCreateViewModel = new ProductViewModel();
            productCreateViewModel.Category = (IEnumerable<SelectListItem>)_context.Categories.Select(c =>
                new SelectListItem()
                {
                    Text = c.CategoryName,
                    Value = c.CategoryId.ToString()
                });

            return View(productCreateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel productCreateViewModel)
        {
            productCreateViewModel.Category = (IEnumerable<SelectListItem>)_context.Categories.Select(c =>
                new SelectListItem()
                {
                    Text = c.CategoryName,
                    Value = c.CategoryId.ToString()
                });
            var product = new Product()
            {
                Name = productCreateViewModel.Name,
                Description = productCreateViewModel.Description,
                Price = productCreateViewModel.Price,
                Color = productCreateViewModel.Color,
                CategoryId = productCreateViewModel.CategoryId,
                Image = productCreateViewModel.Image
            };
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                TempData["SuccessMsg"] = "Product (" + product.Name + ") added successfully.";
                return RedirectToAction("Index");
            }

            return View(productCreateViewModel);
        }

        public IActionResult Edit(int? id)
        {
            var productToEdit = _context.Products.Find(id);
            if (productToEdit != null)
            {
                var productViewModel = new ProductViewModel()
                {
                    Id = productToEdit.Id,
                    Name = productToEdit.Name,
                    Description = productToEdit.Description,
                    Price = productToEdit.Price,
                    CategoryId = productToEdit.CategoryId,
                    Color = productToEdit.Color,
                    Image = productToEdit.Image,
                    Category = (IEnumerable<SelectListItem>)_context.Categories.Select(c => new SelectListItem()
                    {
                        Text = c.CategoryName,
                        Value = c.CategoryId.ToString()
                    })
                };
                return View(productViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductViewModel productViewModel)
        {
            productViewModel.Category = (IEnumerable<SelectListItem>)_context.Categories.Select(c =>
                new SelectListItem()
                {
                    Text = c.CategoryName,
                    Value = c.CategoryId.ToString()
                });
            var product = new Product()
            {
                Id = productViewModel.Id,
                Name = productViewModel.Name,
                Description = productViewModel.Description,
                Price = productViewModel.Price,
                Color = productViewModel.Color,
                CategoryId = productViewModel.CategoryId,
                Image = productViewModel.Image
            };
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                TempData["SuccessMsg"] = "Product (" + product.Name + ") updated successfully !";
                return RedirectToAction("Index");
            }

            return View(productViewModel);
        }

        public IActionResult Delete(int? id)
        {
            var productToEdit = _context.Products.Find(id);
            if (productToEdit != null)
            {
                var productViewModel = new ProductViewModel()
                {
                    Id = productToEdit.Id,
                    Name = productToEdit.Name,
                    Description = productToEdit.Description,
                    Price = productToEdit.Price,
                    CategoryId = productToEdit.CategoryId,
                    Color = productToEdit.Color,
                    Image = productToEdit.Image,
                    Category = (IEnumerable<SelectListItem>)_context.Categories.Select(c => new SelectListItem()
                    {
                        Text = c.CategoryName,
                        Value = c.CategoryId.ToString()
                    })
                };
                return View(productViewModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int? id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            TempData["SuccessMsg"] = "Product (" + product.Name + ") deleted successfully.";
            return RedirectToAction("Index");
        }

         [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId, int quantity)
        {
            try
            {
                var shoppingCart = HttpContext.Request.Cookies.ContainsKey("ShoppingCart")
                    ? JsonConvert.DeserializeObject<ShoppingCart>(HttpContext.Request.Cookies["ShoppingCart"])
                    : new ShoppingCart();

                shoppingCart.AddProduct(productId, quantity);

                var shoppingCartJson = JsonConvert.SerializeObject(shoppingCart);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Path = "/" // Set the appropriate path
                };

                Response.Cookies.Append("ShoppingCart", shoppingCartJson, cookieOptions);
                HttpContext.Session.SetString("ShoppingCart", shoppingCartJson);

                TempData["SuccessMsg"] = $"Added {quantity} {(quantity > 1 ? "items" : "item")} to the shopping cart.";

                return RedirectToAction("ViewCart");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddToCart action: {ex.Message}");
                TempData["ErrorMsg"] = "An error occurred while adding the product to the shopping cart.";
                return RedirectToAction("ViewCart");
            }
        }

        public IActionResult ViewCart()
        {
            try
            {
                var shoppingCart = HttpContext.Request.Cookies.ContainsKey("ShoppingCart")
                    ? JsonConvert.DeserializeObject<ShoppingCart>(HttpContext.Request.Cookies["ShoppingCart"])
                    : new ShoppingCart();

                var cartProductIds = shoppingCart.Items.Select(item => item.ProductId).ToList();
                var cartProducts = _context.Products
                    .Include(p => p.Category)
                    .Where(p => cartProductIds.Contains(p.Id))
                    .ToList();

                var cartProductViewModels = cartProducts
                    .Join(shoppingCart.Items,
                        product => product.Id,
                        cartItem => cartItem.ProductId,
                        (product, cartItem) => new ProductListViewModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            Color = product.Color,
                            Image = product.Image,
                            CategoryId = product.CategoryId,
                            Quantity = cartItem.Quantity,
                            CategoryName = product.Category.CategoryName
                        })
                    .ToList();

                ViewBag.CartProducts = cartProductViewModels;
                HttpContext.Session.SetString("ShoppingCart", JsonConvert.SerializeObject(shoppingCart));

                var productListViewModels = GetProductListViewModels();

                ViewBag.Categories = _context.Categories.ToList();

                return View("Index", productListViewModels);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ViewCart action: {ex.Message}");
                TempData["ErrorMsg"] = "An error occurred while retrieving the shopping cart.";
                return View("Index", GetProductListViewModels());
            }
        }

        private List<ProductListViewModel> GetProductListViewModels()
        {
            IQueryable<Product> products = _context.Products.Include(p => p.Category);

            var productListViewModelList = products.Select(item => new ProductListViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Color = item.Color,
                Price = item.Price,
                CategoryId = item.CategoryId,
                Image = item.Image,
                CategoryName = item.Category.CategoryName
            }).ToList();

            return productListViewModelList;
        }
    }
}
