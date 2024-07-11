using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using PagedList;
using QuanLyLaptop.Areas.Admin;
using QuanLyLaptop.Models;

namespace QuanLyLaptop.Controllers
{
    public class AccessController : Controller
    {
        QLLaptopEntities1 da = new QLLaptopEntities1();
        [HttpGet]
        //GET: Access
        private bool IsStrongPassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUppercase = false;
            bool hasLowercase = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUppercase = true;
                else if (char.IsLower(c))
                    hasLowercase = true;
                else if (char.IsDigit(c))
                    hasDigit = true;

                if (hasUppercase && hasLowercase && hasDigit)
                    return true;
            }

            return false;
        }

        public ActionResult Login()
        {
            if (Session["User"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult Login(UsSer user)
        {
            if (Session["User"] == null)
            {
                var u = da.UsSer.FirstOrDefault(x => x.username.Equals(user.username) && x.password.Equals(user.password));
                if (u != null)
                {
                    Session["User"] = u;
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        //[HttpPost]
        //public ActionResult Login(UsSer user)
        //{
        //    if (Session["User"] == null)
        //    {
        //var u = da.UsSer.FirstOrDefault(x => x.username.Equals(user.username) && x.password.Equals(user.password));
        //        if (u != null)
        //        {
        //            Session["User"] = u;
        //            if (u.role_id == 2)
        //            {
        //                return RedirectToAction("Index", "Home");
        //            }
        //            else if (u.role_id == 1)
        //            {
        //                return RedirectToAction("HomeAdmin", "Admin");
        //            }
        //        }
        //    }
        //    // Nếu không xác thực thành công hoặc không có quyền hợp lệ, trả về lại trang đăng nhập
        //    return View();
        //}

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            //HttpContext.Session.Clear();
            //HttpContext.Session.Remove("username");
            return RedirectToAction("Index", "Home");
        }
        //[HttpPost]
        //public ActionResult Login(FormCollection collection)
        //{
        //    var tendn = collection["username"];
        //    var matkhau = collection["password"];
        //    if (string.IsNullOrEmpty(tendn))
        //    {
        //        ViewData["Loi1"] = "Phải nhập tên đăng nhập";
        //    }
        //    else if (string.IsNullOrEmpty(matkhau))
        //    {
        //        ViewData["Loi2"] = "Phải nhập mật khẩu";
        //    }
        //    else
        //    {
        //        UsSer ad = da.UsSers.SingleOrDefault(n => n.username == tendn && n.password == matkhau);
        //        if (ad != null)
        //        {
        //            Session["TaiKhoanadmin"] = ad;
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //            ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng"; 
        //    }
        //    return View();
        //}

        [HttpGet]
        public ActionResult DangKy()
        {
            //if (Session["UserID"] == null)
            //    return RedirectToAction("Index");
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, UsSer kh)
        {
            var TenDN = collection["TenDN"];
            var MK = collection["MK"];
            //var MKLai = collection["MKLai"];
            var DiaChi = collection["DiaChi"];
            var Email = collection["Email"];
            var DienThoaiStr = collection["DienThoai"]; // Lưu trữ số điện thoại dưới dạng chuỗi
            int DienThoai; // Biến để lưu số điện thoại dưới dạng int

            UsSer k = da.UsSer.FirstOrDefault(s => s.username == TenDN);
            UsSer mail = da.UsSer.FirstOrDefault(s => s.email == Email);

            // Lấy role_id tương ứng với loại người dùng (trong trường hợp này là loại 2)
            int role_id;

            if (String.IsNullOrEmpty(TenDN) || k != null)
            {
                ViewBag.TenDNError = "Tên đăng nhập không được để trống hoặc trùng";
            }
            else if (String.IsNullOrEmpty(Email) || mail != null)
            {
                ViewBag.EmailError = "Email không được để trống hoặc trùng";
            }
            else if (String.IsNullOrEmpty(MK) || !IsStrongPassword(MK))
            {
                ViewBag.MKError = "Mật khẩu không được để trống và phải đủ 8 ký tự bao gồm chữ hoa, thường, số";
            }
            //else if (String.IsNullOrEmpty(MKLai) || MKLai != MK)
            //{
            //    ViewBag.MKLaiError = "Mật khẩu nhập lại không được để trống và phải giống mật khẩu";
            //}
            else if (String.IsNullOrEmpty(DiaChi))
            {
                ViewBag.DiaChiError = "Địa chỉ không được để trống";
            }
            else if (String.IsNullOrEmpty(DienThoaiStr) || !int.TryParse(DienThoaiStr, out DienThoai) || DienThoaiStr.Any(c => !char.IsDigit(c)) || DienThoaiStr.Length != 10)
            {
                ModelState.AddModelError("DienThoai", "Phone number must contain only 10 digits.");
            }

            //else if (!int.TryParse(DienThoaiStr, out DienThoai) || DienThoaiStr.Any(c => !char.IsDigit(c)) || DienThoaiStr.Length != 10)
            //{
            //    ModelState.AddModelError("DienThoai", "Phone number must contain only digits and must be 10 digits.");
            //}
            //else if (String.IsNullOrEmpty(DiaChi))
            //{
            //    ViewBag.DiaChiError = "Địa chỉ không được để trống";
            //}
            else
            {
                // Gán thông tin người dùng
                kh.username = TenDN;
                kh.password = MK;
                kh.Sdt = DienThoai; // Gán số điện thoại đã chuyển đổi sang kiểu int
                kh.email = Email;
                kh.DiaChi = DiaChi;

                // Tạo mới User_Role và gán role_id
                kh.role_id = 2;

                // Thêm người dùng mới vào database
                da.UsSer.Add(kh);

                // Lưu thay đổi vào database
                da.SaveChanges();

                // Đăng nhập người dùng mới
                //Session["UserName"] = kh.username;

                // Chuyển hướng về trang chính
                return RedirectToAction("Login", "Access");
            }
            return View(kh);
        }
       

    }


}



