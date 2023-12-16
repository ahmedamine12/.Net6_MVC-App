using Microsoft.AspNetCore.Identity;
using System;

namespace Ecommerce_Mvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
       
        // Add any other properties you need...

        // You can also add navigation properties if needed
        // public ICollection<Order> Orders { get; set; }
    }
}