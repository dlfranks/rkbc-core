using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.models
{
    public enum pageEnum
    {
        Home = 1,
        column,
        Service,
        Korean_School,
        Contact

    }
    public class PdfAttachment
    {
        public int id { get; set; }
        public int pageId { get; set; }
        public string sectionId { get; set; }
        public string fileName { get; set; }
        public string caption { get; set; }
    }
    public class ImageAttachment
    {
        public int id { get; set; }
        public int pageId { get; set; }
        public string sectionId { get; set; }
        public string fileName { get; set; }
        public string caption { get; set; }
    }
    public class VideoAttachment
    {
        public int id { get; set; }
        public int pageId { get; set; }
        public string sectionId { get; set; }
        public string url { get; set; }
        public string caption { get; set; }
    }
    public class ItemSet
    {
        public int id { get; set; }
        public int pageId { get; set; }
        public string title { get; set; }
        public virtual List<Item> itemList {get; set;}
    }
    public class Item
    {
        public int id { get; set; }
        public int itemSetId { get; set; }
        public string content { get; set; }
        
    }
    public class ServiceItemSet
    {
        public int id { get; set; }
        public int pageId { get; set; }
        public string title { get; set; }
        public virtual List<ServiceItem> serviceItemList { get; set; }
    }
    public class ServiceItem
    {
        public int id { get; set; }
        public int ServiceItemSetId { get; set; }
        public string content { get; set; }
    }
    public class SchoolItemSet
    {
        public int id { get; set; }
        public int pageId { get; set; }
        public string title { get; set; }
        public virtual List<SchoolItem> schoolItemList { get; set; }
    }
    public class SchoolItem
    {
        public int id { get; set; }
        public int SchoolItemSetId { get; set; }
        public string content { get; set; }
        
    }
    public class homeModel
    {
        public int id { get; set; }
        public ImageAttachment bannerImageFileName { get; set; }
        public string title { get; set; }
        public string titleContent { get; set; }
        public virtual ImageAttachment imageAttachment { get; set; }
        public virtual VideoAttachment sundayServiceVideo { get; set; }
        public virtual List<ItemSet> dashBoard { get; set; }
        public virtual List<ServiceItemSet> serviceInfo { get; set; }
        public virtual List<SchoolItemSet> schoolInfo { get; set; }
        public virtual List<ImageAttachment> photos { get; set; }
    }
}
