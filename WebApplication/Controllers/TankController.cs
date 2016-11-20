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
            string command2 = "SELECT * FROM Sensor, SensorType WHERE Sensor.SensorTypeID = SensorType.ID AND ReadingID=";
            SqlConnection conn = new SqlConnection(_static.dbconn);
            SqlCommand cmd = new SqlCommand(command, conn);
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                reader.Read();
                output.ID = Convert.ToInt32(reader["ID"]);
                output.TankID = Convert.ToInt32(reader["TankID"]);
                output.time = Convert.ToDateTime(reader["ReadTime"]);
                reader.Close();
                command2 += output.ID.ToString();
                SqlCommand cmd2 = new SqlCommand(command2, conn);
                reader = cmd2.ExecuteReader();
                reader.Read();
                List<Sensor> sensor = new List<Sensor>();
                do
                {
                    sensor.Add(new Sensor()
                    {
                        SensorName = reader["Name"].ToString(),
                        SensorTypeID = Convert.ToInt32(reader["SensorTypeID"]),
                        ReadingValue = Convert.ToDouble(reader["ReadingValue"])
                    });
                } while (reader.Read());
                output.sensors = sensor.ToArray();
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
            string command = "SELECT * FROM Tank WHERE Active = 1 ORDER BY Name";
            SqlConnection conn = new SqlConnection(_static.dbconn);
            SqlCommand cmd = new SqlCommand(command, conn);
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                reader.Read();
                do
                {
                    Tank t = new Tank();
                    t.ID = Convert.ToInt32(reader["ID"]);
                    t.Name = reader["Name"].ToString();
                    t.SpeciesID = Convert.ToInt32(reader["SpeciesID"]);
                    t.Active = Convert.ToBoolean(reader["Active"]);
                    t.LifeCycleStart = Convert.ToDateTime(reader["LifeCycleStart"]);
                    t.LifeCycleEnd = Convert.ToDateTime(reader["LifeCycleEnd"]);
                    output.Add(t);
                } while (reader.Read());
            }
            catch (Exception ex) { return ex.Message; }
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
        //public string AddReading(Readings model, string TankName)
        //{
        //    string command = "DECLARE @tankID int = (SELECT TOP(1) ID FROM Tank WHERE Active = 1 AND Name = '" + TankName + "')";
        //    command += ";INSERT INTO Reading (TankID, ReadTime) VALUES (@tankID, GETDATE());";
        //    command += "DECLARE @readingID int = @@IDENTITY;";
        //    foreach (Sensor s in model.sensors)
        //    {
        //        command += "INSERT INTO Sensor (SensorTypeID, ReadingValue, readingID) VALUES (";
        //        command += s.SensorTypeID.ToString() + ",";
        //        command += s.ReadingValue.ToString() + ",@readingID);";
        //    }

        //    SqlConnection conn = new SqlConnection(_static.dbconn);
        //    SqlCommand cmd = new SqlCommand(command, conn);
        //    try
        //    {
        //        conn.Open();
        //        var reader = cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception ex) { return "Bad: " + ex.Message; }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //    return "Good";
        //}
        public string AddReading(string json, string TankName)
        {
            string decoded = HttpUtility.UrlDecode(json);
            Readings model = JsonConvert.DeserializeObject<Readings>(decoded);
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

        public string ActiveTankHistory()
        {
            return "";
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
            string x = "%7B%22TankName%22%3A%22A1%22%2C+%22model%22%3A%7B%22sensors%22%3A%5B%7B%22SensorTypeID%22%3A1%2C%22ReadingValue%22%3A1.0%7D%2C%7B%22SensorTypeID%22%3A2%2C%22ReadingValue%22%3A1.0%7D%2C%7B%22SensorTypeID%22%3A3%2C%22ReadingValue%22%3A1.0%7D%2C%7B%22SensorTypeID%22%3A4%2C%22ReadingValue%22%3A1.0%7D%2C%7B%22SensorTypeID%22%3A5%2C%22ReadingValue%22%3A1.0%7D%5D%7D";
            string z = HttpUtility.UrlDecode(x);
            string a = z;
            return "";// TestAddReading();
        }

        public string ActiveBranchAll(int ID)
        {
            List<Readings> output = new List<Readings>();
            string command = "SELECT * FROM Reading, Sensor, SensorType WHERE Reading.ID = Sensor.ReadingID AND Sensor.SensorTypeID = SensorType.ID AND Reading.TankID=" + ID.ToString();
            
            SqlConnection conn = new SqlConnection(_static.dbconn);
            SqlCommand cmd = new SqlCommand(command, conn);
            try
            {
                int PrevReadID = 0;
                Readings curr;
                List<Sensor> ls = new List<Sensor>();
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    PrevReadID = Convert.ToInt32(reader["ReadingID"]);
                    curr = new Readings()
                    {
                        TankID = Convert.ToInt32(reader["TankID"]),
                        time = Convert.ToDateTime(reader["ReadTime"])
                    };
                    output.Add(curr);
                    do
                    {
                        if (Convert.ToInt32(reader["ReadingID"]) != PrevReadID)
                        {
                            PrevReadID = Convert.ToInt32(reader["ReadingID"]);
                            curr.sensors = ls.ToArray();
                            ls = new List<Sensor>();
                            curr = new Readings()
                            {
                                TankID = Convert.ToInt32(reader["TankID"]),
                                time = Convert.ToDateTime(reader["ReadTime"])
                            };
                            output.Add(curr);
                        }
                        ls.Add(new Sensor()
                        {
                            SensorName = reader["Name"].ToString(),
                            ReadingValue = Convert.ToDouble(reader["readingValue"])
                        });
                    } while (reader.Read());
                    curr.sensors = ls.ToArray();
                }
            }
            catch (Exception ex) { return "Bad: " + ex.Message; }
            finally
            {
                conn.Close();
            }
            return JsonConvert.SerializeObject(output);
        }

        public string ActiveBranchType(int ID, int typeID)
        {
            string all = ActiveBranchAll(ID);
            List<Readings> readings = JsonConvert.DeserializeObject<List<Readings>>(all);
            List<Readings> output = new List<Readings>();
            foreach (Readings read in readings)
            {
                read.sensors = new Sensor[1] { read.sensors.FirstOrDefault(x => x.SensorTypeID == typeID) };
            }
            return JsonConvert.SerializeObject(readings);
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