using QuanLyLaptop.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;

namespace QuanLyLaptop.Controllers
{
    public class CartController : Controller
    {
        QLLaptopEntities1 da = new QLLaptopEntities1();
        // GET: Cart
        private List<CartItemModelcs> GetCarts()
        {
            List<CartItemModelcs> carts = Session["CartModel"] as List<CartItemModelcs>;
            if (carts == null)//chua co sp nao trong gio hang
            {
                carts = new List<CartItemModelcs>();
                Session["CartModel"] = carts;
            }
            return carts;
        }
        public ActionResult ListCarts()
        {
            List<CartItemModelcs> carts = GetCarts();
            ViewBag.TongSoLuong = TongSL();
            ViewBag.TongTien = TongTien();
            return View(carts);
        }

        public ActionResult AddCart(string id)
        {
            List<CartItemModelcs> carts = GetCarts();
            //lay thong tin sp
            CartItemModelcs c = carts.Find(s => s.MaSP == id);
            //SanPham sp = da.SanPhams.FirstOrDefault(s => s.MaSP == id);
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("Login", "Access");
            }
            else
            {

                if (c == null)
                {
                    c = new CartItemModelcs(id);
                    carts.Add(c);
                }
                else
                {
                    c.SoLuong++;
                }

                Session["CartModel"] = carts;

                //Thiet lap thuoc tinh
                HttpContext.Session["Cart"] = carts;
                return RedirectToAction("ListCarts", "Cart");
            }
            //List<CartItemModelcs> carts = GetCarts();
            ////lay thong tin sp
            //CartItemModelcs c = carts.Find(s => s.MaSP == id);
            ////SanPham sp = da.SanPhams.FirstOrDefault(s => s.MaSP == id);

            //if (c == null)
            //{
            //    c = new CartItemModelcs(id);
            //    carts.Add(c);
            //}
            //else
            //    c.SoLuong++;
            //Session["CartModel"] = carts;


            ////Thiet lap thuoc tinh
            //HttpContext.Session["Cart"] = carts;
            //return RedirectToAction("ListCarts", "Cart");
        }
        private int TongSL()
        {
            int SL = 0;
            List<ChiTietDonHang> carts = Session["dh"] as List<ChiTietDonHang>;
            if (carts != null)
            {
                SL = (int)carts.Sum(c => c.SoLuong);
            }
            return SL;
        }
        private double TongTien()
        {
            double Tong = 0;
            List<ChiTietDonHang> carts = Session["dh"] as List<ChiTietDonHang>;
            if (carts != null)
            {
                Tong = (double)carts.Sum(c => c.SoLuong * c.DonGia);
            }
            return Tong;
        }
        //[HttpPost]
        //public ActionResult UpdateQuantity(string id, int quantity)
        //{
        //    List<CartItemModelcs> carts = GetCarts();
        //    CartItemModelcs cartItem = carts.FirstOrDefault(c => c.MaSP == id);
        //    if (cartItem != null)
        //    {
        //        cartItem.SoLuong = quantity;
        //    }

        //    // Update the session
        //    Session["CartModel"] = carts;

        //    // Recalculate totals
        //    int totalQuantity = carts.Sum(c => c.SoLuong);
        //    double totalPrice = carts.Sum(c => c.Total);

        //    return Json(new { totalQuantity, totalPrice });
        //}


