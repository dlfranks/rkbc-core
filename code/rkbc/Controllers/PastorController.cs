using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using rkbc.config.models;
using rkbc.core.helper;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.controllers;
using rkbc.web.viewmodels;
using Microsoft.EntityFrameworkCore;

namespace rkbc.web.viewmodels
{
    public class PastorPageViewModel
    {
        public PastorPage pastorPage { get; set; }
        public string imageUrl { get; set; }
    }
}
namespace rkbc.web.controllers
{
    public class PastorController : AppBaseController
    {
        protected UserManager<ApplicationUser> userManager;
        protected FileHelper fileHelper;
        protected IOptions<RkbcConfig> rkbcSetting;

        public PastorController(IUnitOfWork _unitOfWork, FileHelper _fileHelper,
                                UserManager<ApplicationUser> _userManager, UserService _userService,
                                IOptions<RkbcConfig> rkbcConfig) : base(_unitOfWork, _userService)
        {

            this.fileHelper = _fileHelper;
            this.userManager = _userManager;
            this.rkbcSetting = rkbcConfig;

        }
        protected async Task<PastorPageViewModel> setupViewModel(PastorPage modelObj, FormViewMode mode)
        {
            var vm = new PastorPageViewModel()
            {
                pastorPage = modelObj,
               
            };
            if(mode == FormViewMode.View)
            {
                var img = await unitOfWork.attachments.get().Where(q => q.attachmentSectionEnum == (int)AttachmentSectionEnum.Pastor_Gallery).FirstOrDefaultAsync();
                if (img != null)
                {
                    vm.imageUrl = fileHelper.generateAssetURL("gallery", img.fileName);
                }
            }
            
            return vm;
        }
        
        
        public async Task<IActionResult> Index()
        {
            var pastorPageId = rkbcSetting.Value.PastorPageId;
            var model = await unitOfWork.pastorPages.get(pastorPageId).FirstOrDefaultAsync();
            if (model == null) throw new InvalidOperationException("Not Found Pastor page Id.");
            var vm = await setupViewModel(model, FormViewMode.View);
            return View(vm);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            PastorPage modelObj = null;
            if (id == null) id = rkbcSetting.Value.PastorPageId;
            modelObj = await unitOfWork.pastorPages.get(id.Value).FirstOrDefaultAsync();

            if (modelObj == null)
            {
                modelObj = new PastorPage();
                modelObj.pageTitle = "\"우리 주 예수 그리스도를 변함없이 사랑하는 모든자 에게 은혜가 있을찌어다\" 엡 6:24";
                modelObj.author = "이순신 목사(Soon - Shin Choi)";
                modelObj.columnTitle = "주님께서는 반드시 우리가 그분께 늘 의지하도록 하신다.";
                modelObj.column = "주님께서는 반드시 우리가 그분께 늘 의지하도록 하신다. 우리가 감당할 수 없는 상황으로 우리를 인도하심으로써 그렇게 하신다. 그렇기때문에 우리가 걱정하더라도 하나님께서는 걱정하지 않으신다. 무엇을 행할 것인지 이미 알고 계시기 때문이다. 그분께는 걸어갈 길과 해야할 일에 대한 계획이 이미 있으며, 그것은 우리를 위해 온전히 준비되어 있다. 이런 이야기가 있다.한 젊은이가 그리스도를 믿는 믿음 때문에 감옥에 있는데 다음날 기둥에 묶여 화형될 처지였다.같은 감방에 나이도 많고 연륜도 깊은 신자가 있었는데 주님의 도에 더 잘 아는 사람이었다. 날이 어두워지자 젊은이는 호에 불을 붙이려고 했고 그러다가 손가락을 데었다. 손가락의 고통 때문에 소리를 지르면서 그 동료에게 물었다. \"손가락 데인것도 참을 수 없는데, 기둥에 묶여 화형되는 걸 어떻게 참죠?\" 그 연장자는 조용히 대답했다. \"젊은이! 하나님께서는 당신에게 손가락을 데라고 하지 않으셨기 때문에 그 일에 대해서는 아무 은햬도 임하지 않았지만, 믿음을 위해 목숨을 잃는 것은 하나님께서 당신에게 요구하시는 것이므로 그때가 되면 하나님의 은햬가 그곳에 임할 것이오.\" 하나님께서는 우리가 이 세상을 살아갈때 겪는 모든 일을 처리할 계획을 갖고 계신다. 하나님의 넉넉한 은혜는 우리의 부족을 채우신다.";
                //throw new NullReferenceException("Pastor Page must have data.");
            }
            
            

            return View(modelObj);
            
        }
        [HttpPost]
        public async Task<IActionResult> Edit()
        {
            PastorPage vmodel = new PastorPage();
            await TryUpdateModelAsync(vmodel);
            var currentUser = userService.CurrentUserSettings;
            PastorPage modelObj = await unitOfWork.pastorPages.get(vmodel.id).FirstOrDefaultAsync();
            
            if (modelObj == null)
            {
                modelObj = new PastorPage();
                modelObj.createDt = DateTime.UtcNow;
                modelObj.createUser = currentUser.userId;
                modelObj.lastUpdDt = DateTime.UtcNow;
                modelObj.lastUpdUser = currentUser.userId;
                unitOfWork.pastorPages.add(modelObj);
            }
            else
            {
                if (modelObj.createDt == null) modelObj.createDt = DateTime.UtcNow;
                if (modelObj.createUser == null) modelObj.createUser = currentUser.userId;
                modelObj.lastUpdDt = DateTime.UtcNow;
                modelObj.lastUpdUser = currentUser.userId;
                unitOfWork.pastorPages.update(modelObj);
            }
            modelObj.pageTitle = vmodel.pageTitle;
            modelObj.columnTitle = vmodel.columnTitle;
            modelObj.column = vmodel.column;
            modelObj.author = vmodel.author;
            if (ModelState.ErrorCount == 0)
            {
                var success = await unitOfWork.tryConcurrencyCommitAsync();
                if (success)
                {
                    return RedirectToAction("Index", "Pastor");
                }
                ModelState.AddModelError("", "Unable to update data.");

                //Elmah error
            }
            return View(modelObj);
            
        }
        
    }
}