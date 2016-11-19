using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class LifecycleInfo
    {
        public int ID { get; set; }
        public int tankID { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }
}