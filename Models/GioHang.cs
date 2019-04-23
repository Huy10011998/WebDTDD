using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopDTDD.Models
{
    public class GioHang
    {
        dbQLBHDataContext data = new dbQLBHDataContext();
        public int iMaHH { get; set; }
        public string sTenHH { get; set; }
        public string sMaLoai { get; set; }
        public double dGiaBan { get; set; }
        public double dGiaKhuyenMai { get; set; }
        public string sHinhAnh { get; set; }
        public string sMoTaSP { get; set; }
        public int iSoLuong { get; set; }
        public double dThanhTien
        {
            get { return iSoLuong * dGiaBan; }
        }
        public GioHang(int MaHH)
        {
            iMaHH = MaHH;
            tb_HangHoa HangHoa = data.tb_HangHoas.Single(n => n.MaHH == iMaHH);
            sTenHH = HangHoa.TenHH;
            sHinhAnh = HangHoa.HinhAnh;
            dGiaBan = double.Parse(HangHoa.GiaBan.ToString());
            iSoLuong = 1;

        }
    }
}