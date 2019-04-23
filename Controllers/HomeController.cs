using ShopDTDD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASPSnippets.GoogleAPI;
using System.Web.Script.Serialization;
using ShopDTDD.Class;

namespace ShopDTDD.Controllers
{
    public class HomeController : Controller
    {

        dbQLBHDataContext db = new dbQLBHDataContext();
        

        [ActionName("LoginWithGooglePlus")]
        public ActionResult LoginWithGooglePlusConfirmed()
        {
            dbQLBHDataContext db = new dbQLBHDataContext();
            string code = Request.QueryString["code"];
            string json = GoogleConnect.Fetch("me", code);
            GoogleProfile profile = new JavaScriptSerializer().Deserialize<GoogleProfile>(json);
            try
            {
                tb_TaiKhoan kh = new tb_TaiKhoan();
                kh.TenTK = profile.Emails.Find(email => email.Type == "account").Value;
                kh.Ten = profile.DisplayName;
                UpdateModel(kh);
                db.tb_TaiKhoans.InsertOnSubmit(kh);
                db.SubmitChanges();

            }
            catch (Exception) { }

            tb_TaiKhoan kh2 = db.tb_TaiKhoans.SingleOrDefault(n => n.TenTK == profile.Emails.Find(email => email.Type == "account").Value);
            Session["TaiKhoanKH"] = kh2;
            Session["UserKH"] = kh2.Ten;
            return RedirectToAction("Index", "TrangChu");
        }

        [HttpGet]
        public ActionResult GoogleDangNhap()
        {
            ViewBag.UserName = Convert.ToString(Session["UserName"]);

            return View();
        }
        [HttpPost]

        public JsonResult Login(string name, string email)

        {

            Session["UserName"] = email;

            return Json(new { success = "True" });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void LoginWithGooglePlus()
        {
            GoogleConnect.ClientId = "391859287769-hv4jb1e0q05ae7ggd56pe81gnloaa6uf.apps.googleusercontent.com";
            GoogleConnect.ClientSecret = "W2QwP2oGcM3K5m31SxfXmE3S";
            GoogleConnect.RedirectUri = Request.Url.AbsoluteUri.Split('?')[0];
            GoogleConnect.Authorize("profile", "email");
        }
        [HttpGet]
        public ActionResult FacebookDangNhap()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }


        [HttpPost]
        public ActionResult DangKy(FormCollection collection, tb_TaiKhoan kh)
        {
            
            var fName = collection["FirstName"];
            var lName = collection["LastName"];
            var Email = collection["Email"];
            var Pass = collection["Pass"];
            var RePass = collection["RePass"];

            if (String.IsNullOrEmpty(Email))
            {
                ViewData["LoiEmail"] = "Email Khong Duoc Trong";
            }
            else if (String.IsNullOrEmpty(Pass))
            {
                ViewData["LoiPass"] = "Pass Khong Duoc Trong";
            }
            else if (!Pass.Equals(RePass))
            {
                ViewData["LoiRePass"] = "Repeat Password Khong Giong Password";
            }
            else
            {
                try
                {
                    kh.TenTK = Email;
                    kh.MatKhau = Pass;
                    kh.Ho = fName;
                    kh.Ten = lName;
                    UpdateModel(kh);
                    db.tb_TaiKhoans.InsertOnSubmit(kh);
                    db.SubmitChanges();
                } catch (Exception) { ViewData["LoiEmail2"] = "Email Da Ton Tai"; }
                return RedirectToAction("DangNhap");
                
            }


            return this.DangKy();
        }

        public ActionResult DangXuat()
        {
            Session["TaiKhoanKH"] = null;
            Session["UserKH"] = null;
            Session["GioHang"] = null;
            Session["SoHang"] = null;
            return RedirectToAction("Index", "TrangChu");
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {


            var Email = collection["Email"];
            var Pass = collection["Pass"];
            if (String.IsNullOrEmpty(Email))
            {
                ViewData["Loi1"] = "Phải Nhập Tên Đăng Nhập";
            }
            else
            {
                if (String.IsNullOrEmpty(Pass))
                {
                    ViewData["Loi2"] = "Phải Nhập Mật Khẩu";
                }
                else
                {
                    tb_TaiKhoan kh = db.tb_TaiKhoans.SingleOrDefault(n=> n.TenTK==Email && n.MatKhau==Pass);
                    if (kh != null)
                    {
                        ViewBag.ThongBao = "Đăng Nhập Thành Công";
                        Session["TaiKhoanKH"] = kh;
                        Session["UserKH"] = kh.Ten;
                        return RedirectToAction("Index", "TrangChu");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên Đăng Nhập Hoặc MK không đúng";
                    }
                }
            }
            return this.DangNhap();
                

        }

    }
}