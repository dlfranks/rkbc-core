using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.web.viewmodels;
using RKBC.Models;
using ElmahCore;
using rkbc.core.helper;
using Microsoft.AspNetCore.Identity;
using rkbc.models.extension;
using Microsoft.AspNetCore.Authorization;
using rkbc.web.controllers;
using rkbc.core.service;
using System.ComponentModel.DataAnnotations;
using rkbc.config.models;
using Microsoft.Extensions.Options;

namespace rkbc.web.viewmodels
{
    public class HomeContentItemViewModel{
        public int id { get; set; }
        public int homePageId { get; set; }
        public int sectionId { get; set; }
        public string content { get; set; }
    }
    public class HomeImageUrl
    {
        public string imageUrl { get; set; }
    }
    public class HomePageViewModel
    {
        public HomePageViewModel()
        {
            churchAnnouncements = new List<HomeContentItemViewModel>();
            memberAnnouncements = new List<HomeContentItemViewModel>();
            schoolAnnouncements = new List<HomeContentItemViewModel>();
        }
        public int id { get; set; }
        public string bannerUrl { get; set; }
        public string bannerFileName { get; set; }
        [Display(Name = "Banner Image")]
        public IFormFile bannerImage { get; set; }
        [Display(Name = "Title")]
        public string title { get; set; }
        [Display(Name = "Title Content")]
        public string titleContent { get; set; }
        [Display(Name = "Church Title")]
        public string churchAnnounceTitle { get; set; }
        [Display(Name = "Member Title")]
        public string memberAnnounceTitle { get; set; }
        [Display(Name = "School Title")]
        public string schoolAnnounceTitle { get; set; }
        [Display(Name = "Youtube Link for the Sermon of the Week")]
        public string sundayServiceVideoUrl { get; set; }
        public string embedVideoUrl { get; set; }
        public virtual List<HomeContentItemViewModel> churchAnnouncements { get; set; }
        public virtual List<HomeContentItemViewModel> memberAnnouncements { get; set; }
        public virtual List<HomeContentItemViewModel> schoolAnnouncements { get; set; }
        public virtual List<HomeImageUrl> attachments { get; set; }
        public int numLi { get; set; }
    }
}
namespace rkbc.web.controllers
{
    public class HomeController : AppBaseController
    {
        protected UserManager<ApplicationUser> userManager;
        protected SignInManager<ApplicationUser> signinManager;
        protected FileHelper fileHelper;
        protected IOptions<RkbcConfig> rkbcSetting;
        
        public HomeController(IUnitOfWork _unitOfWork,FileHelper _fileHelper,
                                UserManager<ApplicationUser> _userManager, UserService _userService,
                                SignInManager<ApplicationUser> _signinManager, IOptions<RkbcConfig> rkbcConfig) : base(_unitOfWork, _userService)
        {
            
            this.fileHelper = _fileHelper;
            this.userManager = _userManager;
            this.signinManager = _signinManager;
            this.rkbcSetting = rkbcConfig;
        
        }
        
        public async Task<IActionResult> Index()
        {
            int homePageId = rkbcSetting.Value.HomePageId;
           HomePage modelObj = new HomePage();
            modelObj = await unitOfWork.homePages.get(homePageId).Include("announcements").FirstOrDefaultAsync();
            
            var vm = setupViewModel(modelObj, FormViewMode.View);
            vm.attachments = await unitOfWork.homeAttachments.get().Where(q => q.sectionId == (int)SectionEnum.Home_Gallery && q.isOn == true)
                    .Select(q => new HomeImageUrl
                    {
                        imageUrl = fileHelper.generateAssetURL("gallery", q.fileName)
                    }
                ).ToListAsync();
            
            vm.numLi = vm.attachments.Count / 6;
            
            return View(vm);
        }
        protected void acceptPost(HomePage modelObj, HomePageViewModel model)
        {
            modelObj.title = model.title;
            modelObj.titleContent = model.titleContent;
            modelObj.churchAnnounceTitle = model.churchAnnounceTitle;
            modelObj.memberAnnounceTitle = model.memberAnnounceTitle;
            modelObj.schoolAnnounceTitle = model.schoolAnnounceTitle;
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
                System.Drawing.Bitmap bitmap = null;
                try { 
                    bitmap = new System.Drawing.Bitmap(model.bannerImage.OpenReadStream());
                }
                catch (Exception e)
                {
                    var msg = "Unable to read image format, please upload either .jpeg or .png images.";
                    ModelState.AddModelError("banner", msg);
                    //ElmahCore.XmlFileErrorLog.;
                }
                try
                {
                    ImageHelper.saveJpegImage(bitmap, assetFileAndPathName, 75L);
                    //Thumbnail width 150;
                    
                }
                catch (Exception e)
                {
                    var msg = "Internal error, unable to save the image.";
                    ModelState.AddModelError("bannerImageUrl", msg);
                    //ElmahCore
                }
                ImageHelper.GenerateThumbnail(bitmap, 150, assetFileAndPathName);
                modelObj.bannerFileName = assetFileName;
                modelObj.originalFileName = model.bannerImage.FileName;
                modelObj.bannerUrl = fileHelper.generateAssetURL("banner", modelObj.bannerFileName);
            }
            

            foreach (var item in model.churchAnnouncements)
            {
                announcements.Add(new HomeContentItem()
                {
                    homePageId = modelObj.id,
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
                    homePageId = modelObj.id,
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
                    homePageId = modelObj.id,
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
                embedVideoUrl = fileHelper.youtubeEmbedUrl(model.sundayServiceVideoUrl) == null? "" : fileHelper.youtubeEmbedUrl(model.sundayServiceVideoUrl),
                
                
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
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            
            HomePage modelObj = null;
            if (id == null) id = rkbcSetting.Value.HomePageId;
            modelObj = await unitOfWork.homePages.get(id.Value).Include("announcements").FirstOrDefaultAsync();
            if (modelObj == null)
            {
                throw new NullReferenceException("HomePage must have data.");
            }

            var vm = setupViewModel(modelObj, FormViewMode.Edit);
            
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit()
        {
            HomePageViewModel model = new HomePageViewModel();
            await TryUpdateModelAsync(model);
            var userId = userManager.GetUserId(HttpContext.User);
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
                modelObj.lastUpdDt = DateTime.UtcNow;
                modelObj.lastUpdUser = userId;
                unitOfWork.homePages.update(modelObj);
            }

            acceptPost(modelObj, model);
            if (ModelState.ErrorCount == 0)
            {
                var success = await unitOfWork.tryUniqueConstraintCommitAsync();
                if (success)
                {
                   return RedirectToAction("Index", "Home");
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
