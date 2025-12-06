using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class LoginController : Controller
    {
        DB_DoAN_ShopEntities db = new DB_DoAN_ShopEntities();
        // GET: Login
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]  
        public ActionResult XuLiDangNhap(string tenDN, string mk)
        {
            //Kiểm tra
            if(string.IsNullOrEmpty(tenDN) || string.IsNullOrEmpty(mk)) {
                ViewBag.ThongBao = "Vui lòng điền thông tin đăng nhập";
                return View("DangNhap");
            }
            User X = db.Users.Where(x => x.UserName == tenDN && x.Password == mk).FirstOrDefault();
            if(X != null)
            {
                Session["ID"] = X.ID;
                Session["VaiTro"] = X.VaiTro;
                Session["HoTen"] = X.HoTen;
                return RedirectToAction("View_TrangChu", "Home");
            }
            ViewBag.ThongBao = "Sai tài khoản hoặc mật khẩu!";
            return View("DangNhap");
        }

        //Đăng ký------------------------------------------------------
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XuLiDangKy(FormCollection f)
        {
            //Láy dữ liệu được gửi về từ Form đăng ký
            string HoTen = f["HoTen"];
            string SDT = f["SDT"]?.Trim();
            string Email = f["Email"]?.Trim();
            string GioiTinh = f["GioiTinh"]?.Trim();
            string UserName = f["UserName"]?.Trim();
            string Password = f["Password"];
            string rePassword = f["rePassword"];
            string VaiTro = "User";
            //Kiểm tra nhập thông tin bắt buộc
            if (string.IsNullOrEmpty(HoTen) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                ViewBag.ThongBao = "Vui lòng điền đầy đủ thông tin!";
                return View("DangKy");
            }
            //Kiểm tra xác nhận mật khẩu không đúng
            if(rePassword != Password)
            {
                ViewBag.ThongBao = "Mật khẩu không trùng khớp!";
                return View("DangKy");
            }
            //Kiểm tra trung lặp Username
            if(db.Users.Any(n => n.UserName == UserName))
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại.";
                return View("DangKy");
            }
            //Kiểm tra Email trùng
            if (db.Users.Any(n => n.Email == Email))
            {
                ViewBag.ThongBao = "Email này đã được sử dụng.";
                return View("DangKy");
            }
            //Nếu không lỗi, thực hiện tạo người dùng mới
            User X = new User();
            X.HoTen = HoTen;
            X.SDT = SDT;
            X.Email = Email;
            X.GioiTinh = GioiTinh;
            X.UserName = UserName;
            X.Password = Password;
            X.VaiTro = VaiTro;
            db.Users.Add(X);
            db.SaveChanges();
            ViewBag.ThongBao = "Đăng ký thành công!";
            return View("DangNhap");
        }
        //-----------Đăng xuất----------------------------------------
        public ActionResult DangXuat()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("View_TrangChu","Home");
        }
    }


}