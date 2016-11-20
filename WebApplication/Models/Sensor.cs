using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Sensor
    {
        public int ID { get; set; }
        public int SensorTypeID { get; set; }
        public double ReadingValue { get; set; }
    }
}