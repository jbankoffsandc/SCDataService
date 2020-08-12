using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataService.Models;
using DataService.Data;
using DataService.Models.DatabaseModels;
using System.Data;
using MySql.Data.MySqlClient;
using DataService.Extensions;

namespace DataService.Controllers                       
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public  IActionResult Index()
        {
             return View();
        }
        [Route("Home/GetHangJobData/{trackerType}")]
        [HttpGet]
        public async Task<JsonResult> GetHangJobData(string trackerType)
        {
            List<HangJob> hj = new List<HangJob>();
            DataTable dt = new DataTable();
            using (MySqlConnection con = new MySqlConnection(DB.connection_string))
            {
                using (MySqlCommand cmd = new MySqlCommand("GetHangJobs", con))
                {
                    //string trackerType = "EFS";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@typeTracker", trackerType);
                    using (MySqlDataAdapter sda = new MySqlDataAdapter())
                    {
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);

                    }
                }
            }

            return Json(new
            {
                data = dt

            });
        }
        [HttpGet]
        public async Task<JsonResult> GetTrackerTakedown()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection con = new MySqlConnection(DB.connection_string))
            {
                using (MySqlCommand cmd = new MySqlCommand("GetTrackerTakedown", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (MySqlDataAdapter sda = new MySqlDataAdapter())
                    {
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);

                    }
                }
            }
            
            return Json(new
            {
                data = dt

            });
        }
        [Route("Home/InsertHangJob/{trackerType}")]
        [HttpPost]
        public async Task<JsonResult> InsertHangJob(string trackerType)
        {
            try
            {
                HangJob hj = new HangJob();
                hj.jobordernumber = Request.Form["data[0][job_order_no]"].FirstOrDefault();
                hj.lastname = Request.Form["data[0][emp_ln_hang]"].FirstOrDefault();
                hj.sequence_number = Request.Form["data[0][sequence_no]"].FirstOrDefault();
                hj.part_number = Request.Form["data[0][part_no]"].FirstOrDefault();
                hj.nmr_number = Request.Form["data[0][nmr_no]"].FirstOrDefault();
                hj.paint_color = Request.Form["data[0][paint_color]"].FirstOrDefault();
                hj.value_stream = Request.Form["data[0][value_stream_id]"].FirstOrDefault();
                hj.qty_hung = "1";


                string query = String.Empty;

                query = "INSERT INTO paint_tracker (auto_id, emp_no_hang, job_order_no, nmr_no, sequence_no, ";
                query += "part_no, value_stream_id, paint_color, qty_hung, dt_hang, rework, from_app) VALUES (";
                query += "NULL, ";
                query += "" + hj.hangjob_id + ", ";
                query += "'" + NJS_Helper.FormatStringforDB(hj.jobordernumber) + "', ";
                query += "'" + NJS_Helper.FormatStringforDB(hj.nmr_number) + "', ";
                query += "'" + NJS_Helper.FormatStringforDB(hj.sequence_number) + "', ";
                query += "'" + NJS_Helper.FormatStringforDB(hj.part_number) + "', ";
                query += "'" + NJS_Helper.FormatStringforDB(hj.value_stream) + "', ";
                query += "'" + App_Helper.GetPaintCode() + NJS_Helper.FormatStringforDB(hj.paint_color) + "', ";
                query += "" + NJS_Helper.FormatStringforDB(hj.qty_hung) + ", ";
                query += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                //if (reworkCB.Checked == true)
                //{
                //    query += "'Y' ";
                //}
                //else
                //{
                query += "'N' ";
                //}
                query += ", 'Paint " + trackerType + "'";
                query += ")";
                using (MySqlConnection con = new MySqlConnection(DB.connection_string))
                {
                    MySqlCommand myCommand = new MySqlCommand(query, con);
                    myCommand.Connection.Open();
                    myCommand.ExecuteNonQuery();
                    con.Close();
                }
                // to be implemented to store data in tables
                return Json(new
                {
                    data = "Record Inserted"

                });
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return Json(new
                {
                    data = ex.Message

                }) ;
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateHangJob()
        {
            try
            {
                HangJob hj = new HangJob();
                hj.jobordernumber = Request.Form["data[123][job_order_no]"].FirstOrDefault();
                hj.lastname = Request.Form["data[123][emp_ln_hang]"].FirstOrDefault();
                hj.sequence_number = Request.Form["data[123][sequence_no]"].FirstOrDefault();
                hj.part_number = Request.Form["data[123][part_no]"].FirstOrDefault();
                hj.nmr_number = Request.Form["data[123][nmr_no]"].FirstOrDefault();
                hj.paint_color = Request.Form["data[123][paint_color]"].FirstOrDefault();
                hj.value_stream = Request.Form["data[123][value_stream_id]"].FirstOrDefault();
                hj.qty_hung = "1";

                string query = String.Empty;
                query = "UPDATE paint_tracker SET ";
                query += "job_order_no = '" + NJS_Helper.FormatStringforDB(hj.jobordernumber) + "', ";
                query += "nmr_no = '" + NJS_Helper.FormatStringforDB(hj.nmr_number) + "', ";
                query += "sequence_no = '" + NJS_Helper.FormatStringforDB(hj.sequence_number) + "', ";
                query += "part_no = '" + NJS_Helper.FormatStringforDB(hj.part_number) + "', ";
                query += "value_stream_id = '" + NJS_Helper.FormatStringforDB(hj.value_stream) + "', ";
                query += "paint_color = '" + App_Helper.GetPaintCode() + NJS_Helper.FormatStringforDB(hj.paint_color) + "', ";
                query += "qty_hung = " + NJS_Helper.FormatStringforDB(hj.qty_hung) + ", ";
                //if (reworkCB.Checked == true)
                //{
                //    query += "rework= 'Y' ";
                //}
                //else
                //{
                query += "rework= 'N' ";
                //}
                // query += "WHERE (auto_id = " + auto_id + ")";

                using (MySqlConnection con = new MySqlConnection(DB.connection_string))
                {
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    // to be implemented to store data in tables

                    return Json(new
                    {
                        data = "Not Implemented"

                    });
                }
            }
            catch (Exception ex)
            {
                return Json( new
                {
                    data = ex.Message

                });
            }
        }
        [Route("Home/DeleteHangJob")]
        [HttpPost]
        public async Task<JsonResult> DeleteHangJob()
        {

            // to be implemented to delete record from table


            return Json(new
            {
                data = "Not Implemented"

            }) ;
        }

        [HttpGet]
        public async Task<JsonResult> GetPaintTrackerData()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection con = new MySqlConnection(DB.connection_string))
            {
                string query = null;
                //open connection to database
                con.Open();
                query += "SELECT pt.auto_id, pt.emp_no_hang, pt.job_order_no, pt.nmr_no, pt.sequence_no, pt.part_no, pt.paint_color, pt.qty_hung,pt.emp_no_transaction,pt.transaction_type_id,pt.qty_complete,pt.qty_moved,pt.dt_transaction, pt.rework ,";
                query += "DATE_FORMAT(pt.dt_hang, '%m-%d-%Y %H:%i:%s') AS 'dt_hang',DATE_FORMAT(DATE(dt_hang + INTERVAL (7 - DAYOFWEEK(DATE(dt_hang))) DAY), '%m-%d-%Y') AS 'week_ending', ";
                query += "pt.emp_no_takedown, pt.qty_good, pt.qty_rejected, m.reject_text, pt.rejection_notes, m.paint_non_paint, m.work_station, ";
                query += "DATE_FORMAT(pt.dt_takedown, '%m-%d-%Y %H:%i:%s') AS 'dt_takedown',mt.transaction_type,e1.last_name as name_hang,e2.last_name as name_takedown, mi.value_stream FROM paint_tracker pt ";
                query += "LEFT JOIN master_reject_code m ON(pt.rejection_code = m.reject_code) ";
                query += " LEFT JOIN master_transaction_type mt ON(pt.transaction_type_id = mt.transaction_id)";
                query += "LEFT JOIN sandc_mildew.master_value_stream mi ON(pt.value_stream_id = mi.value_stream_id)";
                query += "LEFT JOIN employees_hidden e1 ON (pt.emp_no_hang = e1.employee_no) ";
                query += "LEFT JOIN employees_hidden e2 ON (pt.emp_no_takedown = e2.employee_no) limit 0,10";
                MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                ds.Clear();
                da.Fill(dt);
                return Json(new
                {
                    data = dt

                });

            }


           
                
            
        }
    
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
