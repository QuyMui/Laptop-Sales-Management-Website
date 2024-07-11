using QuanLyLaptop.Models;
using QuanLyLaptop.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QuanLyLaptop.Areas.Admin.Controllers
{
    public class AccessAdminController : Controller
    {
        QLLaptopEntities1 da = new QLLaptopEntities1();
        [HttpGet]
        // GET: Admin/AccessAdmin
        public ActionResult Login()
        {
            if (Session["User"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "HomeAdmin");
            }
        }
        
        [HttpPost]
        public ActionResult Login(UsSer user)
        {
            if (Session["UserName"] == null)
            {
                var u = da.UsSer.Where(x => x.username.Equals(user.username) && x.password.Equals(user.password)).FirstOrDefault();
                if (u != null)
                {
                    // Check if the user's role_id is 1
                    if (u.role_id == 1)
                    {
                        Session["UserName"] = u.username.ToString();
                        return RedirectToAction("Index", "HomeAdmin");
                    }
                    else
                    {
                        // Optional: Add a message to inform the user about the role restriction
                        ViewBag.ErrorMessage = "Login failed: You do not have the necessary permissions.";
                    }
                }
                else
                {
                    // Optional: Add a message to inform the user about invalid credentials
                    ViewBag.ErrorMessage = "Invalid username or password.";
                }
            }
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            //HttpContext.Session.Clear();
            //HttpContext.Session.Remove("username");
            return RedirectToAction("Login", "AccessAdmin");
        }
    }
}