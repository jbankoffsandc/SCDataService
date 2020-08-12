using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataService.Models.DatabaseModels
{
    public class HangJob
    {
        public int hangjob_id { get; set; }

        public string lastname { get; set; }

        public string jobordernumber { get; set; }
        // [DefaultValue("Click here")]
        //  [Ignore]
        //  public string hyperlink { get; set; } = "Click here";
        public string nmr_number { get; set; }

        public string sequence_number { get; set; }

        public string part_number { get; set; }
        public string value_stream { get; set; }

        public string paint_color { get; set; }

        public string qty_hung { get; set; }
        public string datetime_hung { get; set; }

    }
}
