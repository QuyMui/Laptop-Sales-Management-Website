using Microsoft.AspNetCore.Mvc;
using PagedList;
using QuanLyLaptop.Models;
using QuanLyLaptop.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace QuanLyLaptop.Areas.Admin.Controllers
{
    [Area("admin")]
    [System.Web.Mvc.Route("admin")]
    [System.Web.Mvc.Route("admin/homeadmin")]
    
    public class HomeAdminController : System.Web.Mvc.Controller
    {
        QLLaptopEntities1 da = new QLLaptopEntities1();
        [System.Web.Mvc.Route("")]
        [System.Web.Mvc.Route("index")]
        // GET: Admin/Home
        // Admin

        [Authentication]
        public System.Web.Mvc.ActionResult Index()
        {
            return View();
         }
        [Authentication]
        public System.Web.Mvc.ActionResult DanhMucSanPham(int? page)
        {


            int pageSize = 8;
            int pageNumber = (page ?? 1);

            var listSP = da.SanPham.OrderBy(x => x.MaSP).ToList();

            // Sử dụng thư viện PagedList để phân trang danh sách sản phẩm
            PagedList<SanPham> lst = new PagedList<SanPham>(listSP, pageNumber, pageSize);

            // Truyền danh sách sản phẩm vào view
            return View(lst);

            ////var listSP = da.SanPhams.OrderBy(x => x.MaSP).Take(19).ToList();
            //PagedList<SanPham> lst = new PagedList<SanPham>(listSP, pageNumber, pageSize);
            //return View().T(pageNumber, pageSize));
        }
        [System.Web.Mvc.HttpGet]
        public System.Web.Mvc.ActionResult ThemSanPhamMoi()
        {
            ViewBag.MaNhaCungCap = new SelectList(da.NhaCungCap.ToList(), "MaNhaCungCap", "TenNhaCungCap");
            ViewBag.MaHang = new SelectList(da.PhanLoaiHang.ToList(), "MaHang", "TenHang");
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public System.Web.Mvc.ActionResult ThemSanPhamMoi(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                da.SanPham.Add(sanPham);
                da.SaveChanges();

                var updatedListSP = da.SanPham.OrderBy(x => x.MaSP).ToList();
                return RedirectToAction("DanhMucSanPham", updatedListSP);
            }
            return View(sanPham);
        }
        [System.Web.Mvc.HttpGet]
        public System.Web.Mvc.ActionResult SuaSanPham(string MaSP)
        {
            var sanPham = da.SanPham.Find(MaSP);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNhaCungCap = new SelectList(da.NhaCungCap.ToList(), "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaHang = new SelectList(da.PhanLoaiHang.ToList(), "MaHang", "TenHang", sanPham.MaHang);
            return View(sanPham);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public System.Web.Mvc.ActionResult SuaSanPham(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                da.Entry(sanPham).State = System.Data.Entity.EntityState.Modified;
                da.SaveChanges();
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }
            ViewBag.MaNhaCungCap = new SelectList(da.NhaCungCap.ToList(), "MaNhaCungCap", "TenNhaCungCap", sanPham.MaNhaCungCap);
            ViewBag.MaHang = new SelectList(da.PhanLoaiHang.ToList(), "MaHang", "TenHang", sanPham.MaHang);
            return View(sanPham);
        }

       
        [System.Web.Mvc.HttpGet]
        public System.Web.Mvc.ActionResult XoaSanPham(string MaSP)
        {
            var sanPham = da.SanPham.FirstOrDefault(x => x.MaSP == MaSP);
            var chiTietDonHang = da.ChiTietDonHang.Where(x => x.MaSP == MaSP).ToList();
            if (chiTietDonHang.Count > 0)
            {
                TempData["Message"] = "Khong xóa được sản phẩm này";
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }
            else if (sanPham == null)
            {
                TempData["Message"] = "Không tìm thấy sản phẩm để xóa";
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }
            da.SanPham.Remove(sanPham);
            da.SaveChanges();
            TempData["Message"] = "Sản phẩm đã được xóa";
            return RedirectToAction("DanhMucSanPham", "HomeAdmin");
        }

        [Authentication]
        public System.Web.Mvc.ActionResult DanhMucDonHang(int? page)
        {
            int pageSize = 10; // Số lượng đơn hàng trên mỗi trang
            int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là trang 1

            var listDonHang = da.DonHang.OrderBy(x => x.MaDH).ToList(); // Lấy danh sách đơn hàng và sắp xếp theo MaDH

            // Sử dụng PagedList để phân trang
            PagedList<DonHang> lst = new PagedList<DonHang>(listDonHang, pageNumber, pageSize);

            // Truyền danh sách đơn hàng đã phân trang vào view
            return View(lst);
        }

        [System.Web.Mvc.HttpGet]
        public System.Web.Mvc.ActionResult ChiTietDonHang(int MaDH)
        {
            var donHang = da.DonHang.Find(MaDH);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        [System.Web.Mvc.HttpGet]
        public System.Web.Mvc.ActionResult SuaDonHang(int MaDH)
        {
            var donHang = da.DonHang.Find(MaDH);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public System.Web.Mvc.ActionResult SuaDonHang(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                var existingDonHang = da.DonHang.Find(donHang.MaDH);

                if (existingDonHang != null)
                {
                    existingDonHang.NgayDat = donHang.NgayDat;
                    existingDonHang.NgayGiao = donHang.NgayGiao;
                    existingDonHang.userID = donHang.userID;
                    existingDonHang.TinhTrangGiaoHang = donHang.TinhTrangGiaoHang;
                    existingDonHang.DaThanhToan = donHang.DaThanhToan;

                    da.Entry(existingDonHang).State = System.Data.Entity.EntityState.Modified;
                    da.SaveChanges();
                    return RedirectToAction("DanhMucDonHang", "HomeAdmin");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return View(donHang);
        }

        [System.Web.Mvc.HttpGet]
        public System.Web.Mvc.ActionResult ThemDonHangMoi()
        {
            // Nếu bạn cần lấy danh sách từ bảng khác để hiển thị trong View, bạn có thể thêm vào ViewBag như sau:
            // ViewBag.MaNhaCungCap = new SelectList(da.NhaCungCap.ToList(), "MaNhaCungCap", "TenNhaCungCap");
            // ViewBag.MaHang = new SelectList(da.PhanLoaiHang.ToList(), "MaHang", "TenHang");
            return View();
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public System.Web.Mvc.ActionResult ThemDonHangMoi(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                da.DonHang.Add(donHang);
                da.SaveChanges();

                return RedirectToAction("DanhMucDonHang");
            }
            return View(donHang);
        }

        //[System.Web.Mvc.HttpGet]
        //public System.Web.Mvc.ActionResult XoaDonHang(string MaDH)
        //{
        //    var donHang = da.DonHang.Find(MaDH);
        //    if (donHang == null)
        //    {
        //        TempData["Message"] = "Không tìm thấy đơn hàng để xóa";
        //        return RedirectToAction("QuanLyDonHang");
        //    }
        //    da.DonHang.Remove(donHang);
        //    da.SaveChanges();
        //    TempData["Message"] = "Đơn hàng đã được xóa";
        //    return RedirectToAction("QuanLyDonHang");
        //}
        [System.Web.Mvc.HttpGet]
        public System.Web.Mvc.ActionResult XoaDonHang(int MaDH)
        {
            var donHang = da.DonHang.Find(MaDH);
            var chiTietDonHang = da.ChiTietDonHang.Where(x => x.MaDH == MaDH).ToList();

            if (chiTietDonHang.Count > 0)
            {
                TempData["Message"] = "Không thể xóa đơn hàng này vì có chi tiết đơn hàng liên quan.";
                return RedirectToAction("DanhMucDonHang", "HomeAdmin");
            }
            else if (donHang == null)
            {
                TempData["Message"] = "Không tìm thấy đơn hàng để xóa.";
                return RedirectToAction("DanhMucDonHang", "HomeAdmin");
            }

            return View(donHang);
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public System.Web.Mvc.ActionResult XoaDonHangConfirmed(int MaDH)
        {
            var donHang = da.DonHang.Find(MaDH);
            var chiTietDonHang = da.ChiTietDonHang.Where(x => x.MaDH == MaDH).ToList();

            if (chiTietDonHang.Count > 0)
            {
                TempData["Message"] = "Không thể xóa đơn hàng này vì có chi tiết đơn hàng liên quan.";
                return RedirectToAction("DanhMucDonHang", "HomeAdmin");
            }
            else if (donHang == null)
            {
                TempData["Message"] = "Không tìm thấy đơn hàng để xóa.";
                return RedirectToAction("DanhMucDonHang", "HomeAdmin");
            }

            da.DonHang.Remove(donHang);
            da.SaveChanges();

            TempData["Message"] = "Đơn hàng đã được xóa thành công.";
            return RedirectToAction("DanhMucDonHang", "HomeAdmin");
        }

    }
}

