using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopDTDD.Models;
namespace ShopDTDD.Controllers
{
    public class TrangChuController : Controller
    {
        dbQLBHDataContext data = new dbQLBHDataContext();
        private List<tb_HangHoa> LayHangmoi(int count)
        {
            //Sắp xếp sách theo ngày cập nhật, sau đó lấy top @count 
            return data.tb_HangHoas.OrderByDescending(a => a.MaHH).Take(count).ToList();
        }
        public ActionResult SPTuongTu(String id)
        {
            var hang = from s in data.tb_HangHoas where s.MaLoai == id  select s;
            return PartialView(hang);
        }
        public ActionResult SPTheoLoai(String id)
        {
            var hang = from s in data.tb_HangHoas where s.MaLoai == id select s;
            return PartialView(hang);
        }
        public ActionResult SPTheodanhmuc(String id)
        {
            var hang = from s in data.tb_HangHoas where s.MaLoai == id select s;
            ViewBag.ID = id;
            return View(hang);
        }
        public ActionResult HinhAnh(int id)
        {
            var img = from ha in data.tb_HinhAnhs where ha.MaHH==id select ha;
            return PartialView(img);
        }

        public ActionResult Details(int id)
        {
            var hang = from s in data.tb_HangHoas
                       where s.MaHH == id
                       select s;
            return View(hang.Single());
        }
        // GET: TrangChu
        public ActionResult Index()
        {
            var hangmoi = LayHangmoi(15);
            return View(hangmoi);
        }
        public ActionResult DanhMuc()
        {
            var danhmuc = from ten in data.tb_Menus select ten;
            return PartialView(danhmuc);
        }

        public ActionResult DanhMuc2()
        {
            var danhmuc2 = from ten in data.tb_Menus select ten;
            return PartialView(danhmuc2);
        }
    }
}