using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Readings
    {
        public int ID { get; set; }
        public int TankID { get; set; }
        public DateTime time { get; set; }
        public Sensor[] sensors { get; set; }
    }
}
