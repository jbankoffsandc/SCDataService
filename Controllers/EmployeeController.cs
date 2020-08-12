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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataService.Controllers
{

    public class EmployeeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public EmployeeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }




        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection con = new MySqlConnection("Server = localhost; Port = 3306; Database = sandc_paint_pfs; User = jbankoff; Password = Ns2020!!; Convert Zero Datetime = True; "))
            {

                string query = "SELECT employee_no FROM employees_hidden WHERE (employee_no = '" + id.ToString() + "')";
                MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                ds.Clear();
                da.Fill(dt);
            }
            return Json(new
            {
                data = dt

            });
        }                                     
        
        // POST api/<EmployeeController>
        [HttpPost]
        public void Post([FromBody] Employee employee)
        {
            MySqlCommand cmd;
            using (MySqlConnection con = new MySqlConnection("Server = localhost; Port = 3306; Database = sandc_paint_pfs; User = jbankoff; Password = Ns2020!!; Convert Zero Datetime = True; "))
            {

                string query = "INSERT INTO employees_hidden (employee_no, employee_password, first_name, middle_name, last_name, employee_email, is_supervisor) VALUES ('" + employee.employee_no + "', '" + employee.employee_password + "', '" + employee.employee_first_name + "', '" + employee.employee_middle_name + "', '" + employee.employee_last_name + "', '" + employee.employee_email + "', '" + employee.is_supervisor + "')";

                MySqlDataAdapter da = new MySqlDataAdapter();
                cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();
            }
            // return some response 
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Employee employee)
        {
            string query = "UPDATE employees_hidden SET first_name = '" + employee.employee_first_name + "middle_name = '" + employee.employee_middle_name + "', last_name = '" + employee.employee_last_name + "', employee_email = '" + employee.employee_email + "', is_supervisor = '" + employee.is_supervisor + "' WHERE (employee_no = '" + id + "');";


        }

        // PATCH api/<EmployeeController>/5
        [HttpPatch("{id}")]
        public void Patch(int id, [FromBody] string password)
        {
            
            string query = "UPDATE employees_hidden SET employee_password = '" + password + "' WHERE (employee_no = '" + id + "');";

        }


        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            string query = "DELETE FROM employees_hidden WHERE (employee_no = '" + id + "');";

        }
    }
}
