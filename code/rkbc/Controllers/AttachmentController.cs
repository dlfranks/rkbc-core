using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rkbc.core.helper;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.Helpers;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{

    public class HomeAttachmentIndexViewModel
    {
        public object[] sectionList { get; set; }
        public List<HomeAttachmentViewModel> items { get; set; }
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
        public async Task<ActionResult> Index()
        {
            var vm = new HomeAttachmentIndexViewModel();
            var lst = await unitOfWork.homeAttachments.get().Where(q => q.sectionId == (int)SectionEnum.Home_Gallery)
                .Select(q => new HomeAttachmentViewModel
                {
                    id = q.id,
                    sectionId = (int)SectionEnum.Home_Gallery,
                    fileName = q.fileName,
                    originalFileName = q.originalFileName,
                    url = fileHelper.generateAssetURL("gallery", q.fileName, true)
                }).ToListAsync();
            vm.sectionList = new object[]
                {
                    new { id =(int)SectionEnum.Home_Gallery, name = EnumHelper.GetDiscription<SectionEnum>(SectionEnum.Home_Gallery)}
                };

            vm.items = lst;
            ViewBag.InitialData = lst;
            return View(vm);
        }

        // GET: Attachment/Details/5
        public async Task<JsonResult> GetRecords()
        {
            var lst = await unitOfWork.homeAttachments.get().Where(q => q.sectionId == (int)SectionEnum.Home_Gallery)
                .Select(q => new HomeAttachmentViewModel { 
                    id = q.id,
                    sectionId = (int)SectionEnum.Home_Gallery,
                    fileName = q.fileName,
                    originalFileName = q.originalFileName,
                    url = fileHelper.generateAssetURL("gallery", q.fileName, true)
                }).ToListAsync();
            ViewBag.sectionList = new object[]
                {
                    new { id =(int)SectionEnum.Home_Gallery, name = EnumHelper.GetDiscription<SectionEnum>(SectionEnum.Home_Gallery)}
                };
                
            
            return Json(lst);
        }

        // GET: Attachment/Create
        public JsonResult GetRecord(int id)
        {
            var model = new HomeAttachmentViewModel();
            model.sectionId = (int)SectionEnum.Home_Gallery;
            
            return Json(model);
        }

        // POST: Attachment/Create
        [HttpPost]
        public async Task<JsonResult> SaveAttachment(IFormFile image)
        {
            
            var homePageId = 3;
            List<string> errmsg = new List<string>();
            List<string> succmsg = new List<string>();
            HomeAttachment modelObj = new HomeAttachment();
            var homePage = unitOfWork.homePages.get(homePageId).First();
            var user = userService.CurrentUserSettings;
            //var file = HttpContext.Request.Form.Files["image"];
            unitOfWork.homeAttachments.add(modelObj);

            modelObj.homePageId = homePage.id;
            modelObj.sectionId = (int)SectionEnum.Home_Gallery;
            modelObj.createDt = DateTime.UtcNow;
            modelObj.createUser = user.userId;
            modelObj.lastUpdDt = DateTime.UtcNow;
            modelObj.lastUpdUser = user.userId;
            modelObj.isOn = true;
            modelObj.originalFileName = image.FileName;
            //Save a attachment
            var extension = fileHelper.getExtension(image.FileName);
            var fileName = fileHelper.getFileName(image.FileName);
            var assetFileName = fileHelper.newAssetFileName("gallery", extension);
            var assetFileAndPathName = fileHelper.mapAssetPath("gallery", assetFileName, false);
            Bitmap bitmap = null;
            try
            {
                bitmap = new Bitmap(image.OpenReadStream());
            }
            catch (Exception e)
            {
                var msg = "Unable to read image format, please upload either .jpeg or .png images.";
                ModelState.AddModelError("gallery", msg);
                errmsg.Add(msg);
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
                ModelState.AddModelError("GalleryImageUrl", msg);
                errmsg.Add(msg);
                //ElmahCore
            }
            ImageHelper.GenerateThumbnail(bitmap, 150, assetFileAndPathName);
            modelObj.fileName = assetFileName;
            //Update the database
            if (errmsg.Count == 0)
            {

                if (await unitOfWork.tryCommitAsync())
                {
                    succmsg.Add("Image Saved.");
                }
                else
                {
                    errmsg.Add("Database error, unable to save the image."); 
                }
            }
            var strerrmsg = ""; if (errmsg.Count() > 0) foreach (var s in errmsg) strerrmsg += "<p class=\"error\">" + s + "</p>";
            var strsuccmsg = ""; if (succmsg.Count() > 0) foreach (var s in succmsg) strsuccmsg += "<p>" + s + "</p>";
            var result = new
            {
                errmsg = strerrmsg,
                succmsg = strsuccmsg,
                success = errmsg.Count == 0,
                item = new {
                    id = modelObj.id,
                    sectionId = modelObj.sectionId,
                    originalFileName = modelObj.originalFileName,
                    fileName = modelObj.fileName,
                    caption = modelObj.caption,
                    url = fileHelper.generateAssetURL("gallery", modelObj.fileName),
                    
                }
            };

            return Json(result);
        }
        // GET: Attachment/Delete/5
        [HttpPost]
        public async Task<JsonResult> DeleteRecord(int id)
        {
            string errmsg = "", succmsg = "";
            if (id == 0)
            {
                errmsg = "<p class=\"error\">Unable to delete the image</p>";
            }
            else
            {
                await unitOfWork.homeAttachments.removeAsync(id);
                if(!(await unitOfWork.tryCommitAsync()))
                {
                    errmsg = "<p class=\"error\">Unable to delete the image</p>";
                }
                else
                {
                    succmsg = "<p>The image Deleted</p>";
                }
            }
            return Json(new {
                success = String.IsNullOrEmpty(errmsg),
                errmsg = errmsg,
                succmsg = succmsg
            });
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