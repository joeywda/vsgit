using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DefaultController : Controller
    {
        private readonly dbToDoEntities db = new dbToDoEntities();
        // GET: Default
        public ActionResult Index2()
        {
            var todos = db.tToDo.OrderByDescending(m => m.fDate).ToList();
            return View(todos);
        }
    }
}