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
    
    public class HomeAttachment:IEntity
    {
        [Key]
        public int id { get; set; }
        public int homePageId { get; set; }
        public HomePage homePage { get; set; }
        public string sectionId { get; set; }
        public string fileName { get; set; }
        public string caption { get; set; }
        public bool isPdf { get; set; }
        public bool isOn { get; set; }
    }
    public class HomeBannerAttachment : IEntity
    {
        [Key]
        public int id { get; set; }
        public int homePageId { get; set; }
        public HomePage homePage { get; set; }
        public string sectionId { get; set; }
        public string fileName { get; set; }
        public string caption { get; set; }
        public bool isOn { get; set; }
    }
    public class HomeVideoAttachment:IEntity
    {
        [Key]
        public int id { get; set; }
        public int homePageId { get; set; }
        public HomePage homePage { get; set; }
        public string sectionId { get; set; }
        public string url { get; set; }
        public string caption { get; set; }
        public bool isOn { get; set; }
    }
    
    public class HomeItem :IEntity
    {
        [Key]
        public int id { get; set; }
        public int homePageId { get; set; }
        public HomePage homePage { get; set; }
        public int sectionId { get; set; }
        public string content { get; set; }
        public bool isOn { get; set; }
    }
    public class Page : IPage, IEntity
    {
        [Key]
        public int id { get; set; }
        public int siteId { get; set; }
        public PageEnum pageId { get; set; }
         
        
    }
    public class Site :IEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public string domain { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
    }
    public class HomePage :Page
    {
        public int bannerId { get; set; }
        public HomeBannerAttachment banner { get; set; }
        public string title { get; set; }
        public string titleContent { get; set; }
        public string churchAnnounceTitle { get; set; }
        public string memberAnnounceTitle { get; set; }
        public string schoolAnnounceTitle { get; set; }
        public virtual List<HomeItem> announcements { get; set; }
        public virtual List<HomeVideoAttachment> sundayServiceVideos { get; set; }
        public virtual List<HomeAttachment> homephotoGallery { get; set; }
    }
}