        public ActionResult Index()
        {

            return View();
        }
        public RedirectToRouteResult SuaSoLuong(string id, int soluongmoi)
        {
            // tìm carditem muon sua
            List<CartItemModelcs> giohang = Session["CartModel"] as List<CartItemModelcs>;
            CartItemModelcs itemSua = giohang.FirstOrDefault(m => m.MaSP == id);
            if (itemSua != null)
            {
                itemSua.SoLuong = soluongmoi;
            }
            return RedirectToAction("ListCarts");

        }
        public ActionResult XoaGioHang(string id)
        {
            List<CartItemModelcs> carts = GetCarts();
            //lay thong tin sp
            CartItemModelcs c = carts.Single(s => s.MaSP == id);
            if (c != null)
            {
                carts.RemoveAll(n => n.MaSP == id);
                return RedirectToAction("ListCarts");
            }
            if (carts.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //Thiet lap thuoc tinh


            return RedirectToAction("ListCarts");
        }

        public ActionResult DatHang()
        {
            //if (Session["User"] == null || Session["User"].ToString() == "")
            //{
            //    return RedirectToAction("Login", "Access");
            //}
            if (Session["CartModel"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //lay gio hang tu session
            List<CartItemModelcs> carts = GetCarts();
            Session["CartModel"] = carts;
            ViewBag.TongSoLuong = TongSL();
            ViewBag.TongTien = TongTien();
            return View(carts);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            List<CartItemModelcs> carts = GetCarts();
            bool canPlaceOrder = true;

            foreach (var item in carts)
            {
                var product = da.SanPham.SingleOrDefault(p => p.MaSP == item.MaSP);
                if (product == null || product.SoLuong < item.SoLuong)
                {
                    canPlaceOrder = false;
                    break;
                }
            }

            if (!canPlaceOrder)
            {
                ViewBag.ErrorMessage = "Số lượng sản phẩm trong giỏ hàng vượt quá số lượng có sẵn.";
                return View("DatHang", carts);
            }

            using (TransactionScope tranScope = new TransactionScope())
            {
                try
                {
                    DonHang o = new DonHang();
                    UsSer c = (UsSer)Session["User"];
                    o.userID = c.userID;
                    o.NgayDat = DateTime.Now;

                    da.DonHang.Add(o);
                    da.SaveChanges();

                    foreach (var item in carts)
                    {
                        ChiTietDonHang ctdh = new ChiTietDonHang();
                        ctdh.MaDH = o.MaDH;
                        ctdh.MaSP = item.MaSP;
                        ctdh.DonGia = item.Gia;
                        ctdh.SoLuong = (short)item.SoLuong;
                        da.ChiTietDonHang.Add(ctdh);

                        var product = da.SanPham.SingleOrDefault(p => p.MaSP == item.MaSP);
                        if (product != null)
                        {
                            product.SoLuong -= item.SoLuong;
                        }
                    }
                    da.SaveChanges();
                    tranScope.Complete();
                    Session["CartModel"] = null;
                    ViewBag.OrderID = o.MaDH;
                }
                catch (Exception)
                {
                    tranScope.Dispose();
                    return RedirectToAction("ListCarts", "Cart");
                }
            }

            return RedirectToAction("XacNhanDonHang", "Cart");
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }
        //private int TongSL()
        //{
        //    int SL = 0;
        //    List<CartModel> carts = Session["CartModel"] as List<CartModel>;
        //    if (carts != null)
        //    {
        //        SL = carts.Sum(c => c.Quantity);
        //    }
        //    return SL;
        //}
        //private double TongTien()
        //{
        //    double Tong = 0;
        //    List<CartModel> carts = Session["CartModel"] as List<CartModel>;
        //    if (carts != null)
        //    {
        //        Tong = (double)carts.Sum(c => c.Total);
        //    }
        //    return Tong;
        //}

        public ActionResult XemDonHang()
        {
            if (Session["User"] == null)
                return RedirectToAction("Index", "Home");
            UsSer k = (UsSer)Session["User"];
            List<DonHang> dh = da.DonHang.Where(s => s.userID == k.userID).ToList();

            if (dh.Count > 0)
                ViewBag.SoLuongDH = "";
            else
                ViewBag.SoLuongDH = dh.Count;
            return View(dh);
        }
        public ActionResult XemCTDonHang(int id)
        {
            if (Session["User"] == null)
                return RedirectToAction("Index", "Home");

            List<ChiTietDonHang> dh = da.ChiTietDonHang.Where(s => s.MaDH == id).ToList();
            Session["dh"] = dh;

            ViewBag.TongSL = TongSL();
            ViewBag.TongTien = TongTien();
            return View(dh);
        }


    }
}
   
