namespace Ecommerce_Mvc.Models
{
    public class ShoppingCartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        public void AddProduct(int productId, int quantity)
        {
            var existingItem = Items.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                // If the product is already in the cart, update the quantity
                existingItem.Quantity += quantity;
            }
            else
            {
                // If the product is not in the cart, add it with the specified quantity
                Items.Add(new ShoppingCartItem { ProductId = productId, Quantity = quantity });
            }
        }

        public void RemoveProduct(int productId)
        {
            var existingItem = Items.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                // If the product is in the cart, remove it
                Items.Remove(existingItem);
            }
        }
    }
}