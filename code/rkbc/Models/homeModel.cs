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
    public class Page : IEntity, IPage, ISection, IAuditStamp
    {
        [Key]
        public int id { get; set; }
        public int sectionId { get; set; }
        public int pageId { get; set; }
        public DateTime? createDt { get; set; }
        public string createUser { get; set; }
        public DateTime? lastUpdDt { get; set; }
        public string lastUpdUser { get; set; }
    }
    public class Attachment : Page
    {
        public string fileName { get; set; }
        public string originalFileName { get; set; }
        public string caption { get; set; }
        public bool isOn { get; set; }
    }
    public class VideoAttachment:Page
    {
        public string url { get; set; }
        public string caption { get; set; }
        public bool isOn { get; set; }
    }
    
    public class ContentItem :Page
    {
        public string content { get; set; }
        public bool isOn { get; set; }
    }
    
    
    public class HomePage :Page
    {
        public int bannerId { get; set; }
        public Attachment banner { get; set; }
        public string title { get; set; }
        public string titleContent { get; set; }
        public string churchAnnounceTitle { get; set; }
        public string memberAnnounceTitle { get; set; }
        public string schoolAnnounceTitle { get; set; }
        public virtual List<ContentItem> announcements { get; set; }
        public virtual List<VideoAttachment> sundayServiceVideos { get; set; }
        public virtual List<Attachment> homephotoGallery { get; set; }
    }
}
