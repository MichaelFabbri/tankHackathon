using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Note
    {
        public int ID { get; set; }
        public int TankID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public byte[] Picture { get; set; }
        public string PictureName { get; set; }
    }
}