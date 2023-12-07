using Ecommerce_Mvc.Data;
using Microsoft.EntityFrameworkCore;

// Create a web application builder with command-line arguments.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext service for database interactions.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AppContextDB") ?? string.Empty)
);

/*
    **Cookies: Used to store shopping cart information on the client-side, 
 allowing persistence even when the browser is closed and reopened.
   
    **Session: Used to store and retrieve session information, including serialized
    shopping cart data, providing a way to maintain data across different pages of the application.
   
    **Cache: AddDistributedMemoryCache adds an in-memory cache to temporarily store session data. 
   This cache is used by session 
   services, enhancing performance by storing data between HTTP requests on the server side.
 */

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Set the maximum idle time for the session (e.g., 30 minutes).
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Configure cookie properties for session management.
    options.Cookie.HttpOnly = true;     // Ensures the cookie is only accessible through HTTP requests.
    options.Cookie.IsEssential = true;  // Indicates that the session cookie is essential for the application.
});

// Add logging services.
builder.Services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Information);
    builder.AddConsole();
    // Add other logging providers as needed
});

// Add controllers and views services.
builder.Services.AddControllersWithViews();

// Build the web application.
var app = builder.Build();

// Configure the HTTP request pipeline.

// If not in development mode, handle exceptions and enforce HTTPS.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Enable session usage in the application.
app.UseSession();

// Map controller route for handling requests.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application.
app.Run();