using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ecommerce_Mvc.Models;
using Ecommerce_Mvc.ViewModel;
using Ecommerce_Mvc.Controllers;
namespace Ecommerce_Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly byte[] _jwtSecretKey;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSecretKey = GenerateJwtSecretKey();
        }

        // Registration action
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Generate a JWT token
                    var token = GenerateJwtToken(model.Email);

                    // You can do something with the token, such as storing it in a cookie

                    // Redirect to the login page after successful registration
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }




        // Login action
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Generate a JWT token
                    var token = GenerateJwtToken(model.Email);
                    var currentUser = await _userManager.FindByEmailAsync(model.Email);
                    Console.WriteLine("hiiiiiiiiiiiiiiiiii***" + currentUser);

                    // Store the token in a cookie
                    Response.Cookies.Append("JwtToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        // Other cookie options as needed
                    });

                    // Redirect to the product page after successful login
                    return RedirectToAction("Index", "Product");
                }
                else if (result.RequiresTwoFactor)
                {
                    // Handle two-factor authentication if it's enabled for the user
                    // You may redirect to a two-factor authentication page
                }
                else if (result.IsLockedOut)
                {
                    // Handle account lockout
                    // You may redirect to a lockout page
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                }
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await _signInManager.SignOutAsync();

            // Remove the JWT token cookie
            Response.Cookies.Delete("JwtToken");

            // Redirect to the home page or another desired page after logout
            return RedirectToAction("Index", "Product");
        }


        private string GenerateJwtToken(string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, email),
                // Add additional claims as needed
            };

            // Add debug statement or log here
            Console.WriteLine($"Claims in JWT Token: {string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}"))}");

            var key = new SymmetricSecurityKey(_jwtSecretKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MVCapp",
                audience: "MVCapp",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private byte[] GenerateJwtSecretKey()
        {
            var keyBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }

            return keyBytes;
        }
    }
}
