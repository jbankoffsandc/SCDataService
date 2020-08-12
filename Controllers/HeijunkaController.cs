using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeijunkaController : Controller
    {
       // GET: api/<ValuesController>
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            string query = null;
            using (MySqlConnection conn = new MySqlConnection(DB.connection_string))
            {
                //open connection to database
                conn.Open();

                query = "SELECT a.auto_id, a.shift, a.value_stream, v.value_stream_id, v.value_stream_name, a.datetime, a.shift_start_dt, a.shift_end_dt ";
                query += "FROM admin_heijuhka_setup a, value_streams v ";
              //  query += "WHERE(shift = '" + this.ShiftLbl.InnerHtml + "' AND a.shift_start_dt LIKE '%" + this.DateLbl.InnerHtml + "%' AND a.value_stream = v.value_stream_id) ";
                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataSet ds = new DataSet();
                ds.Clear();
                da.Fill(ds, "results");
                return Json(new
                {
                    data = ds.Tables["results"]

                }) ; 

            }
        }

        // GET api/<ValuesController>/5
        [HttpGet()]
        public async Task<JsonResult> GetHeijunkaSetup(DateRange dr)
        {
            using (MySqlConnection conn = new MySqlConnection(DB.connection_string))
            {
               {
                    // open connection to database
                    conn.Open();
                    string query1 = "SELECT p.sequence_no, p.value_stream_id, p.qty_hung, p.qty_rejected, v.value_stream_id, v.value_stream_name, v.mildew_id, TIME(p.dt_hang)  AS dt_hang  FROM paint_tracker p, value_streams v ";
                    query1 += "WHERE(dt_hang >= '" + dr.start_date + "' AND dt_hang <= '" + dr.end_date + "' ) GROUP BY sequence_no";
                    MySqlDataAdapter da_tracker = new MySqlDataAdapter(query1, conn);
                    DataSet ds_tracker = new DataSet();
                    ds_tracker.Clear();
                    da_tracker.Fill(ds_tracker, "tracker_results");

                    string query2 = "SELECT p.sequence_no, p.value_stream_id, p.work_center_id, p.stage_completed FROM production_tracker p ";
                    query2 += "WHERE(p.stage_completed >='" + dr.start_date + "' AND p.stage_completed <= '" + dr.end_date + "' AND p.work_center_id = '13') ";
                    MySqlDataAdapter da_production_tracker = new MySqlDataAdapter(query2, conn);
                    DataSet ds_production_tracker = new DataSet();
                    ds_production_tracker.Clear();
                    da_production_tracker.Fill(ds_production_tracker, "production_tracker_results");
                    return Json(new
                    {
                        data1 = ds_tracker.Tables["tracker_results"],
                        data2 = ds_production_tracker.Tables["production_tracker_results"]

                    });
                }
            }
        }

        // GET api/<ValuesController>/5
        [HttpGet()]
        public async Task<JsonResult> GetPaintedPartsReport()
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
                query += "pt.dt_takedown >=DATE(NOW()) - INTERVAL 1 DAY";
                query += ")";

                DataSet ds = new DataSet();
                        conn.Open();
                        MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                        ds.Clear();
                        da.Fill(ds, "results");
                        conn.Close();
                    return Json(new
                    {
                        data = ds.Tables["results"],
                    
                    });

            }
        }
        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
