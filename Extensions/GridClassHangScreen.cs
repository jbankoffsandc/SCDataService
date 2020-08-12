using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace sandc_paint_pfs_app.App_Code
{
    public class GridClassHangScreen
    {
        [JsonProperty("last_name")]
        public string last_name;
        [JsonProperty("job_order_no")]
        public string job_order_no;
        [JsonProperty("nmr_no")]
        public string nmr_no;
        [JsonProperty("sequence_no")]
        public string sequence_no;
        [JsonProperty("part_no")]
        public string part_no;
        [JsonProperty("value_stream_id")]
        public string value_stream_id;
        [JsonProperty("paint_color")]
        public string paint_color;
        [JsonProperty("qty_hung")]
        public string qty_hung;
        [JsonProperty("dt_hang")]
        public string dt_hang;

    }
    public class GridHangScreenData
    {
        public static GridClassHangScreen[] GetHangScreenData(DataSet ds)
        {
            ArrayList rows = new ArrayList();
            try
            {
                if (ds.Tables["table_results"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["table_results"].Rows)
                    {
                        DateTime dt_hang = Convert.ToDateTime(row["dt_hang"].ToString());
                        string dt_hang_time = dt_hang.ToString("HH:mm:ss");
                        string start_time = "04:00:00";
                        string end_time = "16:00:00";
                        GridClassHangScreen obj = new GridClassHangScreen();
                        string details_url = "tracker_takedown_screen_iframe.aspx?type=report_recent_painted&auto_id=" + row["auto_id"].ToString();
                        /*if (Cookies_Helper.GetEmployeeLoggedInIsSupervisor().ToLower() == "yes")
                        {
                            obj.edit_item = "<span onclick = \"open_paint_tracker_modal( '" + details_url + "','" + row["auto_id"].ToString() + "')\"><i class=\"fa fa-pencil\"></i></span>";
                        }
                        else
                        {
                            obj.edit_item = "";
                        }*/
                        obj.last_name = row["last_name"].ToString();
                        obj.job_order_no = row["job_order_no"].ToString();
                        obj.nmr_no = row["nmr_no"].ToString();
                        obj.sequence_no = row["sequence_no"].ToString();
                        obj.part_no = row["part_no"].ToString();
                        obj.value_stream_id = row["value_stream"].ToString();
                        obj.paint_color = row["paint_color"].ToString();
                        obj.qty_hung = row["qty_hung"].ToString();
                        obj.dt_hang = Convert.ToDateTime(row["dt_hang"].ToString()).ToString("MM/dd/yyyy hh:mm tt");
                        /*if (DateTime.Parse(dt_hang_time) >= DateTime.Parse(start_time) && DateTime.Parse(dt_hang_time) <= DateTime.Parse(end_time))

                        {
                            obj.shift = "1";
                        }
                        else
                        {
                            obj.shift = "2";
                        }*/
                        rows.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            GridClassHangScreen[] arrReturn = (GridClassHangScreen[])rows.ToArray(typeof(GridClassHangScreen));
            return arrReturn;
        }
    }
}