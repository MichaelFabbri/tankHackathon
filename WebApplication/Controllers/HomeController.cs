using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebApplication.Models;
using Newtonsoft.Json;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        protected override void Execute(RequestContext requestContext)
        {
            TankController ctr = new TankController();
            Tank[] tanks = JsonConvert.DeserializeObject<Tank[]>(ctr.GetActive());
            List<string> ActiveTanks = new List<string>();
            foreach (Tank t in tanks)
            {
                ActiveTanks.Add(t.Name);
            }
            ViewBag.ActiveTanks = ActiveTanks.ToArray();
            base.Execute(requestContext);
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}