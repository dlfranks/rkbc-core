using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using rkbc.core.repository;

namespace rkbc.core.models
{
    public enum AttachmentSectionEnum
    {
        [Display(Description = "Home Gallery")]
        Home_Gallery = 200,
        [Display(Description = "Pastor Gallery")]
        Pastor_Gallery,
    }
    public class Attachment : IEntity, IAuditStamp
    {
        public int id { get; set; }
        public int pageEnum { get; set; }
        public int attachmentSectionEnum { get; set; }
        public DateTime? createDt { get; set; }
        public string createUser { get; set; }
        public DateTime? lastUpdDt { get; set; }
        public string lastUpdUser { get; set; }
        public string fileName { get; set; }
        public string originalFileName { get; set; }
        public string caption { get; set; }
        public bool isOn { get; set; }
    }
    
}
