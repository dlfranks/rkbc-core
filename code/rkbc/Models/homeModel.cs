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
        Pastor,
        Mission,
        Korean_School,
        Contact

    }
    public enum HomeSectionEnum
    {
        [Display(Description = "Banner")]
        Banner = 100,
        [Display(Description = "Church Announce")]
        Church_Announce,
        [Display(Description = "Member Announce")]
        Member_Announce,
        [Display(Description = "Member Announce")]
        School_Announce,
        [Display(Description = "Sermon Video")]
        Sermon_Video,
        [Display(Description = "Home Gallery")]
        Home_Gallery,


    }
    public class HomeContentItem : IEntity
    {
        public int id { get; set; }
        public int homePageId { get; set; }
        public virtual HomePage homePage { get; set; }
        public int sectionId { get; set; }
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
    public class Contact:IEntity
    {
        public int id { get; set; }
        public DateTime? createDt { get; set; }
        public string createUser { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string subject { get; set; }
        public string message { get; set; }
    }
}
