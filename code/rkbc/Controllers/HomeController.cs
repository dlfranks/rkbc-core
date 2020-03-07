using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.web.viewmodels;
using RKBC.Models;
using ElmahCore;
using rkbc.core.services;
using rkbc.core.helper;

namespace rkbc.web.viewmodels
{
    public class HomePageViewModel
    {

        public string bannerUrl { get; set; }
        public string bannerFileName { get; set; }
        public IFormFile bannerImage { get; set; }
        public string title { get; set; }
        public string titleContent { get; set; }
        public string churchAnnounceTitle { get; set; }
        public string memberAnnounceTitle { get; set; }
        public string schoolAnnounceTitle { get; set; }
        public string sundayServiceVideoUrl { get; set;}
        public virtual List<HomeContentItem> churchAnnouncements { get; set; }
        public virtual List<HomeContentItem> memberAnnouncements { get; set; }
        public virtual List<HomeContentItem> schoolAnnouncements { get; set; }
        
    }
}
namespace rkbc.web.Controllers
{
    public class HomeController : Controller
    {
        public IUnitOfWork unitOfWork;
        public IMapper mapper;
        public FileHelper fileHelper;
        public HomeController(IUnitOfWork unitOfWork, IMapper _mapper, FileHelper _fileHelper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = _mapper;
            this.fileHelper = _fileHelper;

        }
        public IActionResult Index()
        {
            return View();
        }
        protected void acceptPost(HomePage modelObj)
        {
            HomePageViewModel model = new HomePageViewModel();
            TryUpdateModelAsync(model);
            if (modelObj == null) modelObj = new HomePage();
            mapper.Map<HomePage, HomePageViewModel>(modelObj, model);
            var announcements = new List<HomeContentItem>();
            //Video
            modelObj.sundayServiceVideoUrl = model.sundayServiceVideoUrl;
            

            //Banner image
            if(model.bannerImage != null)
            {
                var extension = fileHelper.getExtension(model.bannerImage.FileName);
                var fileName = fileHelper.getFileName(model.bannerImage.FileName);
                var assetFileName = fileHelper.newAssetFileName("banner", extension);
                var assetFileAndPathName = fileHelper.mapAssetPath("banner", assetFileName, false);
                Bitmap bitmap = null;
                try { 
                    bitmap = new Bitmap(model.bannerImage.OpenReadStream());
                }
                catch (Exception e)
                {
                    var msg = "Unable to read image format, please upload either .jpeg or .png images.";
                    ModelState.AddModelError("bannerImageUrl", msg);
                    //ElmahCore.XmlFileErrorLog.;
                }
                try
                {
                    Imaging.saveJpegImage(bitmap, assetFileAndPathName, 75L);
                    Imaging.GenerateThumbnail(bitmap, 150, assetFileAndPathName);
                    modelObj.bannerFileName = assetFileName;
                    modelObj.originalFileName = model.bannerImage.FileName;
                }
                catch (Exception e)
                {
                    var msg = "Internal error, unable to save the image.";
                    ModelState.AddModelError("bannerImageUrl", msg);
                    //ElmahCore
                }
            }
            

            foreach (var item in model.churchAnnouncements)
            {
                announcements.Add(new HomeContentItem()
                {
                    homePageId = (int)PageEnum.Home,
                    sectionId = (int)SectionEnum.Church_Announce,
                    content = item.content,
                    isOn = true
                });
            }
            foreach (var item in model.memberAnnouncements)
            {
                announcements.Add(new HomeContentItem()
                {
                    homePageId = (int)PageEnum.Home,
                    sectionId = (int)SectionEnum.Member_Announce,
                    content = item.content,
                    isOn = true
                });
            }
            foreach (var item in model.schoolAnnouncements)
            {
                announcements.Add(new HomeContentItem()
                {
                    homePageId = (int)PageEnum.Home,
                    sectionId = (int)SectionEnum.School_Announce,
                    content = item.content,
                    isOn = true
                });
            }

        }
        protected HomePageViewModel setupViewModel(HomePage model, FormViewMode mode)
        {
            HomePageViewModel vm = new HomePageViewModel() {

                bannerFileName = model.bannerFileName,
                title = model.title,
                titleContent = model.titleContent,
                bannerUrl = fileHelper.generateAssetURL("banner", model.bannerFileName),
                churchAnnounceTitle = model.churchAnnounceTitle,
                memberAnnounceTitle = model.memberAnnounceTitle,
                schoolAnnounceTitle = model.schoolAnnounceTitle,
                sundayServiceVideoUrl = model.sundayServiceVideoUrl,
                churchAnnouncements = model.announcements.Where(q => q.sectionId == (int)SectionEnum.Church_Announce).ToList(),
                memberAnnouncements = model.announcements.Where(q => q.sectionId == (int)SectionEnum.Member_Announce).ToList(),
                schoolAnnouncements = model.announcements.Where(q => q.sectionId == (int)SectionEnum.School_Announce).ToList(),
                
            };
            ViewBag.formViewMode = mode;
            return vm;
        }
        public IActionResult Edit(int? id)
        {
            //if (id == null) return RedirectToAction("Error");
            var model = unitOfWork.homePages.get().FirstOrDefault();
            if (model == null) model = new HomePage();
            var vm = setupViewModel(model, FormViewMode.Edit);
            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit()
        {
            var modelObj = unitOfWork.homePages.get().FirstOrDefault();
            if (modelObj == null)
                modelObj = new HomePage();
            acceptPost(modelObj);
            if (ModelState.ErrorCount == 0)
                return RedirectToAction("Index");
            var vm = setupViewModel(modelObj, FormViewMode.Edit);
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
            var homePage = new HomePage()
            {

            };
        }
    }
}
