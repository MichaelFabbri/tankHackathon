using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class TankController : Controller
    {
        public string GetLastReading(int ID)
        {
            Readings output = new Readings();
            string command = "SELECT TOP(1) * FROM Reading WHERE tankID = " + ID.ToString() + " ORDER BY ReadTime DESC";
            SqlConnection conn = new SqlConnection(_static.dbconn);
            SqlCommand cmd = new SqlCommand(command, conn);
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                output.ID = Convert.ToInt32(reader["ID"]);
                output.TankID = Convert.ToInt32(reader["TankID"]);
                output.time = Convert.ToDateTime(reader["ReadTime"]);
            }
            catch { }
            finally
            {
                conn.Close();
            }
            return JsonConvert.SerializeObject(output);
        }
        public string GetAll()
        {
            return "";
        }
        public string GetActive()
        {
            List<Tank> output = new List<Tank>();
            string command = "SELECT * FROM Tank WHERE Active = true ORDER BY Name";
            SqlConnection conn = new SqlConnection(_static.dbconn);
            SqlCommand cmd = new SqlCommand(command, conn);
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                Tank t = new Tank();
                t.ID = Convert.ToInt32(reader["ID"]);
                t.Name = reader["TankID"].ToString();
                t.SpeciesID = Convert.ToInt32(reader["ReadTime"]);
                t.Active = Convert.ToBoolean(reader["Active"]);
                t.LifeCycleStart = Convert.ToDateTime(reader["LifeCycleStart"]);
                t.LifeCycleEnd = Convert.ToDateTime(reader["LifeCycleEnd"]);
            }
            catch { }
            finally
            {
                conn.Close();
            }
            return JsonConvert.SerializeObject(output);
        }
        [HttpPost]
        public string StartTank(Tank tank)
        {
            string command = "INSERT INTO Tank (Name, SpeciesID, Active, LifecycleStart, LifecycleEnd) VALUES (";
            command += "'" + tank.Name + "',";
            command += tank.SpeciesID.ToString() + ",1,";
            command += "'" + tank.LifeCycleStart.ToString("yyyy-MM-dd") + "',";
            command += "'" + tank.LifeCycleEnd.ToString("yyyy-MM-dd") + "')";

            SqlConnection conn = new SqlConnection(_static.dbconn);
            SqlCommand cmd = new SqlCommand(command, conn);
            try
            {
                conn.Open();
                var reader = cmd.ExecuteNonQuery();
            }
            catch(Exception ex) { return "Bad: " + ex.Message; }
            finally
            {
                conn.Close();
            }
            return "Good";
        }
        [HttpPost]
        public string EndTank(int ID)
        {
            string command = "UPDATE Tank SET Active=0 WHERE ID=" + ID.ToString();

            SqlConnection conn = new SqlConnection(_static.dbconn);
            SqlCommand cmd = new SqlCommand(command, conn);
            try
            {
                conn.Open();
                var reader = cmd.ExecuteNonQuery();
            }
            catch { return "Bad"; }
            finally
            {
                conn.Close();
            }
            return "Good";
        }
        [HttpPost]
        public string AddReading(Readings model, string TankName)
        {
            string command = "DECLARE @tankID int = (SELECT TOP(1) ID FROM Tank WHERE Active = 1 AND Name = '" + TankName + "')";
            command += ";INSERT INTO Reading (TankID, ReadTime) VALUES (@tankID, GETDATE());";
            command += "DECLARE @readingID int = @@IDENTITY;";
            foreach (Sensor s in model.sensors)
            {
                command += "INSERT INTO Sensor (SensorTypeID, ReadingValue, readingID) VALUES (";
                command += s.SensorTypeID.ToString() + ",";
                command += s.ReadingValue.ToString() + ",@readingID);";
            }

            SqlConnection conn = new SqlConnection(_static.dbconn);
            SqlCommand cmd = new SqlCommand(command, conn);
            try
            {
                conn.Open();
                var reader = cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { return "Bad: " + ex.Message; }
            finally
            {
                conn.Close();
            }
            return "Good";
        }
        [HttpPost]
        public string AddNote(Note model)
        {

            string command = "INSERT INTO Note (TankID, Title, Content, ";
            if (model.Picture != null)
            {
                command += "Picture, PictureName";
            }
            command += ") VALUES (";
            command += model.TankID.ToString() + ",";
            command += "'" + model.Title + "',";
            command += "'" + model.Content + "',";
            if (model.Picture != null)
            {
                command += "0x" + BitConverter.ToString(model.Picture).Replace("-", "") + ",";
                command += model.PictureName;
            }
            command += "); ";

            SqlConnection conn = new SqlConnection(_static.dbconn);
            SqlCommand cmd = new SqlCommand(command, conn);
            try
            {
                conn.Open();
                var reader = cmd.ExecuteNonQuery();
            }
            catch { return "Bad"; }
            finally
            {
                conn.Close();
            }
            return "Good";
        }
        public string DoStuff()
        {
            EndTank(20);
            //string output = "";
            //string[] tanks = { "A", "B", "C" };
            //foreach (string s in tanks)
            //{
            //    for (int i = 1; i < 6; i++)
            //    {
            //        Tank t = new Tank()
            //        {
            //            Name = s + i.ToString(),
            //            SpeciesID = 1,
            //            LifeCycleEnd = DateTime.Now.AddDays(30),
            //            LifeCycleStart = DateTime.Now
            //        };
            //        output += StartTank(t) + "<br />";
            //    }
            //}
            //return output;
            return TestAddReading();
        }

        public string TestAddReading()
        {
            Readings r = new Readings();
            Sensor s = new Sensor();
            s.ReadingValue = 1.0;
            s.SensorTypeID = 1;
            r.sensors = new Sensor[] { s };
            return JsonConvert.SerializeObject(r);
        }
    }
}