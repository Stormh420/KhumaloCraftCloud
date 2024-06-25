// Models/AccountViewModel.cs

using KhumaloCraftPOE.Models.Entities;
using System.Collections.Generic;

namespace KhumaloCraftPOE.Models
{
    public class AccountViewModel
    {
        public User User { get; set; }
        public List<Order> Orders { get; set; }
        public List<Product> ProductsForSale { get; set; }
        public List<Order> OrdersOnMyProducts { get; set; }

        public AccountViewModel(User user, List<Order> orders, List<Product> productsForSale, List<Order> ordersOnMyProducts)
        {
            User = user;
            Orders = orders;
            ProductsForSale = productsForSale;
            OrdersOnMyProducts = ordersOnMyProducts;
        }
    }
}




