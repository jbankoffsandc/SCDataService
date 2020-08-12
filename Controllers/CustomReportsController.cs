using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataService.Data;
using DataService.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataService.Controllers
{
    
    public class CustomReportsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public CustomReportsController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        // GET: api/<CustomReportsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CustomReportsController>/5
        [Route("CustomReports/GetKurtsReport")]
        [HttpGet("{id}")]
        public async Task<JsonResult>  GetKurtsReport(DateRange dr)
        {
            using (MySqlConnection conn = new MySqlConnection(DB.connection_string))
            {
                string query = "";
                query += "SELECT pt.value_stream_id, pt.paint_color, pt.dt_hang, ";
                query += "pt.qty_good, pt.qty_rejected, pt.rejection_code, pt.dt_takedown ";
                query += "FROM paint_tracker pt  ";
                query += "WHERE(pt.dt_takedown IS NOT NULL AND DATE(dt_takedown) >= '" + dr.start_date + "' AND DATE(dt_takedown) <= '" + dr.end_date + "') ";


                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataSet ds = new DataSet();
                ds.Clear();
                da.Fill(ds, "results");
                return Json(new
                {
                    data = ds

                });
            }
        }
        [Route("CustomReports/GetKurtsSummaryReport")]
        [HttpGet("{id}")]
        public async Task<JsonResult> GetKurtsSummaryReport(DateRange dr)
        {
            using (MySqlConnection conn = new MySqlConnection(DB.connection_string))
            {
                // open connection to database
                conn.Open();

                string query = "";
                query += "SELECT DATE(dt_hang) AS dt_hang, SUM(qty_hung) AS qty_hung, SUM(qty_rejected) AS qty_rejected ";
                query += "FROM paint_tracker ";
                query += "WHERE(DATE(dt_hang) >= '" + dr.start_date + "' AND DATE(dt_hang) <= '" + dr.end_date + "') ";
                query += "GROUP BY DATE(dt_hang) ";
                query += "ORDER BY DATE(dt_hang); ";

                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataSet ds = new DataSet();
                ds.Clear();
                da.Fill(ds, "results");
                return Json(new
                {
                    data = ds

                });
            }
        }
        [Route("CustomReports/GetKurtsSummaryPmgReport")]
        [HttpGet("{id}")]
        public async Task<JsonResult> GetKurtsSummaryPmgReport(DateRange dr)
        {
            using (MySqlConnection conn = new MySqlConnection(DB.connection_string))

            {
                // open connection to database
                conn.Open();
                string query = "";
                query += "SELECT DATE(dt_hang) AS dt_hang, SUM(qty_hung) AS qty_hung, SUM(qty_rejected) AS qty_rejected ";
                query += "FROM paint_tracker ";
                query += "WHERE(DATE(dt_hang) >= '" + dr.start_date+ "' AND DATE(dt_hang) <= '" + dr.end_date + "') AND value_stream_id = 37 ";
                query += "GROUP BY DATE(dt_hang) ";
                query += "ORDER BY DATE(dt_hang); ";

                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataSet ds = new DataSet();
                ds.Clear();
                da.Fill(ds, "results");
                return Json(new
                {
                    data = ds

                });

            }
        }
        [Route("CustomReports/GetKurtsRejectReport")]
        [HttpGet("{id}")]
        public async Task<JsonResult> GetKurtsRejectReport(DateRange dateRange)
        {
            using (MySqlConnection conn = new MySqlConnection(DB.connection_string))

            {
                // open connection to database
                conn.Open();
                string query = "";
                query += "SELECT DATE(dt_hang) AS dt_hang, SUM(qty_hung) AS qty_hung, SUM(qty_rejected) AS qty_rejected ";
                query += "FROM paint_tracker ";
                query += "WHERE(DATE(dt_hang) >= '" + dateRange.start_date + "' AND DATE(dt_hang) <= '" + dateRange.end_date + "') AND value_stream_id = 37 ";
                query += "GROUP BY DATE(dt_hang) ";
                query += "ORDER BY DATE(dt_hang); ";

                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataSet ds = new DataSet();
                ds.Clear();
                da.Fill(ds, "results");
                return Json(new
                {
                    data = ds

                });

            }
        }
        [Route("CustomReports/GetPaintedPartsReport")]
        [HttpGet("{id}")]
        public async Task<string> GetPaintedPartsReport()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB.connection_string))
                {
                    string query = "";
                    query += "SELECT pt.auto_id, pt.emp_no_hang, pt.job_order_no, pt.nmr_no, pt.sequence_no, pt.part_no, pt.paint_color, pt.qty_hung,pt.emp_no_transaction,pt.transaction_type_id,pt.qty_complete,pt.qty_moved,pt.dt_transaction, pt.rework ,";
                    query += "DATE_FORMAT(pt.dt_hang, '%m-%d-%Y %H:%i:%s') AS 'dt_hang',DATE_FORMAT(DATE(dt_hang + INTERVAL (7 - DAYOFWEEK(DATE(dt_hang))) DAY), '%m-%d-%Y') AS 'week_ending', ";
                    query += "pt.emp_no_takedown, pt.qty_good, pt.qty_rejected,mt.transaction_type, m.reject_text, pt.rejection_notes, m.paint_non_paint, m.work_station, ";
                    query += "DATE_FORMAT(pt.dt_takedown, '%m-%d-%Y %H:%i:%s') AS 'dt_takedown',e1.last_name as name_hang,e2.last_name as name_takedown, mi.value_stream FROM paint_tracker pt ";
                    query += "LEFT JOIN master_reject_code m ON(pt.rejection_code = m.reject_code) ";
                    query += " LEFT JOIN master_transaction_type mt ON(pt.transaction_type_id = mt.transaction_id)";
                    query += "LEFT JOIN sandc_mildew.master_value_stream mi ON(pt.value_stream_id = mi.value_stream_id)";
                    query += "LEFT JOIN employees_hidden e1 ON (pt.emp_no_hang = e1.employee_no) ";
                    query += "LEFT JOIN employees_hidden e2 ON (pt.emp_no_takedown = e2.employee_no) ";
                    query += "WHERE(";
                    query += "pt.dt_takedown >= '2020-07-15'";
                    //query += "pt.dt_takedown >=DATE(NOW()) - INTERVAL 1 DAY";

                    query += ")";
                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    conn.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    ds.Clear();
                    da.Fill(dt);
                    conn.Close();
                    return PopulatePaintTracker(dt);
                }
            }
            catch(Exception ex)
            {

                return ex.Message;

               
            }
        }
        private string PopulatePaintTracker(DataTable dt)
        {

            GridClass[] arrReturn = GridPaintTrackerData.GetPaintTrackerData(dt);
            int arry_size = arrReturn.Length;

            for (int i = 0; i < arry_size; i++)
            {
               // arrReturn[i].edit_item = arrReturn[i].edit_item.ToString();
                arrReturn[i].emp_no_hang = arrReturn[i].emp_no_hang.ToString();
                arrReturn[i].job_order_no = arrReturn[i].job_order_no.ToString();
                arrReturn[i].nmr_no = arrReturn[i].nmr_no.ToString();
                arrReturn[i].sequence_no = arrReturn[i].sequence_no.ToString();
                arrReturn[i].part_no = arrReturn[i].part_no.ToString();
                arrReturn[i].value_stream_id = arrReturn[i].value_stream_id.ToString();
                arrReturn[i].paint_color = arrReturn[i].paint_color.ToString();
                arrReturn[i].qty_hung = arrReturn[i].qty_hung.ToString();
                arrReturn[i].dt_hang = arrReturn[i].dt_hang.ToString();
                arrReturn[i].shift = arrReturn[i].shift.ToString();
                arrReturn[i].emp_no_takedown = arrReturn[i].emp_no_takedown.ToString();
                arrReturn[i].qty_good = arrReturn[i].qty_good.ToString();
                arrReturn[i].qty_rejected = arrReturn[i].qty_rejected.ToString();
                arrReturn[i].rejection_code = arrReturn[i].rejection_code.ToString();
                arrReturn[i].rejection_notes = arrReturn[i].rejection_notes.ToString();
                arrReturn[i].dt_takedown = arrReturn[i].dt_takedown.ToString();
                arrReturn[i].emp_no_transaction = arrReturn[i].emp_no_transaction.ToString();
                arrReturn[i].transaction_type_id = arrReturn[i].transaction_type_id.ToString();
                arrReturn[i].qty_complete = arrReturn[i].qty_complete.ToString();
                arrReturn[i].qty_moved = arrReturn[i].qty_moved.ToString();
                arrReturn[i].dt_transaction = arrReturn[i].dt_transaction.ToString();
                arrReturn[i].rework = arrReturn[i].rework.ToString();
                arrReturn[i].week_ending = arrReturn[i].week_ending.ToString();
                arrReturn[i].paint_non_paint = arrReturn[i].paint_non_paint.ToString();
                arrReturn[i].work_station = arrReturn[i].work_station.ToString();

            }
            string data = JsonConvert.SerializeObject(arrReturn);
            return "{\"data\": " + data + "}";
            //return data;

            
        }
        // POST api/<CustomReportsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CustomReportsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomReportsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
