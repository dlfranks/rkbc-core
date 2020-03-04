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
        public int bannerId { get; set; }
        public Attachment banner { get; set; }
        public IFormFile bannerImage { get; set; }
        public string title { get; set; }
        public string titleContent { get; set; }
        public string churchAnnounceTitle { get; set; }
        public string memberAnnounceTitle { get; set; }
        public string schoolAnnounceTitle { get; set; }
        public virtual List<ContentItem> churchAnnouncements { get; set; }
        public virtual List<ContentItem> memberAnnouncements { get; set; }
        public virtual List<ContentItem> schoolAnnouncements { get; set; }
        public virtual List<VideoAttachment> sundayServiceVideos { get; set; }
        public virtual List<Attachment> homephotoGallery { get; set; }
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
        protected void acceptPost()
        {

        }
        protected HomePageViewModel setupViewModel(HomePage model, FormViewMode mode)
        {
            HomePageViewModel vm = new HomePageViewModel() {
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
            var model = unitOfWork.homePages.get().FirstOrDefault();
            if (model == null) return RedirectToAction("Error");
            var vm = setupViewModel(model, FormViewMode.Edit);
            return View(vm);
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
