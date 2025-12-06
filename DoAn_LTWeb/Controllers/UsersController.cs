using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_LTWeb.Models;

namespace DoAn_LTWeb.Controllers
{
    public class UsersController : Controller
    {
        DB_DoAN_ShopEntities db = new DB_DoAN_ShopEntities();
        // GET: Users
        public ActionResult UsersInformation()
        {
            int id = 1;
            User X = db.Users.Where(x => x.ID == id).FirstOrDefault();
            return View(X);
        }

        // 2. Danh sách Lịch sử đơn hàng
        public ActionResult LichSuDonHang()
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            int idUser = int.Parse(Session["ID"].ToString());

            // Lấy danh sách hóa đơn của user đó, sắp xếp ngày mới nhất lên đầu
            var dsDonHang = db.HoaDons.Where(hd => hd.ID == idUser)
                                      .OrderByDescending(hd => hd.NgayTao)
                                      .ToList();

            return View(dsDonHang);
        }

        // 3. Xem Chi tiết đơn hàng cụ thể
        public ActionResult ChiTietDonHang(int id) // id ở đây là MaHD
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            int idUser = int.Parse(Session["ID"].ToString());

            // Lấy hóa đơn theo Mã HĐ
            var donHang = db.HoaDons.FirstOrDefault(hd => hd.MaHD == id);

            // Bảo mật: Kiểm tra xem đơn hàng này có đúng là của User đang đăng nhập không
            if (donHang == null || donHang.ID != idUser)
            {
                return RedirectToAction("LichSuDonHang"); // Không phải của mình thì đẩy về
            }

            return View(donHang);
        }

    }
}