using System.Collections.Generic;
using System.Linq;
using Hive.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hive.Backend.Controllers
{
    public partial class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["ReturnUrl"] = "Home/Index";
            
            if(TempData["createdUser"] != null)
            {
                ViewData["createdUser"] = TempData["createdUser"];
            }
            if (TempData["createUserErrorsModel"] != null)
            {
                var errors = TempData["createUserErrorsModel"].ToString().Split(";").ToList();
                ViewData["ModelErrors"] = errors;
                var model = JsonConvert.DeserializeObject<RegisterViewModel>(TempData["NotValidModel"].ToString());
                return View(model);
            }
            return View();
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}