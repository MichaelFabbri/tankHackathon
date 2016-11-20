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
        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            TankController ctr = new TankController();
            Tank[] tanks = JsonConvert.DeserializeObject<Tank[]>(ctr.GetActive());
            List<string> ActiveTanks = new List<string>();
            List<int> ActiveTankIds = new List<int>();
            foreach (Tank t in tanks)
            {
                ActiveTanks.Add(t.Name);
                ActiveTankIds.Add(t.ID);
            }
            ViewBag.ActiveTanks = ActiveTanks.ToArray();
            ViewBag.ActiveTankIds = ActiveTankIds.ToArray();
            return base.BeginExecute(requestContext, callback, state);
        }
        protected override void Execute(RequestContext requestContext)
        {
            
            base.Execute(requestContext);
        }
        public ActionResult Index(int? ID)
        {
            if (ID != null)
            {
                //dear employers, this does not accurately represent my abilities
                //Dear God, I'm sorry.
                string x = new TankController().GetLastReading(ID ?? 0);
                Readings j = JsonConvert.DeserializeObject<Readings>(x);
                double[] RecentReading = new double[5];
                foreach (Sensor s in j.sensors)
                {
                    RecentReading[s.SensorTypeID - 1] = s.ReadingValue;
                }
                ViewBag.RecentReading = RecentReading;
            }
            return View();
        }
    }
}