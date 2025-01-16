namespace ECommerceShopingCartASPNET8.Models
{
    public class Item  // This is the CartItem class with modifications
    {
        public int Id { get; set; }  // Primary key for EF Core
        public string CartId { get; set; }  // Foreign key to the Cart

        public Product Product { get; set; }  // Navigation to Product
        public string ProductId { get; set; }  // Foreign key to Product table

        public int Quantity { get; set; }

        // Calculate cost for each item
        public double Cost => Product.Price * Quantity;

        // Navigation property to Cart
        public Cart Cart { get; set; }
    }

}
