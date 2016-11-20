using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Tank
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int SpeciesID { get; set; }
        public bool Active { get; set; }
        public DateTime LifeCycleStart { get; set; }
        public DateTime LifeCycleEnd { get; set; }
    }
}