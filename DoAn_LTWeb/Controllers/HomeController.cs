using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class HomeController : Controller
    {
        DB_DoAN_ShopEntities db = new DB_DoAN_ShopEntities();
        public ActionResult View_TrangChu()
        {
            return View();
        }
        
    }
}