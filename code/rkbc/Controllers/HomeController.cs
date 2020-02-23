using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.web.viewmodels;
using RKBC.Models;

namespace rkbc.web.viewmodels
{
    public class HomePageViewModel
    {
        public int id { get; set; }
        public int bannerId { get; set; }
        public HomeBannerAttachment banner { get; set; }
        public string title { get; set; }
        public string titleContent { get; set; }
        public string churchAnnounceTitle { get; set; }
        public string memberAnnounceTitle { get; set; }
        public string schoolAnnounceTitle { get; set; }
        public virtual List<HomeItem> churchAnnouncements { get; set; }
        public virtual List<HomeItem> memberAnnouncements { get; set; }
        public virtual List<HomeItem> schoolAnnouncements { get; set; }
        public virtual List<HomeVideoAttachment> sundayServiceVideos { get; set; }
        public virtual List<HomeAttachment> homephotoGallery { get; set; }
    }
}
namespace rkbc.web.Controllers
{
    public class HomeController : Controller
    {
        public IUnitOfWork unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        protected async Task acceptPost(HomePage modelObj)
        {
            HomePageViewModel model = new HomePageViewModel();
            TryUpdateModelAsync(model);
        }
        protected HomePageViewModel setupViewModel(HomePage model, FormViewMode mode)
        {
            HomePageViewModel vm = new HomePageViewModel() {
                id = model.id,
                bannerId = model.bannerId,
                banner = model.banner,
                title = model.title,
                titleContent = model.titleContent,
                churchAnnounceTitle = model.churchAnnounceTitle,
                memberAnnounceTitle = model.memberAnnounceTitle,
                schoolAnnounceTitle = model.schoolAnnounceTitle,
                sundayServiceVideos = model.sundayServiceVideos,
                churchAnnouncements = model.announcements.Where(q => q.sectionId == (int)SectionEnum.Church_Announce).ToList(),
                memberAnnouncements = model.announcements.Where(q => q.sectionId == (int)SectionEnum.Member_Announce).ToList(),
                schoolAnnouncements = model.announcements.Where(q => q.sectionId == (int)SectionEnum.School_Announce).ToList(),
                homephotoGallery = model.homephotoGallery
            };
            ViewBag.formViewMode = mode;
            return vm;
        }
        public IActionResult Edit(int? id)
        {
            if (id == null) return RedirectToAction("Error");
            //var model = unitOfWork.homePages.get().Where(q => q.siteId == id).FirstOrDefault();
            var vm = new HomePageViewModel()
            {
                title = "Vision 2020",
                titleContent = "2천명의 성도 - 2백명의 목자 - 20명의 선교사 파송",
                churchAnnounceTitle = "신년 금식성회",
                churchAnnouncements = new List<HomeItem>()
                {
                    new HomeItem(){ id=1, homePageId = 1, sectionId =101, isOn = true, content = "여호수아 14:6-12"},
                    new HomeItem(){ id=2, homePageId = 1, sectionId =101, isOn = true, content = "이 산지를 내게 주소서"},
                    new HomeItem(){ id=3, homePageId = 1, sectionId =101, isOn = true, content = "2020년 1월2일(목)-4릴(토)"}
                },
            };
            //if (model == null) return RedirectToAction("Error");
            //var vm = setupViewModel(model, FormViewMode.Edit);
            return View(vm);
        }
        
        public IActionResult ItemRow(int? sectionId)
        {
            if (sectionId == null) 
                throw new InvalidOperationException("Section Id is required when adding home items");
            else
            {
                HomeItem item = new HomeItem();
                item.homePageId = (int)PageEnum.Home;
                item.sectionId = sectionId?? (int)SectionEnum.Church_Announce;
                return PartialView("_Item", item);
            }
            
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public void setup()
        {

        }
    }
}
