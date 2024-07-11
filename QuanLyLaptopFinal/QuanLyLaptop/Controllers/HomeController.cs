using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyLaptop.Models;
using PagedList;
namespace QuanLyLaptop.Controllers
{
    public class HomeController : Controller
    {
        QLLaptopEntities1 da = new QLLaptopEntities1();
        public ActionResult Index(int? page, string searchString)
        {
            
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            IEnumerable<SanPham> ds;

            if (!String.IsNullOrEmpty(searchString))
            {
                ds = da.SanPham
                      .Where(s => s.TenSP.Contains(searchString) || s.PhanLoaiHang.TenHang.Contains(searchString))
                      .OrderByDescending(s => s.MaSP)
                      .ToList();
            }
            else
            {
                ds = da.SanPham.OrderBy(x => x.MaSP).ToList();
            }

            return View(ds.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult PhanLoaiHang()
        {
            var loaihang = from cd in da.PhanLoaiHang select cd;
            return PartialView(loaihang);
        }
        public ActionResult SPTheoHang(int mahang)
        {
            List<SanPham> lstsanpham = da.SanPham.Where(x => x.MaHang == mahang).OrderBy(x => x.TenSP).ToList();
            return View(lstsanpham);
        }
        public ActionResult ChiTietSanPham(string maSP)
        {
            var sanPham = da.SanPham.FirstOrDefault(x => x.MaSP == maSP);
            var anhSanPham = da.SanPham.Select(x => x.AnhSP).ToList();
            ViewBag.sanPham = sanPham;
            ViewBag.anhSanPham = anhSanPham;
            return View(sanPham); // Trả về một đối tượng SanPham thay vì danh sách
        }



    }
}
