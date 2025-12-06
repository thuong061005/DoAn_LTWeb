using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn_LTWeb.Models; // Thay bằng namespace model của bạn

namespace DoAn_LTWeb.Controllers
{
    public class GioHangController : Controller
    {
        // Khởi tạo context database
        DB_DoAN_ShopEntities db = new DB_DoAN_ShopEntities();

        // 1. Xem giỏ hàng
        public ActionResult Index()
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("DangNhap", "Login"); // Chưa đăng nhập thì bắt đăng nhập
            }

            int userID = int.Parse(Session["ID"].ToString());

            // Lấy danh sách sản phẩm trong giỏ của user đó, kèm thông tin sản phẩm
            var listGioHang = db.GioHangs.Where(gh => gh.ID == userID).ToList();

            // Tính tổng tiền
            ViewBag.TongTien = listGioHang.Sum(x => x.ThanhTien);

            return View(listGioHang);
        }

        // 2. Thêm vào giỏ hàng
        public ActionResult ThemVaoGio(int maSP, string strURL)
        {
            // Kiểm tra đăng nhập
            if (Session["ID"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            int userID = int.Parse(Session["ID"].ToString());

            // Kiểm tra sản phẩm có tồn tại trong giỏ chưa
            GioHang cartItem = db.GioHangs.FirstOrDefault(x => x.MaSP == maSP && x.ID == userID);

            if (cartItem != null) // Đã có -> Tăng số lượng
            {
                cartItem.SoLuong++;
                // Cập nhật lại thành tiền (đơn giá * số lượng)
                var sanPham = db.SanPhams.Find(maSP);
                cartItem.ThanhTien = cartItem.SoLuong * sanPham.Gia;
            }
            else // Chưa có -> Thêm mới
            {
                var sanPham = db.SanPhams.Find(maSP);
                GioHang newItem = new GioHang();
                newItem.ID = userID;
                newItem.MaSP = maSP;
                newItem.SoLuong = 1;
                newItem.ThanhTien = sanPham.Gia;
                db.GioHangs.Add(newItem);
            }

            db.SaveChanges();
            return Redirect(strURL); // Quay lại trang cũ
        }

        // 3. Cập nhật số lượng (Dùng cho AJAX hoặc Form Post)
        public ActionResult CapNhatGioHang(int maSP, int soLuong)
        {
            if (Session["ID"] == null) return RedirectToAction("DangNhap", "Login");

            int userID = int.Parse(Session["ID"].ToString());
            var item = db.GioHangs.FirstOrDefault(x => x.MaSP == maSP && x.ID == userID);

            if (item != null)
            {
                var sanPham = db.SanPhams.Find(maSP);
                item.SoLuong = soLuong;
                item.ThanhTien = soLuong * sanPham.Gia;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // 4. Xóa khỏi giỏ
        public ActionResult XoaKhoiGio(int maSP)
        {
            if (Session["ID"] == null) return RedirectToAction("DangNhap", "Login");

            int userID = int.Parse(Session["ID"].ToString());
            var item = db.GioHangs.FirstOrDefault(x => x.MaSP == maSP && x.ID == userID);

            if (item != null)
            {
                db.GioHangs.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}