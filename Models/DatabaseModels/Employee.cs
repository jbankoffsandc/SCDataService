using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataService.Models.DatabaseModels
{
    public class Employee
    {
        public string employee_no { get; set; }

        public string employee_password { get; set; }
       
        public string employee_first_name { get; set; }

        public string employee_middle_name { get; set; }
        public string employee_last_name { get; set; }

        public string employee_email { get; set; }
        public bool is_supervisor{ get; set; }
    }
}
