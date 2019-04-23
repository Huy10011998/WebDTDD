using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ShopDTDD.Models;

namespace ShopDTDD.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserRepository _userRepository;
        public AccountController()
        {
            _userRepository = new UserRepository();
        }
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (_userRepository.Validate(model.Email, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                return RedirectToLocal(returnUrl);
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpGet]
        public ActionResult DangXuat()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangXuat(FormCollection collection)
        {
            //WebSecurity.Logout();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("ThongKeSP", "Admin");
            }
        }
    }
    public class UserRepository
    {

        public bool Validate(string username, string password)
        {
            dbQLBHDataContext db = new dbQLBHDataContext();
            var user = db.Admins.SingleOrDefault(n => n.TenTK == username);
            Admin ad = db.Admins.SingleOrDefault(n => n.TenTK == username);
            if (user == null)
                return false;
            if (user.MatKhau == password) return true;
            return false;
        }
    }
}