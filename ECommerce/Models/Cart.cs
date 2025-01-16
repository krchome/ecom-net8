using System.ComponentModel.DataAnnotations;
namespace ECommerceShopingCartASPNET8.Models
{
    public class Cart
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public List<Item> Items { get; set; } = new List<Item>(); // A list of items in the cart
        public double TotalCost => Items.Sum(item => item.Cost); // Calculate the total cost based on the costs of all items

        
    }
}
