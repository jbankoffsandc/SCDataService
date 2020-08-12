using System;
using System.Data;
using DataService.Models.DatabaseModels;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace DataService.Extensions
{
    /// <summary>
    /// Summary description for App_Helper
    /// </summary>
    public static class App_Helper
    {
        //----------------------------------------------------------------------------------------------------------------------------------------
        //***********************************/
        //		FUNCTIONS
        //***********************************/
        static public string GetAppName()
        {
            return "PFS Paint";
        }

        static public string GetPaintCode()
        {
            return "PNT";
        }

        static public string ReturnValueStreamIDForPartNo(string inPartNo)
        {
            string retFlag = "";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB.mildew_database_connection_string))
                {
                    //open connection to database
                    conn.Open();

                    string query = "SELECT value_stream_id FROM master_part_details WHERE (part_no = '" + NJS_Helper.FormatStringforDB(inPartNo) + "');";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        if (obj.ToString() != "")
                        {
                            retFlag = obj.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retFlag = "";
            }
            return retFlag;
        }

        static public string ReturnValueStreamNameForPartNo(string inPartNo)
        {
            string retFlag = "";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB.mildew_database_connection_string))
                {
                    //open connection to database
                    conn.Open();

                    string query = "";
                    query += "SELECT v.value_stream ";
                    query += "FROM master_part_details m ";
                    query += "INNER JOIN master_value_stream v ON m.value_stream_id = v.value_stream_id ";
                    query += "WHERE(m.part_no = '" + NJS_Helper.FormatStringforDB(inPartNo) + "')'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        if (obj.ToString() != "")
                        {
                            retFlag = obj.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retFlag = "";
            }
            return retFlag;
        }

        static public string ReturnMiLDEWVSNameForVSID(string inVSID)
        {
            if (inVSID == "")
                return "";

            string retFlag = "";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB.mildew_database_connection_string))
                {
                    //open connection to database
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.Open();

                    string query = "";
                    query += "SELECT value_stream ";
                    query += "FROM master_value_stream ";
                    query += "WHERE(value_stream_id = '" + NJS_Helper.FormatStringforDB(inVSID) + "');";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        if (obj.ToString() != "")
                        {
                            retFlag = obj.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retFlag = "";
            }
            return retFlag;
        }

        static public string InsertIntoPartDetails(string inPartNo, string inValueStreamID)
        {
            string retFlag = "";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB.mildew_database_connection_string))
                {
                    //open connection to database
                    conn.Open();

                    string query = "INSERT INTO master_part_details (part_no, value_stream_id) VALUES ";
                    query += "(";
                    query += "'" + NJS_Helper.FormatStringforDB(inPartNo) + "', ";
                    query += "'" + NJS_Helper.FormatStringforDB(inValueStreamID) + "'";
                    query += ");";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 1)
                        retFlag = "ok";
                }
            }
            catch (Exception ex)
            {
                retFlag = "";
            }
            return retFlag;
        }

        static public void InsertIntoMildew(string paint_auto_id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB.connection_string))
                {
                    using (MySqlConnection mildew_conn = new MySqlConnection(DB.mildew_database_connection_string))
                    {
                        conn.Open();
                        mildew_conn.Open();

                        string query = "";
                        MySqlCommand cmd = null;

                        string mildew_auto_id = "";
                        query = "SELECT auto_id FROM mildew_tracker WHERE (from_app = '" + App_Helper.GetAppName() + "' AND from_app_auto_id = " + paint_auto_id + ");";
                        cmd = new MySqlCommand(query, mildew_conn);
                        object obj1 = cmd.ExecuteScalar();
                        if (obj1 != null)
                        {
                            if (obj1.ToString() != "")
                            {
                                mildew_auto_id = obj1.ToString();
                            }
                        }

                        query = "SELECT * FROM paint_tracker WHERE (auto_id = " + paint_auto_id + ");";
                        MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                        DataSet ds = new DataSet();
                        ds.Clear();
                        da.Fill(ds, "results");

                        foreach (DataRow row in ds.Tables["results"].Rows)
                        {
                            string value_stream_id = row["value_stream_id"].ToString();
                            string emp_no_takedown = row["emp_no_takedown"].ToString();
                            string dt_takedown = row["dt_takedown"].ToString();
                            string part_no = row["part_no"].ToString();
                            string qty_rejected = row["qty_rejected"].ToString();
                            string job_order_no = row["job_order_no"].ToString();
                            string eco_nmr_psr_di_no = row["nmr_no"].ToString();
                            string sequence_no = row["sequence_no"].ToString();
                            string color = row["paint_color"].ToString();

                            string reject_code = row["rejection_code"].ToString();
                            string first_why_id = "";
                            if (reject_code != "")
                            {
                                string first_why_query = "SELECT * FROM master_paint_reject_first_why WHERE (reject_code = " + reject_code + ");";

                                MySqlDataAdapter first_why_da = new MySqlDataAdapter(first_why_query, mildew_conn);
                                DataSet first_why_ds = new DataSet();
                                first_why_ds.Clear();
                                first_why_da.Fill(first_why_ds, "first_why_results");

                                foreach (DataRow first_why_row in first_why_ds.Tables["first_why_results"].Rows)
                                {
                                    first_why_id = first_why_row["first_why_id"].ToString();
                                }
                            }

                            string direct_cause_detail = row["rejection_notes"].ToString();
                            string direct_cause_id = "";
                            if (first_why_id != "")
                            {
                                string direct_cause_id_query = "SELECT direct_cause_id FROM master_first_whys WHERE (first_why_id = " + first_why_id + ");";
                                MySqlDataAdapter direct_cause_da = new MySqlDataAdapter(direct_cause_id_query, mildew_conn);
                                DataSet direct_cause_ds = new DataSet();
                                direct_cause_ds.Clear();
                                direct_cause_da.Fill(direct_cause_ds, "direct_cause_results");
                                foreach (DataRow direct_cause_row in direct_cause_ds.Tables["direct_cause_results"].Rows)
                                {
                                    direct_cause_id = direct_cause_row["direct_cause_id"].ToString();
                                }
                            }
                            string gFrom_app_auto_id = row["auto_id"].ToString();

                            if (emp_no_takedown == "")
                            {
                                emp_no_takedown = "0";
                            }

                            if (part_no == "")
                            {
                                part_no = "null";
                            }

                            if (job_order_no == "")
                            {
                                job_order_no = "null";
                            }

                            if (eco_nmr_psr_di_no == "")
                            {
                                eco_nmr_psr_di_no = "null";
                            }

                            if (sequence_no == "")
                            {
                                sequence_no = "null";
                            }

                            if (color == "")
                            {
                                color = "null";
                            }

                            if (first_why_id == "")
                            {
                                first_why_id = "0";
                            }

                            if (direct_cause_detail == "")
                            {
                                direct_cause_detail = "null";
                            }

                            string mildew_query = "";
                            if (mildew_auto_id == "")
                            {
                                mildew_query = "INSERT INTO mildew_tracker (value_stream_id, requester_id, requested_date_time, part_no, ";
                                mildew_query += "quantity, job_order_no, eco_nmr_psr_di_no, sequence_no, color, direct_cause_id, first_why_id, direct_cause_detail, ";
                                mildew_query += "supplier_id, location, owner_id, promise_date_time, actual_date_time, root_cause_department_id, root_cause_team_id, ";
                                mildew_query += "closed_by_id, last_updated_date_time, from_app, from_app_auto_id) ";
                                mildew_query += " VALUES ";
                                mildew_query += " ( ";
                                mildew_query += "" + NJS_Helper.FormatStringforDB(value_stream_id) + ", ";
                                mildew_query += "" + NJS_Helper.FormatStringforDB(emp_no_takedown) + ", ";
                                mildew_query += "'" + Convert.ToDateTime(dt_takedown).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                                mildew_query += "'" + NJS_Helper.FormatStringforDB(part_no) + "', ";
                                mildew_query += "" + NJS_Helper.FormatStringforDB(qty_rejected) + ", ";
                                mildew_query += "'" + NJS_Helper.FormatStringforDB(job_order_no) + "', ";
                                if (eco_nmr_psr_di_no == "null")
                                {
                                    mildew_query += "'" + NJS_Helper.FormatStringforDB(eco_nmr_psr_di_no) + "', ";
                                }
                                else
                                {
                                    mildew_query += "'NMR (" + NJS_Helper.FormatStringforDB(eco_nmr_psr_di_no) + ")', ";
                                }
                                mildew_query += "'" + NJS_Helper.FormatStringforDB(sequence_no) + "', ";
                                mildew_query += "'" + NJS_Helper.FormatStringforDB(color) + "', ";
                                mildew_query += "" + NJS_Helper.FormatStringforDB(direct_cause_id) + ", ";
                                mildew_query += "" + NJS_Helper.FormatStringforDB(first_why_id) + ", ";
                                mildew_query += "'" + NJS_Helper.FormatStringforDB(direct_cause_detail) + "', ";
                                mildew_query += "1, ";
                                mildew_query += "'4326', ";
                                mildew_query += "1, ";
                                mildew_query += "'" + Convert.ToDateTime(dt_takedown).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                                mildew_query += "'" + Convert.ToDateTime(dt_takedown).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                                mildew_query += "1, ";
                                mildew_query += "1, ";
                                mildew_query += "" + NJS_Helper.FormatStringforDB(emp_no_takedown) + ", ";
                                mildew_query += "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                                mildew_query += "'" + GetAppName() + "', ";
                                mildew_query += "" + NJS_Helper.FormatStringforDB(gFrom_app_auto_id) + " ";
                                mildew_query += ")";
                            }
                            else if (mildew_auto_id != "")
                            {
                                mildew_query = "UPDATE mildew_tracker SET ";
                                mildew_query += "value_stream_id = " + NJS_Helper.FormatStringforDB(value_stream_id) + ", ";
                                mildew_query += "requester_id = " + NJS_Helper.FormatStringforDB(emp_no_takedown) + ", ";
                                mildew_query += "requested_date_time = '" + Convert.ToDateTime(dt_takedown).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                                mildew_query += "part_no = '" + NJS_Helper.FormatStringforDB(part_no) + "', ";
                                mildew_query += "quantity = " + NJS_Helper.FormatStringforDB(qty_rejected) + ", ";
                                mildew_query += "job_order_no = '" + NJS_Helper.FormatStringforDB(job_order_no) + "', ";
                                if (eco_nmr_psr_di_no == "null")
                                {
                                    mildew_query += "eco_nmr_psr_di_no = '" + NJS_Helper.FormatStringforDB(eco_nmr_psr_di_no) + "', ";
                                }
                                else
                                {
                                    mildew_query += "eco_nmr_psr_di_no = 'NMR (" + NJS_Helper.FormatStringforDB(eco_nmr_psr_di_no) + ")', ";
                                }
                                mildew_query += "sequence_no = '" + NJS_Helper.FormatStringforDB(sequence_no) + "', ";
                                mildew_query += "color = '" + NJS_Helper.FormatStringforDB(color) + "', ";
                                mildew_query += "direct_cause_id = " + NJS_Helper.FormatStringforDB(direct_cause_id) + ", ";
                                mildew_query += "first_why_id = " + NJS_Helper.FormatStringforDB(first_why_id) + ", ";
                                mildew_query += "direct_cause_detail = '" + NJS_Helper.FormatStringforDB(direct_cause_detail) + "', ";
                                mildew_query += "supplier_id = 1, ";
                                mildew_query += "location = '4327', ";
                                mildew_query += "owner_id = 1, ";
                                mildew_query += "promise_date_time = '" + Convert.ToDateTime(dt_takedown).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                                mildew_query += "actual_date_time = '" + Convert.ToDateTime(dt_takedown).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                                mildew_query += "root_cause_department_id = 1, ";
                                mildew_query += "root_cause_team_id= 1, ";
                                mildew_query += "closed_by_id = " + NJS_Helper.FormatStringforDB(emp_no_takedown) + ", ";
                                mildew_query += "last_updated_date_time = '" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                                mildew_query += "from_app = '" + GetAppName() + "', ";
                                mildew_query += "from_app_auto_id = " + NJS_Helper.FormatStringforDB(gFrom_app_auto_id) + " ";
                                mildew_query += "WHERE (auto_id = " + mildew_auto_id + ");";
                            }

                            MySqlCommand mildew_cmd = new MySqlCommand(mildew_query, mildew_conn);
                            int rows = mildew_cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //this.error.InnerHtml = ex.Message;
            }
        }

        static public string ReturnValueStreamLetter(string id)
        {
            string ret_str = "";

            //if (id == "1")
            //    ret_str = "E";
            //else if (id == "2")
            //    ret_str = "H";
            if (id == "3")
                ret_str = "C";
            else if (id == "4")
                ret_str = "V";
            else if (id == "5")
                ret_str = "L";
            else if (id == "6")
                ret_str = "P";
            //else if (id == "7")
            //    ret_str = "W";
            else if (id == "8")
                ret_str = "D";
            else if (id == "9")
                ret_str = "S";
            else if (id == "10")
                ret_str = "O";
            else if (id == "11")
                ret_str = "U";
            else if (id == "12")
                ret_str = "I";
            else if (id == "13")
                ret_str = "T";
            else if (id == "14")
                ret_str = "W";
            else
                ret_str = "-";

            return ret_str;
        }

        static public string ReturnValueStreamID(string letter)
        {
            string ret_str = "";

            letter = letter.ToUpper();

            if (letter == "E")
                ret_str = "1";
            else if (letter == "H")
                ret_str = "2";
            else if (letter == "C")
                ret_str = "3";
            else if (letter == "V")
                ret_str = "4";
            else if (letter == "L")
                ret_str = "5";
            else if (letter == "P")
                ret_str = "6";
            //else if (letter == "W")
            //    ret_str = "7";
            else if (letter == "D")
                ret_str = "8";
            else if (letter == "S")
                ret_str = "9";
            else if (letter == "O")
                ret_str = "10";
            else if (letter == "U")
                ret_str = "11";
            else if (letter == "I")
                ret_str = "12";
            else if (letter == "T")
                ret_str = "13";
            else if (letter == "W")
                ret_str = "14";

            return ret_str;
        }

        public static string ConfigureProductionGoals(string date)
        {
            string retStr = "ok";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB.connection_string))
                {
                    // open connection to database
                    conn.Open();

                    string query = "";
                    int no_of_days_difference = 0;
                    MySqlDataAdapter da = null;
                    DataSet ds = null;
                    MySqlCommand cmd = null;

                    string dt = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
                    DateTime selected_date = Convert.ToDateTime(dt);

                    // check if rows added for the date selected
                    query = "SELECT * FROM admin_heijuhka_setup WHERE (DATE(shift_start_dt) = '" + dt + "'); ";
                    da = new MySqlDataAdapter(query, conn);
                    ds = new DataSet();
                    ds.Clear();
                    da.Fill(ds, "results");
                    if (ds.Tables["results"].Rows.Count == 0)
                    {
                        //get last data
                        query = "SELECT DISTINCT(CAST(shift_start_dt as DATE)) AS date FROM admin_heijuhka_setup ORDER BY shift_start_dt DESC LIMIT 1";
                        da = new MySqlDataAdapter(query, conn);
                        ds = new DataSet();
                        ds.Clear();
                        da.Fill(ds, "results_row");

                        if (ds.Tables["results_row"].Rows.Count > 0)
                        {
                            string last_date = "";
                            foreach (DataRow row in ds.Tables["results_row"].Rows)
                            {
                                last_date = Convert.ToDateTime(row["date"]).ToString("yyyy-MM-dd");
                                DateTime date1 = Convert.ToDateTime(last_date);
                                TimeSpan difference = selected_date - date1;
                                var days = difference.TotalDays;
                                no_of_days_difference = Convert.ToInt32(days);

                                //last entered data values in database
                                string query1 = "SELECT * FROM  admin_heijuhka_setup WHERE (DATE(shift_start_dt) = '" + last_date + "')";
                                da = new MySqlDataAdapter(query1, conn);
                                ds = new DataSet();
                                ds.Clear();
                                da.Fill(ds, "lastdate_results");

                                if (ds.Tables["lastdate_results"].Rows.Count > 0)
                                {
                                    foreach (DataRow row1 in ds.Tables["lastdate_results"].Rows)
                                    {
                                        string shift = row1["shift"].ToString();
                                        string value_stream_id = row1["value_stream"].ToString();
                                        DateTime dt1 = new DateTime();
                                        DateTime shift_start = new DateTime();
                                        DateTime shift_end = new DateTime();

                                        dt1 = Convert.ToDateTime(row1["datetime"].ToString()).AddDays(no_of_days_difference);
                                        shift_start = Convert.ToDateTime(row1["shift_start_dt"].ToString()).AddDays(no_of_days_difference);
                                        shift_end = Convert.ToDateTime(row1["shift_end_dt"].ToString()).AddDays(no_of_days_difference);

                                        //else if (selected_date < date1)
                                        //{
                                        //    dt1 = Convert.ToDateTime(row1["datetime"].ToString()).AddDays(-no_of_days_difference);
                                        //    shift_start = Convert.ToDateTime(row1["shift_start_dt"].ToString()).AddDays(-no_of_days_difference);
                                        //    shift_end = Convert.ToDateTime(row1["shift_end_dt"].ToString()).AddDays(-no_of_days_difference);
                                        //}
                                        query = "";
                                        query += "INSERT INTO admin_heijuhka_setup VALUES ";
                                        query += "(";
                                        query += "NULL, ";
                                        query += "'" + NJS_Helper.FormatStringforDB(shift) + "', ";
                                        query += "'" + NJS_Helper.FormatStringforDB(value_stream_id) + "', ";
                                        query += "'" + NJS_Helper.FormatStringforDB(dt1.ToString("yyyy-MM-dd HH:mm:ss")) + "', ";
                                        query += "'" + NJS_Helper.FormatStringforDB(shift_start.ToString("yyyy-MM-dd HH:mm:ss")) + "', ";
                                        query += "'" + NJS_Helper.FormatStringforDB(shift_end.ToString("yyyy-MM-dd HH:mm:ss")) + "' ";
                                        query += ") ";
                                        cmd = new MySqlCommand(query, conn);
                                        int rows = cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retStr = ex.Message;
            }
            return retStr;
        }

      /*  public static string PopulateProductionTrackerInB6App(string seq_no, string hang_emp_no)
        {
            string retStr = "ok";
            bool flag = false;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB.ProductionTrackerDatabaseConnString()))
                {
                    //open connection to database
                    conn.Open();

                    string vs_id = "";
                    string vs_letter = seq_no.Substring(0, 1);
                    vs_id = ReturnValueStreamID(vs_letter);

                    string query = "SELECT DISTINCT sequence_no FROM sequence_nos WHERE sequence_no='" + seq_no.ToUpper() + "'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        flag = true;
                    }
                    if (flag == true)
                    {
                        string employee_no = Cookies_Helper.GetEmployeeLoggedInNo();
                        DateTime dt = DateTime.Now;
                        query = "SELECT * FROM production_tracker WHERE sequence_no='" + seq_no.ToUpper() + "' AND work_center_id='12' ";
                        MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                        DataSet ds = new DataSet();
                        da.Fill(ds, "results");
                        if (ds.Tables["results"].Rows.Count == 0)
                        {
                            query = "INSERT INTO production_tracker (employee_no, value_stream_id, sequence_no, work_center_id, start_dt, hold_dt, skip_dt, ";
                            query += "stage_completed_by, stage_completed, is_out_of_sequence, is_out_of_sequence_reason_id) VALUES ";
                            query += "('" + NJS_Helper.FormatStringforDB(employee_no) + "', ";
                            query += "" + vs_id + ", ";
                            query += "'" + NJS_Helper.FormatStringforDB(seq_no.ToUpper()) + "', ";
                            query += "12, ";
                            query += "'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                            query += "NULL, ";
                            query += "NULL, ";
                            query += "" + hang_emp_no + ", ";
                            query += "'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                            query += "NULL, ";
                            query += "NULL ";
                            query += ");";

                            MySqlCommand ins_cmd = new MySqlCommand(query, conn);
                            ins_cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retStr = ex.Message;
            }
            return retStr;
        }*/

        public static string EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }

        public static string DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException fe)
            {
                decrypted = "";
            }
            return decrypted;
        }

        static public string ReturnEmployeeNamePFS(string id)
        {
            string Employee_name = "";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DB.connection_string))
                {
                    string query = "";

                    //open connection to database
                    conn.Open();

                    query = "SELECT last_name FROM employees_hidden WHERE (employee_no = '" + id + "');";
                    var cmd = new MySqlCommand(query, conn);
                    object obj = cmd.ExecuteScalar();

                    if (obj != null)
                    {
                        if (obj.ToString() != "")
                        {
                            Employee_name = obj.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Employee_name = ex.Message;
            }
            return Employee_name;
        }
        public static string PopulatePaintTracker(DataTable dt)
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

        /*static public string ReturnEmployeeNameCare(string id)
        {
            string Employee_name = "";
            string last_name = "";

            try
            {
                using (var conn = new MySqlConnection(DB.CareDatabaseConnectionString()))
                {
                    string query = "";

                    //open connection to database

                    conn.Open();

                    query = "SELECT emp_name FROM employee WHERE (emp_id = '" + id + "');";
                    var cmd = new MySqlCommand(query, conn);
                    object obj = cmd.ExecuteScalar();

                    if (obj != null)
                    {
                        if (obj.ToString() != "")
                        {
                            Employee_name = obj.ToString();
                            last_name = Employee_name.Split(',')[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Employee_name = ex.Message;
            }
            return last_name;
        }*/
        //----------------------------------------------------------------------------------------------------------------------------------------
    }
}
