using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rkbc.core.helper;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{
    public class HomeAttachmentIndexViewModel { 
        
    }
    public class HomeAttachmentViewModel
    {
        public int id { get; set; }
        public string url { get; set; }
        public int sectionId { get; set; }
        public string originalFileName { get; set; }
        public string fileName { get; set; }
        public string caption { get; set; }
        public bool isOn { get; set; }
        public IFormFile image { get; set; }

    }

}

namespace rkbc.web.controllers
{
    public class AttachmentController : AppBaseController
    {
        protected FileHelper fileHelper;
        public AttachmentController(IUnitOfWork _unitOfWork, UserService _userService, 
                                    IWebHostEnvironment _env, FileHelper _fileHelper)
                                    :base(_unitOfWork, _userService)
        {
            this.fileHelper = _fileHelper;
            
        }
        // GET: Attachment
        public ActionResult Index()
        {
            
            
            return View();
        }

        // GET: Attachment/Details/5
        public JsonResult GetRecords()
        {
            var lst = unitOfWork.homeAttachments.get().Where(q => q.sectionId == (int)SectionEnum.Home_Gallery)
                .Select(q => new HomeAttachmentViewModel { 
                    id = q.id,
                    sectionId = (int)SectionEnum.Home_Gallery,
                    fileName = q.fileName,
                    originalFileName = q.originalFileName,
                    url = fileHelper.mapAssetPath("gallery", q.fileName, true)
                }).ToList();
            return Json(lst);
        }

        // GET: Attachment/Create
        public JsonResult GetRecord(int id)
        {
            var result = unitOfWork.homeAttachments.get().Where(q => q.sectionId == (int)SectionEnum.Home_Gallery && q.id == id)
                .Select(q => new HomeAttachmentViewModel
                {
                    id = q.id,
                    sectionId = (int)SectionEnum.Home_Gallery,
                    fileName = q.fileName,
                    originalFileName = q.originalFileName,
                    url = fileHelper.mapAssetPath("gallery", q.fileName, true)
                }).FirstOrDefault();
            return Json(result);
        }

        // POST: Attachment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SaveRecord(int? id)
        {
            bool creating = (id?? 0) <= 0;
            var homePageId = 3;
            List<string> errmsg = new List<string>();
            List<string> succmsg = new List<string>();
            HomeAttachmentViewModel vm = new HomeAttachmentViewModel();
            HomeAttachment modelObj = new HomeAttachment();
            var homePage = unitOfWork.homePages.get(homePageId).First();
            var user = userService.CurrentUserSettings;
            TryUpdateModelAsync(vm);
            if (!ModelState.IsValid)
            {
                errmsg.Add("");

            }
            else
            {
                if (!creating)
                {
                    modelObj = unitOfWork.homeAttachments.get().Where(q => q.id == id).FirstOrDefault();
                    modelObj.lastUpdDt = DateTime.UtcNow;
                    modelObj.lastUpdUser = user.userId;
                    modelObj.isOn = true;
                }
                else
                {
                    unitOfWork.homeAttachments.add(modelObj);

                    modelObj.homePageId = homePage.id;
                    modelObj.sectionId = (int)SectionEnum.Home_Gallery;
                    modelObj.createDt = DateTime.UtcNow;
                    modelObj.createUser = user.userId;
                    modelObj.lastUpdDt = DateTime.UtcNow;
                    modelObj.lastUpdUser = user.userId;
                    modelObj.isOn = true;
                    modelObj.originalFileName = vm.image.FileName;
                    
                    var extension = fileHelper.getExtension(vm.image.FileName);
                    var fileName = fileHelper.getFileName(vm.image.FileName);
                    var assetFileName = fileHelper.newAssetFileName("banner", extension);
                    var assetFileAndPathName = fileHelper.mapAssetPath("banner", assetFileName, false);
                    Bitmap bitmap = null;
                    try
                    {
                        bitmap = new Bitmap(vm.image.OpenReadStream());
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
                    modelObj.fileName = assetFileName;
                }
                //Update the database
                if(errmsg.Count == 0)
                {
                    var result = await unitOfWork.tryCommitAsync();
                }
            }
            
            
            
        }

        // GET: Attachment/Edit/5
        public void AccepPost()
        {
            return View();
        }

        // POST: Attachment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Attachment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Attachment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}