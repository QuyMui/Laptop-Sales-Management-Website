using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyLaptop.Models
{
    public class CartItemModelcs
    {
        QLLaptopEntities1 da = new QLLaptopEntities1();
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public double Gia { get; set; }
        public int SoLuong { get; set; }
        public string ThongSoKyThuat { get; set; }
        public string AnhSP { get; set; }
        public double Total
        {
            get { return SoLuong * Gia; }
        }
        public CartItemModelcs(string id)
        {
            MaSP = id; 
            SanPham sp = da.SanPham.FirstOrDefault(s => s.MaSP == id);
            TenSP = sp.TenSP;
            AnhSP = sp.AnhSP;
            Gia = sp.Gia;
            SoLuong = 1;
        }




    }
}

