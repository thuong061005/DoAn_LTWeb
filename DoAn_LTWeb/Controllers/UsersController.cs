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
        //Giỏ hàng -------------------------------------------------
        public ActionResult GioHang()
        {
            int id = 2;
            var X = db.GioHangs.Where(x => x.ID == id).Include(gh => gh.SanPham).ToList();
            var sanPhamList = X.Select(gh => gh.SanPham).ToList();
            return View(sanPhamList);
        }
    }
}