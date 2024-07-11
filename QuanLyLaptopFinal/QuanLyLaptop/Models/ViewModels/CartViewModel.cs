using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuanLyLaptop.Models;
namespace QuanLyLaptop.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemModelcs> CartItems { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
