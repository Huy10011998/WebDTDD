using ShopDTDD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;
using System.Data.Linq;
using System.Web.Security;

namespace ShopDTDD.Controllers
{
    public class AdminController : Controller
    {
        dbQLBHDataContext db = new dbQLBHDataContext();
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            //WebSecurity.Logout();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Admin");
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {

            var TenTK = collection["TenTK"];
            var MatKhau = collection["MatKhau"];
            if (String.IsNullOrEmpty(TenTK))
            {
                ViewData["Loi1"] = "Phải Nhập Tên Đăng Nhập";
                return this.Login();
            }
            else
            {
                if (String.IsNullOrEmpty(MatKhau))
                {
                    ViewData["Loi2"] = "Phải Nhập Mật Khẩu";
                    return this.Login();
                }
                else
                {
                    Admin kh = db.Admins.SingleOrDefault(n => n.TenTK == TenTK && n.MatKhau == MatKhau);
                    if (kh != null)
                    {
                        FormsAuthentication.SetAuthCookie(TenTK, true);
                        ViewBag.ThongBao = "Đăng Nhập Thành Công";
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên Đăng Nhập Hoặc MK không đúng";
                        return this.Login();
                    }
                }
            }
        }
        [Authorize]
        public ActionResult MatHang(string timkiem, int? page)
        {
            ViewBag.TuKhoa = timkiem;
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            if (timkiem != null)
            {
                List<tb_HangHoa> listKQ = db.tb_HangHoas.Where(n => n.TenHH.Contains(timkiem)).ToList();
                if (listKQ.Count == 0)
                {
                    TempData["tb"] = "Không tìm thấy sản phẩm nào phù hợp.";
                    return View(db.tb_HangHoas.OrderBy(n => n.TenHH).ToPagedList(pageNumber, pageSize));
                }
                return View(listKQ.OrderBy(n => n.MaLoai).ToPagedList(pageNumber, pageSize));
            }
            return View(db.tb_HangHoas.ToList().OrderBy(n => n.MaLoai).ToPagedList(pageNumber, pageSize));
        }
        [Authorize]
        [HttpGet]
        public ActionResult ThemHinh(int MaHH)
        {
            tb_HangHoa hh = db.tb_HangHoas.SingleOrDefault(n => n.MaHH == MaHH);
            ViewBag.MaHH = hh.MaHH;
            return View(hh);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemHinh(tb_HangHoa hh, HttpPostedFileBase fileUpload)
        {
            int MAHH = hh.MaHH;
            tb_HinhAnh HinhAnh = new tb_HinhAnh();
            
            if (fileUpload != null)
            {
                if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    HinhAnh.HinhAnh = fileName;
                    HinhAnh.MaHH = MAHH;
                    //Luu vao CSDL
                    try {
                        db.tb_HinhAnhs.InsertOnSubmit(HinhAnh);
                        db.SubmitChanges();
                    }
                    catch (Exception) { }
                    
                }
                return RedirectToAction("MatHang");
            }
            return RedirectToAction("MatHang");
        }
        [Authorize]
        [HttpGet]
        public ActionResult ThemMoiMH()
        {

            //Dua du lieu vao dropdownList
            //Lay ds tu tabke chu de, sắp xep tang dan trheo ten chu de, chon lay gia tri Ma CD, hien thi thi Tenchude
            ViewBag.MaLoai = new SelectList(db.tb_Menus.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMoiMH(tb_HangHoa HangHoa, HttpPostedFileBase fileUpload)
        {
            //Dua du lieu vao dropdownload
            ViewBag.MaLoai = new SelectList(db.tb_Menus.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            //Kiem tra duong dan file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    HangHoa.HinhAnh = fileName;
                    //Luu vao CSDL
                    db.tb_HangHoas.InsertOnSubmit(HangHoa);
                    db.SubmitChanges();
                }
                return RedirectToAction("MatHang");
            }   
        }
        public ActionResult SPTheodanhmucAD(string id, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            var tb_HangHoa = from s in db.tb_HangHoas where s.MaLoai == id select s;
            return View(tb_HangHoa.ToList().OrderBy(n => n.MaLoai).ToPagedList(pageNumber, pageSize));

        }

        public ActionResult DM()
        {
            var danhmuc = from ten in db.tb_Menus select ten;
            return PartialView(danhmuc);
        }
        public ActionResult DetailsSP (int id)
        {
            //Lay ra doi tuong sach theo ma
            tb_HangHoa hh = db.tb_HangHoas.SingleOrDefault(n => n.MaHH == id);
            ViewBag.MaHH = hh.MaHH;
            if (hh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hh);
        }
        [Authorize]
        [HttpGet]
        public ActionResult XoaSP(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            tb_HangHoa hh = db.tb_HangHoas.SingleOrDefault(n => n.MaHH == id);
            ViewBag.MaHH = hh.MaHH;
            if (hh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hh);
        }

        [HttpPost, ActionName("XoaSP")]
        public ActionResult XacNhanXoaSP(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            tb_HangHoa hh = db.tb_HangHoas.SingleOrDefault(n => n.MaHH == id);
            ViewBag.MaHH = hh.MaHH;
            if (hh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.tb_HangHoas.DeleteOnSubmit(hh);
            db.SubmitChanges();
            return RedirectToAction("MatHang");
        }

        [Authorize]
        [HttpGet]
        public ActionResult SuaSP(int id)
        {
            //Lay ra doi tuong sach theo ma
            tb_HangHoa hh = db.tb_HangHoas.SingleOrDefault(n => n.MaHH == id);
            ViewBag.MaHH = hh.MaHH;
            if (hh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Dua du lieu vao dropdownList
            //Lay ds tu tabke chu de, sắp xep tang dan trheo ten chu de, chon lay gia tri Ma CD, hien thi thi Tenchude
            ViewBag.MaLoai = new SelectList(db.tb_Menus.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai", hh.MaHH);
            return View(hh);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSP(tb_HangHoa hh, HttpPostedFileBase fileUpload)
        {
            int a = hh.MaHH;
            //Dua du lieu vao dropdownload
            ViewBag.MaLoai = new SelectList(db.tb_Menus.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai", hh.MaHH);
            //Kiem tra duong dan file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View(hh);
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;


                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    tb_HangHoa hh2 = db.tb_HangHoas.Where(n => n.MaHH == hh.MaHH).SingleOrDefault();
                    string img= fileName;
                    hh2.MaLoai= Request.Form["MaLoai"];
                    hh2.TenHH = Request.Form["TenHH"];
                    hh2.SoLuong = int.Parse(Request.Form["SoLuong"]);
                    hh2.HinhAnh = img;
                    hh2.GiaBan = int.Parse(Request.Form["GiaBan"]);
                    hh2.MoTaSP= Request.Form["MoTaSP"];
                   
                    //Luu vao CSDL   
                    UpdateModel(hh2);
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (ChangeConflictException e)
                    {
                        Console.WriteLine(e.Message);
                        // Make some adjustments.
                        // ...
                        // Try again.
                        db.SubmitChanges();
                    }

                }
                return RedirectToAction("MatHang");
            }
        }
        [Authorize]
        public ActionResult TaiKhoanAdmin(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            //return View(db.tb_HangHoas.ToList());
            return View(db.Admins.ToList().OrderBy(n => n.TenTK).ToPagedList(pageNumber, pageSize));
        }
        [Authorize]
        public ActionResult TaiKhoanKH(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            //return View(db.tb_HangHoas.ToList());
            return View(db.tb_TaiKhoans.ToList().OrderBy(n => n.TenTK).ToPagedList(pageNumber, pageSize));
        }
        [Authorize]
        [HttpGet]
        public ActionResult ThemTKAdmin()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ThemTKAdmin(FormCollection collection, Admin ad)
        {


            var TenTK = collection["TenTK"];
            var Pass = collection["Pass"];
            var RePass = collection["RePass"];

            if (String.IsNullOrEmpty(TenTK))
            {
                ViewData["LoiTenTK"] = "TenTK Khong Duoc Trong";
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
                    ad.TenTK = TenTK;
                    ad.MatKhau = Pass;

                    UpdateModel(ad);
                    db.Admins.InsertOnSubmit(ad);
                    db.SubmitChanges();
                }
                catch (Exception) { ViewData["LoiTenTK2"] = "Email Da TOn Tai"; }
                return RedirectToAction("TaiKhoanAdmin");

            }


            return this.ThemTKAdmin();
        }
        [Authorize]
        public ActionResult DetailsKH(int id)
        {
            //Lay ra doi tuong sach theo ma
            tb_TaiKhoan kh = db.tb_TaiKhoans.SingleOrDefault(n => n.MaTK == id);
            ViewBag.MaHH = kh.MaTK;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        [Authorize]
        [HttpGet]
        public ActionResult XoaKH(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            tb_TaiKhoan kh = db.tb_TaiKhoans.SingleOrDefault(n => n.MaTK == id);
            ViewBag.MaTK = kh.MaTK;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }

        [HttpPost, ActionName("XoaKH")]
        public ActionResult XacNhanXoaKH(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            tb_TaiKhoan kh = db.tb_TaiKhoans.SingleOrDefault(n => n.MaTK == id);
            ViewBag.MaTK = kh.MaTK;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.tb_TaiKhoans.DeleteOnSubmit(kh);
            db.SubmitChanges();
            return RedirectToAction("TaiKhoanKH");
        }
        [Authorize]
        public ActionResult ThongKeSP(DateTime? NgayA, DateTime? NgayB)
        {
            //Tính tổng doanh thu
            TempData["TongKhachHang"] = db.tb_TaiKhoans.Count();
            TempData["DonHangChuaGiao"] = db.tb_HoaDons.Where(n => n.TinhTrangGiaoHang == false && n.DuyetDH == "Đã Duyệt").Count() ;
            TempData["DonHangChuaDuyet"]= db.tb_HoaDons.Where(n => n.TinhTrangGiaoHang == false && n.DuyetDH == "").Count();
            TempData["TongDoanhThu"] = db.tb_HoaDons.Where(n => Convert.ToString(n.DuyetDH) == "Đã Duyệt" && n.NgayGiao.ToString() != "True").Sum(n => n.Tong);
            return View(db.tb_HoaDons.Where(n => n.NgayDat >= NgayA && n.NgayDat < NgayB && Convert.ToString(n.TinhTrangGiaoHang) == "True").ToList());
        }

        [Authorize]
        public Object TKDoanhThuTheoNgay(DateTime? NgayA, DateTime? NgayB)
        {
            TempData["NgayA"] = NgayA;
            TempData["NgayB"] = NgayB;
            if (NgayA == null || NgayB == null)
            {
                TempData["DoanhThuTheoNgay"] = "0";
                return RedirectToAction("ThongKeSP", "Admin");
            }
            TempData["DoanhThuTheoNgay"] = db.tb_HoaDons.Where(n => n.NgayGiao > NgayA && n.NgayGiao <= NgayB && Convert.ToString(n.TinhTrangGiaoHang) == "True" && Convert.ToString(n.DaThanhToan) == "True").Sum(n => n.Tong);
            return RedirectToAction("ThongKeSP", "Admin");
        }
        [Authorize]
        public ActionResult DanhSachHoaDon(string timkiem, int? page)
        {
            ViewBag.TuKhoa = timkiem;
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            if (timkiem != null)
            {
                List<tb_HoaDon> listKQ = db.tb_HoaDons.Where(n => n.TenTK.Contains(timkiem)).ToList();
                if (listKQ.Count == 0)
                {
                    TempData["tb"] = "Không tìm thấy sản phẩm nào phù hợp.";
                    return View(db.tb_HoaDons.OrderBy(n => n.TenTK).ToPagedList(pageNumber, pageSize));
                }
                return View(listKQ.OrderBy(n => n.TenTK).ToPagedList(pageNumber, pageSize));
            }
            return View(db.tb_HoaDons.ToList().OrderBy(n => n.TenTK).ToPagedList(pageNumber, pageSize));
        }
        [Authorize]
        public ActionResult DetailsHD(int id)
        {
            //Lay ra doi tuong sach theo ma
            tb_HoaDon kh = db.tb_HoaDons.SingleOrDefault(n => n.MaHD == id);
            ViewBag.MaHD = kh.MaHD;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }

        [Authorize]
        public ActionResult MatHangHD(int id, int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            var tb_HangHoa = from s in db.tb_CTHDs where s.MaHD == id select s;
            return View(tb_HangHoa.ToList().OrderBy(n => n.MaLoai).ToPagedList(pageNumber, pageSize));

        }
        [Authorize]
        public ActionResult DanhSachChuaDuyet(string timkiem, int? page)
        {/*
            if (Session["Admin"] == null || Session["TaiKhoanAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Authh");
            }*/
            ViewBag.TuKhoa = timkiem;
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            if (timkiem != null)
            {
                List<tb_HoaDon> listKQ = db.tb_HoaDons.Where(n => n.MaHD.ToString().Contains(timkiem) || n.tb_TaiKhoan.Ten.ToString().Contains(timkiem)).ToList();
                if (listKQ.Count == 0)
                {
                    TempData["thongbao"] = "Không tìm thấy đơn hàng nào phù hợp.";
                    return View(db.tb_HoaDons.Where(n => n.NgayGiao.ToString() != null && n.DuyetDH == "").OrderByDescending(n => n.MaHD).ToPagedList(pageNumber, pageSize));
                }
                return View(listKQ.Where(n => n.NgayGiao.ToString() != null && n.DuyetDH =="").OrderByDescending(n => n.MaHD).ToPagedList(pageNumber, pageSize));
            }
            return View(db.tb_HoaDons.Where(n => n.NgayGiao.ToString() != null && n.DuyetDH =="").OrderByDescending(n => n.MaHD).ToPagedList(pageNumber, pageSize));
        }
        [Authorize]
        public ActionResult DuyetDonHang(int madh)
        {

            tb_HoaDon dh = db.tb_HoaDons.Where(n => n.MaHD == madh).SingleOrDefault();
            dh.DuyetDH = "Đã duyệt";
            db.SubmitChanges();
            return RedirectToAction("DanhSachChuaDuyet", "Admin");

        }
        [Authorize]
        public ActionResult DanhSachChuaGiao(string timkiem, int? page)
        {/*
            if (Session["Admin"] == null || Session["TaiKhoanAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Authh");
            }*/
            ViewBag.TuKhoa = timkiem;
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            if (timkiem != null)
            {
                List<tb_HoaDon> listKQ = db.tb_HoaDons.Where(n => n.MaHD.ToString().Contains(timkiem) || n.tb_TaiKhoan.Ten.ToString().Contains(timkiem)).ToList();
                if (listKQ.Count == 0)
                {
                    TempData["thongbao"] = "Không tìm thấy đơn hàng nào phù hợp.";
                    return View(db.tb_HoaDons.Where(n => n.NgayGiao.ToString() != null && n.TinhTrangGiaoHang == false && n.DuyetDH == "Đã Duyệt").OrderByDescending(n => n.MaHD).ToPagedList(pageNumber, pageSize));
                }
                return View(listKQ.Where(n => n.NgayGiao.ToString() != null && n.TinhTrangGiaoHang == false && n.DuyetDH == "Đã Duyệt").OrderByDescending(n => n.MaHD).ToPagedList(pageNumber, pageSize));
            }
            return View(db.tb_HoaDons.Where(n => n.NgayGiao.ToString() != null && n.TinhTrangGiaoHang == false && n.DuyetDH== "Đã Duyệt").OrderByDescending(n => n.MaHD).ToPagedList(pageNumber, pageSize));
        }
        [Authorize]
        public ActionResult DuyetGiaoHang(int madh)
        {

            tb_HoaDon dh = db.tb_HoaDons.Where(n => n.MaHD == madh).SingleOrDefault();
            dh.TinhTrangGiaoHang = true;
            dh.DaThanhToan = true;
            db.SubmitChanges();
            return RedirectToAction("DanhSachChuaGiao", "Admin");
        }
        [Authorize]
        public ActionResult XoaHoaDon(int madh)
        {
            
            List<tb_CTHD> cthd = db.tb_CTHDs.Where(n => n.MaHD == madh).ToList();
            tb_HoaDon dh = db.tb_HoaDons.Where(n => n.MaHD == madh).SingleOrDefault();
            
            foreach (var item in cthd)
            {
                db.tb_CTHDs.DeleteOnSubmit(item);
                db.SubmitChanges();
            }
            db.tb_HoaDons.DeleteOnSubmit(dh);
            db.SubmitChanges();
            return RedirectToAction("DanhSachHoaDon", "Admin");
        }
    }
}