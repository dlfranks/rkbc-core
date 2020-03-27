using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using rkbc.config.models;
using rkbc.core.helper;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.helpers;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{

    public class HomeAttachmentIndexViewModel
    {
        public string status { get; set; }
        public string search { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }

        public int? getStartRecord { get; set; }
        public int? getRecordCount { get; set; }

        public string sortkey { get; set; }
        public int? sortdir { get; set; }

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
        

    }

}

namespace rkbc.web.controllers
{
    public class AttachmentController : AppBaseController
    {
        protected FileHelper fileHelper;
        protected IOptions<RkbcConfig> rkbcSetting;
        public AttachmentController(IUnitOfWork _unitOfWork, UserService _userService, 
                                    IWebHostEnvironment _env, FileHelper _fileHelper, IOptions<RkbcConfig> rkbcConfig)
                                    :base(_unitOfWork, _userService)
        {
            this.fileHelper = _fileHelper;
            rkbcSetting = rkbcConfig;
        }
        // GET: Attachment
        public ActionResult Index()
        {
            HomeAttachmentIndexViewModel vmodel = loadPagePref<HomeAttachmentIndexViewModel>(defaultPref: () =>
            {
                return new HomeAttachmentIndexViewModel
                {
                    getStartRecord = 0,
                    getRecordCount = 80,
                    status = "incomplete",
                    sortkey = "project",
                    sortdir = 1
                };
            });
            ViewBag.sectionList = new object[]
               {
                    new { id =(int)SectionEnum.Home_Gallery, name = EnumHelper.GetDiscription<SectionEnum>(SectionEnum.Home_Gallery)}
               };
            return View(vmodel);
        }

        // GET: Attachment/Details/5
        public async Task<JsonResult> GetRecords(HomeAttachmentIndexViewModel vmodel)
        {
            int serverCount = 0;
            //Save settings            
            savePagePref(vmodel);
            var query = unitOfWork.homeAttachments.get();
            serverCount = query.Count();
            var lst = await query
                .Skip(vmodel.getStartRecord.Value).Take(vmodel.getRecordCount.Value)
                .Select(q => new HomeAttachmentViewModel { 
                    id = q.id,
                    sectionId = (int)SectionEnum.Home_Gallery,
                    isOn = q.isOn,
                    fileName = q.fileName,
                    originalFileName = q.originalFileName,
                    url = fileHelper.generateAssetURL("gallery", q.fileName, true)
                }).ToListAsync();
            
            
            return Json(new { items =lst, itemServerCount = serverCount });
        }

        // GET: Attachment/Create
        public JsonResult GetRecord(int id)
        {
            var model = new HomeAttachmentViewModel();
            model.sectionId = (int)SectionEnum.Home_Gallery;
            
            return Json(model);
        }
        // POST: Attachment/Edit
        [HttpPost]
        public async Task<JsonResult> UpdateAttachment([FromBody]HomeAttachmentViewModel model)
        {
            var homePageId = rkbcSetting.Value.HomePageId;
            List<string> errmsg = new List<string>();
            List<string> succmsg = new List<string>();
            HomeAttachment modelObj = new HomeAttachment();
            
            if ((!ModelState.IsValid) || (model.id == 0))
            {
                errmsg.Add("Unabled to update the image.");
            }
            else
            {

                modelObj = await unitOfWork.homeAttachments.get(model.id).FirstOrDefaultAsync();
                if (modelObj == null) errmsg.Add("Unabled to update the image.");
                unitOfWork.homeAttachments.update(modelObj);
                modelObj.isOn = model.isOn;
                modelObj.sectionId = model.sectionId;
                var success = await unitOfWork.tryCommitAsync();
                if (success) succmsg.Add("Updated.");
                else errmsg.Add("unabled to update the image.");

            }
            var strerrmsg = ""; if (errmsg.Count() > 0) foreach (var s in errmsg) strerrmsg += "<p class=\"error\">" + s + "</p>";
            var strsuccmsg = ""; if (succmsg.Count() > 0) foreach (var s in succmsg) strsuccmsg += "<p>" + s + "</p>";
            var result = new
            {
                success = errmsg.Count == 0,
                isOn = modelObj.isOn,
                sectionId = modelObj.sectionId,
                errmsg = strerrmsg,
                succmsg = strsuccmsg,
            };
            return Json(result);
        }
        // POST: Attachment/Create
        [HttpPost]
        public async Task<JsonResult> CreateAttachment(IFormFile image)
        {
            var homePageId = rkbcSetting.Value.HomePageId;
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
                    isOn = modelObj.isOn,
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