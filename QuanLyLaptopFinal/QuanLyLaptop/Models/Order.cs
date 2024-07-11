using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyLaptop.Models
{
        public class OrderDetailModel
        {
            public int MaDH { get; set; }
            public DateTime NgayDat { get; set; }
            public string MaSP { get; set; }
            public int SoLuong { get; set; }
            public double Gia { get; set; }
            public string TenSP { get; set; }
        }

        public class OrderModel
        {
            public int MaDH { get; set; }
            public DateTime NgayDat { get; set; }
            public string UserName { get; set; }
            public List<OrderDetailModel> OrderDetails { get; set; }
        }
}

