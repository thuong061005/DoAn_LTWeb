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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult XuLi_ChinhSua_SanPham(SanPham sp, HttpPostedFileBase img)
        //{

        //}
    }
}