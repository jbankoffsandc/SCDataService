using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataService.Models.DatabaseModels
{
    public static class DB
    {
        public static string connection_string { get; set; } = "Server=localhost;Port=3306;Database=sandc_paint_pfs;User=jbankoff;Password=Ns2020!!; Convert Zero Datetime=true";
        public static string mildew_database_connection_string { get; set; } = "Server=localhost;Port=3306;Database=sandc_mildew;User=sandc_db;Password=Sandc@123; Convert Zero Datetime=true";
    }
}

