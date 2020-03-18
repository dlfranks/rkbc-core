using rkbc.core.repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.models
{
    public enum PageEnum
    {
        Home = 1,
        Column,
        Service,
        Korean_School,
        Contact

    }
    public enum SectionEnum
    {
        Banner = 100,
        Church_Announce,
        Member_Announce,
        School_Announce,
        Sermon_Video,
        Home_Gallery,

    }
    public class Section : IEntity, ISection
    {
        [Key]
        public int id { get; set; }
        public int homePageId { get; set; }
        public HomePage homePage {get; set;}
        public int sectionId { get; set; }
    }
    public class HomeAttachment : Section, IAuditStamp
    {
        public DateTime? createDt { get; set; }
        public string createUser { get; set; }
        public DateTime? lastUpdDt { get; set; }
        public string lastUpdUser { get; set; }
        public string fileName { get; set; }
        public string originalFileName { get; set; }
        public string caption { get; set; }
        public bool isOn { get; set; }
    }
    public class HomeVideoAttachment: Section
    {
        public string url { get; set; }
        public string caption { get; set; }
        public bool isOn { get; set; }
    }
    
    public class HomeContentItem : Section
    {

        public string content { get; set; }
        public bool isOn { get; set; }
    }
    
    
    public class HomePage :IEntity, IAuditStamp
    {
        public HomePage()
        {
            announcements = new List<HomeContentItem>();
           
        }
        public int id { get; set; }
        public DateTime? createDt { get; set; }
        public string createUser { get; set; }
        public DateTime? lastUpdDt { get; set; }
        public string lastUpdUser { get; set; }
        public string bannerUrl { get; set; }
        public string bannerFileName { get; set; }
        public string originalFileName { get; set; }
        public string title { get; set; }
        public string titleContent { get; set; }
        public string churchAnnounceTitle { get; set; }
        public string memberAnnounceTitle { get; set; }
        public string schoolAnnounceTitle { get; set; }
        public string sundayServiceVideoUrl { get; set; }
        public virtual List<HomeContentItem> announcements { get; set; }
        
    }
}
