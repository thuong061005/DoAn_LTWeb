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

        // 5. Hiển thị trang xác nhận thanh toán
        public ActionResult ThanhToan()
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            int userID = int.Parse(Session["ID"].ToString());

            // Lấy thông tin giỏ hàng để hiển thị lại cho user kiểm tra
            var listGioHang = db.GioHangs.Where(gh => gh.ID == userID).ToList();

            if (listGioHang.Count == 0)
            {
                return RedirectToAction("Index"); // Giỏ hàng trống thì quay lại
            }

            // Lấy thông tin người dùng để điền sẵn vào form
            var user = db.Users.Find(userID);
            ViewBag.User = user;

            // Tính tổng tiền
            ViewBag.TongTien = listGioHang.Sum(x => x.ThanhTien);

            return View(listGioHang);
        }

        // 6. Xử lý đặt hàng (POST)
        [HttpPost]
        public ActionResult DatHang(string diaChiNhanHang, string ghiChu)
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            int userID = int.Parse(Session["ID"].ToString());
            var listGioHang = db.GioHangs.Where(gh => gh.ID == userID).ToList();

            if (listGioHang.Count == 0) return RedirectToAction("Index");

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // A. Tạo Hóa Đơn (HoaDon)
                    HoaDon hd = new HoaDon();
                    hd.ID = userID;
                    hd.NgayTao = DateTime.Now;
                    hd.TongTien = listGioHang.Sum(x => x.ThanhTien);
                    // hd.DiaChi = diaChiNhanHang; // Nếu DB có cột địa chỉ nhận hàng
                    // hd.GhiChu = ghiChu;        // Nếu DB có cột ghi chú

                    // Giả sử phí vận chuyển cố định hoặc tính toán logic khác
                    hd.VanChuyen = 30000;
                    hd.ThanhTien = hd.TongTien + hd.VanChuyen;

                    db.HoaDons.Add(hd);
                    db.SaveChanges(); // Lưu để lấy MaHD vừa tạo

                    // B. Tạo Chi Tiết Hóa Đơn (CTHD)
                    foreach (var item in listGioHang)
                    {
                        CTHD cthd = new CTHD();
                        cthd.MaHD = hd.MaHD;
                        cthd.MaSP = item.MaSP;

                        cthd.SoLuong = item.SoLuong;
                        cthd.ThanhTien = item.ThanhTien;

                        db.CTHDs.Add(cthd);

                        // C. Trừ tồn kho (Optional)
                        var sp = db.SanPhams.Find(item.MaSP);
                        if (sp != null)
                        {
                            sp.SoLuong -= item.SoLuong;
                        }
                    }

                    db.SaveChanges();

                    // D. Xóa Giỏ Hàng sau khi đặt thành công
                    db.GioHangs.RemoveRange(listGioHang);
                    db.SaveChanges();

                    transaction.Commit();

                    // Chuyển hướng đến trang thông báo thành công
                    return RedirectToAction("DatHangThanhCong");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Error"] = "Có lỗi xảy ra khi đặt hàng: " + ex.Message;
                    return RedirectToAction("ThanhToan");
                }
            }
        }

        // 7. Trang thông báo thành công
        public ActionResult DatHangThanhCong()
        {
            return View();
        }
    }
}