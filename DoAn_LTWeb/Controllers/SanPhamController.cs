using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class SanPhamController : Controller
    {
        DB_DoAN_ShopEntities db = new DB_DoAN_ShopEntities();
        // GET: SanPham
        public ActionResult View_SanPham(int? id)
        {
            List<DanhMuc> lt_DM = db.DanhMucs.ToList();
            ViewBag.DS_DanhMuc = lt_DM;
            List<SanPham> lt_SP;
            if (id != null)
                lt_SP = db.SanPhams.Where(sp => sp.MaDM == id).ToList();
            else
                lt_SP = db.SanPhams.ToList();
            return View(lt_SP);
        }
        public ActionResult QL_SanPham(int? id)
        {
            List<DanhMuc> lt_DM = db.DanhMucs.ToList();
            ViewBag.DS_DanhMuc = lt_DM;
            List<SanPham> lt_SP;
            if(id != null)
                lt_SP = db.SanPhams.Where(sp => sp.MaDM == id).ToList();
            else
                lt_SP = db.SanPhams.ToList();
            return View(lt_SP);
        }
        //public ActionResult View_SanPham()
        //{

        //}
        //-------------------------------------------------------------------------------------------------------------
        //Thêm Sản Phẩm
        public ActionResult Them_SanPham()
        {
            var DS_DanhMuc = db.DanhMucs.ToList();
            ViewBag.MaDM = new SelectList(DS_DanhMuc, "MaDM", "TenLoai");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XuLiThemSanPham(SanPham sp, HttpPostedFileBase Img)
        {
            //Xử lí hình ảnh
            if (ModelState.IsValid)
            {
                //Sử lí ảnh hoa
                if(Img != null && Img.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Img.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    Img.SaveAs(path);
                    sp.HinhAnh = fileName;
                }
                else
                    sp.HinhAnh = "sp.jpg";             
            }
            db.SanPhams.Add(sp);
            db.SaveChanges();
            return RedirectToAction("QL_SanPham");
        }
        //Xóa Sản Phẩm
        //public ActionResult XoaSAnPham(int id)
        //{
        //    SanPham X = db.SanPhams.Where(x => x.MaSP == id).FirstOrDefault();
        //    return View(X);
        //}
        //[HttpPost,ActionName("XoaSAnPham")]
        //[ValidateAntiForgeryToken]
        //public ActionResult XuLiXoaSanPham(int id)
        //{

        //}
        //Sửa Sản Phẩm
        public ActionResult CapNhat_SanPham(int? id)
        {
            var X = db.SanPhams.FirstOrDefault(x => x.MaSP == id);
            return View(X);
        }
        // 1. ACTION HIỂN THỊ CHI TIẾT SẢN PHẨM & PHẢN HỒI
        public ActionResult ChiTietSanPham(int id)
        {
            // Tìm sản phẩm theo ID, đồng thời load luôn danh sách PhanHois và thông tin Users của phản hồi đó
            // Lưu ý: Cần đảm bảo trong Model SanPham có quan hệ với PhanHoi, và PhanHoi có quan hệ với Users
            var sp = db.SanPhams.Include("PhanHois.User").FirstOrDefault(x => x.MaSP == id);

            if (sp == null)
            {
                return HttpNotFound();
            }

            // Đếm số lượng đánh giá để hiển thị (Optional)
            ViewBag.SoLuongDanhGia = sp.PhanHois.Count;

            return View(sp);
        }

        // 2. ACTION XỬ LÝ GỬI PHẢN HỒI (POST)
        [HttpPost]
        public ActionResult GuiPhanHoi(int MaSP, string NoiDung, int DanhGia)
        {
            // Kiểm tra đăng nhập
            if (Session["ID"] == null)
            {
                // Lưu URL hiện tại để quay lại sau khi đăng nhập (nếu muốn)
                return RedirectToAction("DangNhap", "Login");
            }

            try
            {
                PhanHoi ph = new PhanHoi();
                ph.MaSP = MaSP;
                ph.ID = int.Parse(Session["ID"].ToString()); // Lấy ID user từ Session
                ph.NoiDung = NoiDung;
                ph.DanhGia = DanhGia; // Số sao (1-5)
                                      // ph.NgayGui = DateTime.Now; // Nếu trong DB có cột ngày gửi

                db.PhanHois.Add(ph);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                TempData["Error"] = "Có lỗi xảy ra khi gửi đánh giá.";
            }

            // Quay lại trang chi tiết sản phẩm
            return RedirectToAction("ChiTietSanPham", new { id = MaSP });
        }
    }
}