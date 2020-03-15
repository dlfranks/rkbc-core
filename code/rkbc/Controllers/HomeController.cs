using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.web.viewmodels;
using RKBC.Models;
using ElmahCore;
using rkbc.core.services;
using rkbc.core.helper;
using Microsoft.AspNetCore.Identity;
using rkbc.models.extension;
using Microsoft.AspNetCore.Authorization;
using rkbc.web.controllers;
using rkbc.core.service;

namespace rkbc.web.viewmodels
{
    public class HomeContentItemViewModel{
        public int id { get; set; }
        public int homePageId { get; set; }
        public int sectionId { get; set; }
        public string content { get; set; }
    }
    public class HomePageViewModel
    {
        public int id { get; set; }
        public string bannerUrl { get; set; }
        public string bannerFileName { get; set; }
        public IFormFile bannerImage { get; set; }
        public string title { get; set; }
        public string titleContent { get; set; }
        public string churchAnnounceTitle { get; set; }
        public string memberAnnounceTitle { get; set; }
        public string schoolAnnounceTitle { get; set; }
        public string sundayServiceVideoUrl { get; set;}
        public virtual List<HomeContentItemViewModel> churchAnnouncements { get; set; }
        public virtual List<HomeContentItemViewModel> memberAnnouncements { get; set; }
        public virtual List<HomeContentItemViewModel> schoolAnnouncements { get; set; }
        
    }
}
namespace rkbc.web.Controllers
{
    public class HomeController : AppBaseController
    {
        public UserManager<ApplicationUser> userManager;
        public SignInManager<ApplicationUser> signinManager;
        public IMapper mapper;
        public FileHelper fileHelper;
        public HomeController(IUnitOfWork _unitOfWork, IMapper _mapper, FileHelper _fileHelper,
                                UserManager<ApplicationUser> _userManager, UserService _userService,
                                SignInManager<ApplicationUser> _signinManager) : base(_unitOfWork, _userService)
        {
            this.mapper = _mapper;
            this.fileHelper = _fileHelper;
            this.userManager = _userManager;
            this.signinManager = _signinManager;

        }
        public IActionResult Index()
        {
            return View();
        }
        protected void acceptPost(HomePage modelObj, HomePageViewModel model)
        {
            modelObj.title = model.title;
            modelObj.titleContent = model.titleContent;
            modelObj.churchAnnounceTitle = model.churchAnnounceTitle;
            modelObj.memberAnnounceTitle = model.memberAnnounceTitle;
            modelObj.schoolAnnounceTitle = model.schoolAnnounceTitle;
            if (!String.IsNullOrWhiteSpace(modelObj.bannerFileName))
                ModelState.AddModelError("bannerFileName", "Please Choose a banner image.");
            modelObj.bannerUrl = fileHelper.generateAssetURL("banner", modelObj.bannerFileName);
            

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
                    ModelState.AddModelError("banner", msg);
                    //ElmahCore.XmlFileErrorLog.;
                }
                try
                {
                    Imaging.saveJpegImage(bitmap, assetFileAndPathName, 75L);
                    //Thumbnail width 150;
                    
                }
                catch (Exception e)
                {
                    var msg = "Internal error, unable to save the image.";
                    ModelState.AddModelError("bannerImageUrl", msg);
                    //ElmahCore
                }
                Imaging.GenerateThumbnail(bitmap, 150, assetFileAndPathName);
                modelObj.bannerFileName = assetFileName;
                modelObj.originalFileName = model.bannerImage.FileName;
            }
            

            foreach (var item in model.churchAnnouncements)
            {
                announcements.Add(new HomeContentItem()
                {
                    homePageId = (int)PageEnum.Home,
                    homePage = modelObj,
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
                    homePage = modelObj,
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
                    homePage = modelObj,
                    sectionId = (int)SectionEnum.School_Announce,
                    content = item.content,
                    isOn = true
                });
            }
            unitOfWork.updateCollection<HomeContentItem>(modelObj.announcements, announcements);
        }
        protected HomePageViewModel setupViewModel(HomePage model, FormViewMode mode)
        {
            HomePageViewModel vm = new HomePageViewModel() {
                id = model.id,
                bannerFileName = model.bannerFileName,
                title = model.title,
                titleContent = model.titleContent,
                bannerUrl = fileHelper.generateAssetURL("banner", model.bannerFileName),
                churchAnnounceTitle = model.churchAnnounceTitle,
                memberAnnounceTitle = model.memberAnnounceTitle,
                schoolAnnounceTitle = model.schoolAnnounceTitle,
                sundayServiceVideoUrl = model.sundayServiceVideoUrl,
                
                
            };
            
            vm.churchAnnouncements = model.announcements.Where(q => q.sectionId == (int)SectionEnum.Church_Announce)
                .Select(q => new HomeContentItemViewModel { 
                    id = q.id,
                    homePageId = q.homePageId,
                    sectionId = q.sectionId,
                    content = q.content
                }).ToList();
            vm.memberAnnouncements = model.announcements.Where(q => q.sectionId == (int)SectionEnum.Member_Announce)
                .Select(q => new HomeContentItemViewModel
                {
                    id = q.id,
                    homePageId = q.homePageId,
                    sectionId = q.sectionId,
                    content = q.content
                }).ToList();
            vm.schoolAnnouncements = model.announcements.Where(q => q.sectionId == (int)SectionEnum.School_Announce)
                .Select(q => new HomeContentItemViewModel
                {
                    id = q.id,
                    homePageId = q.homePageId,
                    sectionId = q.sectionId,
                    content = q.content
                }).ToList();
            ViewBag.formViewMode = mode;
            return vm;
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit()
        {
            int id = 3;
            //if (id == null) return RedirectToAction("Error");
            HomePage modelObj = null;
            
                modelObj = await unitOfWork.homePages.get(id).Include("announcements").FirstOrDefaultAsync();
            
            if (modelObj == null)
            {
                throw new NullReferenceException("HomePage must have data.");
                
            }
                
            var vm = setupViewModel(modelObj, FormViewMode.Edit);
            
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(int id)
        {
            HomePageViewModel model = new HomePageViewModel();
            await TryUpdateModelAsync(model);
            var userId = userManager.GetUserId(HttpContext.User);//userService
            var currentUser = userService.CurrentUserSettings;

            HomePage modelObj = await unitOfWork.homePages.get(model.id).Include("announcements").FirstOrDefaultAsync();
            var d = HttpContext.User.Identity.getDepartment();
            if (modelObj == null)
            {
                modelObj = new HomePage();
                modelObj.createDt = DateTime.UtcNow;
                modelObj.createUser = userId;
                modelObj.lastUpdDt = DateTime.UtcNow;
                modelObj.lastUpdUser = userId;
                unitOfWork.homePages.add(modelObj);
            }
            else
            {
                if(modelObj.createDt == null ) modelObj.createDt = DateTime.UtcNow;
                if (modelObj.createUser == null) modelObj.createUser = userId;
                modelObj.createUser = userId;
                modelObj.lastUpdDt = DateTime.UtcNow;
                modelObj.lastUpdUser = userId;
                unitOfWork.homePages.update(modelObj);
            }

            acceptPost(modelObj, model);
            if (ModelState.ErrorCount == 0)
            {
                var success = await unitOfWork.tryCommit();
                if (success)
                {
                   return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Unable to update data.");
                //Elmah error
            }
                
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
