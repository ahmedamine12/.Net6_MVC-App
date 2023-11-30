using System.Text.Json;
using Ecommerce_Mvc.Data;
using Ecommerce_Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;


using Ecommerce_Mvc.ViewModel;
using Ecommerce_Mvc.ViewModel;
using Microsoft.EntityFrameworkCore;

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

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.CategoryName == category);
            }

            // Project the results into ProductListViewModel
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
                    Quantity = 0  // Set quantity as needed
                })
                .ToList();

            // Set categories to ViewBag
            ViewBag.Categories = _context.Categories.ToList();

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

                // Save the shopping cart in a cookie with an expiration time (e.g., 7 days)
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true
                };

                Response.Cookies.Append("ShoppingCart", shoppingCartJson, cookieOptions);

                TempData["SuccessMsg"] = $"Added {quantity} {(quantity > 1 ? "items" : "item")} to the shopping cart.";

                return RedirectToAction("ViewCart");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddToCart action: {ex.Message}");
                TempData["ErrorMsg"] = "An error occurred while adding the product to the shopping cart.";
                return RedirectToAction("Index");
            }
        }


        public IActionResult ViewCart()
        {
            var shoppingCartJson = HttpContext.Request.Cookies["ShoppingCart"];
            var shoppingCart = !string.IsNullOrEmpty(shoppingCartJson)
                ? JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartJson)
                : new ShoppingCart();

            // Log the shopping cart content for debugging purposes
            Console.WriteLine($"Shopping Cart Items: {JsonConvert.SerializeObject(shoppingCart.Items)}");

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

            // Set the CartProducts to ViewBag
            ViewBag.CartProducts = cartProductViewModels;

            // Retrieve all products for the main view
            var productListViewModels = GetProductListViewModels();

            // Return the main view with the cart products
            return View("Index", productListViewModels);
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
