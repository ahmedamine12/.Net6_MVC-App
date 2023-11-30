using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Mvc.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Display(Name ="Product Name")]
    public string? Name { get; set; }
    public string? Description { get; set; }
    [Required]
    public string? Color { get; set; }
    [Required]
    public int Price { get; set; }
    [Required(ErrorMessage = "Image URL is required")]
    public string? Image { get; set; }
    [Required]
    public int CategoryId { get; set; }       
    
    public Category Category { get; set; }
}