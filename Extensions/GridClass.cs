using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


public class GridClass
{
    
    [JsonProperty("emp_no_hang")]
    public string emp_no_hang;
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
    [JsonProperty("emp_no_takedown")]
    public string emp_no_takedown;
    [JsonProperty("qty_good")]
    public string qty_good;
    [JsonProperty("qty_rejected")]
    public string qty_rejected;
    [JsonProperty("rejection_code")]
    public string rejection_code;
    [JsonProperty("rejection_notes")]
    public string rejection_notes;
    [JsonProperty("dt_takedown")]
    public string dt_takedown;
    [JsonProperty("emp_no_transaction")]
    public string emp_no_transaction;
    [JsonProperty("transaction_type_id")]
    public string transaction_type_id;
    [JsonProperty("qty_complete")]
    public string qty_complete;
    [JsonProperty("qty_moved")]
    public string qty_moved;
    [JsonProperty("dt_transaction")]
    public string dt_transaction;
    [JsonProperty("rework")]
    public string rework;
    [JsonProperty("week_ending")]
    public string week_ending;
    [JsonProperty("paint_non_paint")]
    public string paint_non_paint;
    [JsonProperty("work_station")]
    public string work_station;
    [JsonProperty("shift")]
    public string shift;
}
public class GridPaintTrackerData
{
    public static GridClass[] GetPaintTrackerData(DataTable dt)
    {
        ArrayList rows = new ArrayList();
        try
        {
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    DateTime dt_hang = Convert.ToDateTime(row["dt_hang"].ToString());
                    string dt_hang_time = dt_hang.ToString("HH:mm:ss");
                    string start_time = "04:00:00";
                    string end_time = "16:00:00";
                    GridClass obj = new GridClass();
                    string details_url = "tracker_takedown_screen_iframe.aspx?type=report_recent_painted&auto_id=" + row["auto_id"].ToString();
                    /*if (Cookies_Helper.GetEmployeeLoggedInIsSupervisor().ToLower() == "yes")
                    {
                        obj.edit_item = "<span onclick = \"open_paint_tracker_modal( '" + details_url + "','" + row["auto_id"].ToString() + "')\"><i class=\"fa fa-pencil\"></i></span>";
                    }
                    else
                    {
                        obj.edit_item = "";
                    }*/
                    obj.emp_no_hang = row["name_hang"].ToString();
                    obj.job_order_no = row["job_order_no"].ToString();
                    obj.nmr_no = row["nmr_no"].ToString();
                    obj.sequence_no = row["sequence_no"].ToString();
                    obj.part_no = row["part_no"].ToString();
                    obj.value_stream_id = row["value_stream"].ToString();
                    obj.paint_color = row["paint_color"].ToString();
                    obj.qty_hung = row["qty_hung"].ToString();
                    obj.dt_hang = Convert.ToDateTime(row["dt_hang"].ToString()).ToString("MM/dd/yyyy hh:mm tt");
                    if (DateTime.Parse(dt_hang_time) >= DateTime.Parse(start_time) && DateTime.Parse(dt_hang_time) <= DateTime.Parse(end_time))

                    {
                        obj.shift = "1";
                    }
                    else
                    {
                        obj.shift = "2";
                    }
                    obj.emp_no_takedown = row["name_takedown"].ToString();
                    obj.qty_good = row["qty_good"].ToString();
                    obj.qty_rejected = row["qty_rejected"].ToString();
                    obj.rejection_code = row["reject_text"].ToString();
                    obj.rejection_notes = row["rejection_notes"].ToString();
                    obj.dt_takedown = Convert.ToDateTime(row["dt_takedown"].ToString()).ToString("MM/dd/yyyy hh:mm tt");
                    obj.emp_no_transaction = row["emp_no_transaction"].ToString();
                    obj.transaction_type_id = row["transaction_type"].ToString();
                    obj.qty_complete = row["qty_complete"].ToString();
                    obj.qty_moved = row["qty_moved"].ToString();
                    obj.dt_transaction = row["dt_transaction"].ToString();
                    obj.rework = row["rework"].ToString();
                    obj.week_ending = row["week_ending"].ToString();
                    obj.paint_non_paint = row["paint_non_paint"].ToString();
                    obj.work_station = row["work_station"].ToString();
                    rows.Add(obj);
                }
            }
        }
        catch (Exception ex)
        {
            string err = ex.Message;
        }
        GridClass[] arrReturn = (GridClass[])rows.ToArray(typeof(GridClass));
        return arrReturn;
    }
}