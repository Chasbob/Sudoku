using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sudoku.Controllers
{
    [Authorize]
    public class AccessController : Controller
    {
        // GET: Access
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.message = "Testing user access levels";
            return View();
        }
        [Authorize(Roles ="Admin")]
        public ActionResult Admin()
        {
            ViewBag.message = "you're an admin";
            return View();
        }
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Manager()
        {
            ViewBag.message = "you're a manager";
            return View();
        }
        [Authorize(Roles = "Admin,Manager,Employee")]
        public ActionResult Employee()
        {
            ViewBag.message = "you're an Employee";
            return View();
        }
        [Authorize(Roles ="Admin,pork")]
        public ActionResult Porker()
        {
            ViewBag.message = "What a porker";
            return View();
        }

    }
}