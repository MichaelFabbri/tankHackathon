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
            command += tank.ID.ToString() + ",";
            command += tank.SpeciesID.ToString() + ",";
            command += tank.Active.ToString() + ",";
            command += tank.LifeCycleStart.ToString() + ",";
            command += tank.LifeCycleEnd.ToString() + ")";

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
        public string EndTank(int ID)
        {
            string command = "UPDATE Tank SET Active=False WHERE ID=" + ID.ToString();

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
            string command = "DECLARE @tankID int = SELECT ID FROM Table WHERE Active=true AND Name = " + TankName;
            command += ";INSERT INTO Reading (TankID, ReadTime) VALUES (@tankID, GETDATE());";
            command += "DECLARE @readingID int = @@IDENTITY";
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
            catch { return "Bad"; }
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
            command += model.Title + ",";
            command += model.Content + ",";
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
    }
}