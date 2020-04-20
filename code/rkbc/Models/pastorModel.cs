using rkbc.core.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.models
{
    public class PastorPage:IEntity, IAuditStamp
    {
        public int id { get; set; }
        public DateTime? createDt { get; set; }
        public string createUser { get; set; }
        public DateTime? lastUpdDt { get; set; }
        public string lastUpdUser { get; set; }
        public string pageTitle { get; set; }
        public string author { get; set; }
        public string columnTitle {get; set;}
        public string column { get; set; }
        
    }
}
