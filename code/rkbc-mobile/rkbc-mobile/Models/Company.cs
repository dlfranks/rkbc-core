using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace rkbc.mobile.models
{
    class Company
    {
        [PrimaryKey]
        public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
    }


}
