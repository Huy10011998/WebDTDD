using ShopDTDD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopDTDD.Controllers
{
    public class GioHangController : Controller
    {
        dbQLBHDataContext data = new dbQLBHDataContext();
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang =Session["GioHang"] as List<GioHang>;
            if(lstGioHang==null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        // GET: GioHang
        public ActionResult ThemGioHang(int iMaHH, string strURL)
        {
            if(Session["TaiKhoanKH"]==null)
                return RedirectToAction("DangNhap", "Home");
            
            List<GioHang> lstGioHang = LayGioHang();
            //kiem tra sach co ton tai trong session chua?
            GioHang sanpham = lstGioHang.Find(n=>n.iMaHH==iMaHH);
            if(sanpham== null)
            {
                sanpham = new GioHang(iMaHH);
                lstGioHang.Add(sanpham);
                Session["SoHang"] = TongSoLuong().ToString();
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                Session["SoHang"] = TongSoLuong().ToString();
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
                iTongTien = lstGioHang.Sum(n => n.dThanhTien);
            return iTongTien;
        }
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            /*if (lstGioHang.Count == 0)
                return RedirectToAction("Index", "TrangChu");*/
            
            //ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);

        }
        public ActionResult GiohangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGiohang(int iMaSP)
        {
            //Lay gio hang tu Session
            List<GioHang> lstGioHang = LayGioHang();
            //Kiem tra sach da co trong Session["Giohang"]
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMaHH == iMaSP);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMaHH == iMaSP);
                Session["SoHang"] = TongSoLuong().ToString();
                return RedirectToAction("GioHang");

            }
            if (lstGioHang.Count == 0)
            {
                Session["SoHang"] = TongSoLuong().ToString();
                return RedirectToAction("Index", "TrangChu");
            }
            Session["SoHang"] = TongSoLuong().ToString();
            return RedirectToAction("GioHang");
        }
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {

            //Lay gio hang tu Session
            List<GioHang> lstGiohang = LayGioHang();
            //Kiem tra sach da co trong Session["Giohang"]
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iMaHH == iMaSP);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            //Lay gio hang tu Session
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            Session["SoHang"] = TongSoLuong().ToString();
            return RedirectToAction("Index", "TrangChu");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiem tra dang nhap

            tb_TaiKhoan tk = (tb_TaiKhoan)Session["TaiKhoanKH"];
            tb_TaiKhoan tk2 = data.tb_TaiKhoans.Where(n => n.TenTK == tk.TenTK).SingleOrDefault();
            ViewBag.DiaChi = tk2.DiaChi;
            //Lay gio hang tu Session
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            tb_HoaDon ddh = new tb_HoaDon();
            tb_TaiKhoan tk = (tb_TaiKhoan)Session["TaiKhoanKH"];
            

            //Them Don hang
            var dChi = collection["DiaChi"];
            if (String.IsNullOrEmpty(dChi))
            {
                ViewData["LoiDC"] = "Vui Lòng nhập Địa Chỉ";
                return this.DatHang();
            }
            else
            {

                tb_TaiKhoan tk2 = data.tb_TaiKhoans.Where(n => n.TenTK == tk.TenTK).SingleOrDefault();
                    tk2.DiaChi = dChi;
                UpdateModel(tk2);
                data.SubmitChanges();
            }
            
                
            
            

            List<GioHang> gh = LayGioHang();
            ddh.Tong = long.Parse(TongTien().ToString());
            ddh.TenTK = tk.TenTK;
            ddh.NgayLap = DateTime.Now;
            ddh.NgayDat = DateTime.Now;
            DateTime NgayGiao = DateTime.Today.AddDays(7);
            ddh.NgayGiao = NgayGiao;
            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            data.tb_HoaDons.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang            
            foreach (var item in gh)
            {
                tb_CTHD ctdh = new tb_CTHD();
                ctdh.MaHD = ddh.MaHD;
                ctdh.MaHH = item.iMaHH;
                ctdh.SL = item.iSoLuong;
                ctdh.GiaBan = (long)item.dGiaBan;
                data.tb_CTHDs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["SoHang"] = 0;
            Session["Giohang"] = null;
            return RedirectToAction("XacNhanDonHang", "Giohang");
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }


}
