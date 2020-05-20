using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ElmahCore;
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

    public class AttachmentIndexViewModel
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
    public class AttachmentViewModel
    {
        public int id { get; set; }
        public string url { get; set; }
        public int sectionId { get; set; }
        public string originalFileName { get; set; }
        public string fileName { get; set; }
        public string caption { get; set; }
        public bool isOn { get; set; }
        public string pictureUrl { get; set; }

    }

}

namespace rkbc.web.controllers
{
    public class AttachmentController : AppBaseController
    {
        protected FileHelper fileHelper;
        protected IOptions<RkbcConfig> rkbcSetting;
        public AttachmentController(IUnitOfWork _unitOfWork, UserService _userService, 
                                    FileHelper _fileHelper, IOptions<RkbcConfig> rkbcConfig)
                                    :base(_unitOfWork, _userService)
        {
            this.fileHelper = _fileHelper;
            rkbcSetting = rkbcConfig;
        }
        //public FileResult View(string type, string fileName, bool thumbnail)
        //{
        //    string path;
        //    if (fileHelper.getExtension(fileName) == "pdf")
        //    {
        //        if (thumbnail)
        //            path = fileHelper.mapAssetPath(type, fileHelper.getFileName(fileName) + ".jpg", thumbnail);
        //        else
        //            path = fileHelper.mapAssetPath(type, fileHelper.getFileName(fileName) + ".pdf", thumbnail);
        //    }
        //    else
        //    {
        //        path = fileHelper.mapAssetPath(type, fileName, thumbnail);
        //    }
        //    return (new FileResult());
        //}
        // GET: Attachment
        public ActionResult Index()
        {
            AttachmentIndexViewModel vmodel = loadPagePref<AttachmentIndexViewModel>(defaultPref: () =>
            {
                return new AttachmentIndexViewModel
                {
                    getStartRecord = 0,
                    getRecordCount = 80,
                    status = "incomplete",
                    sortkey = "project",
                    sortdir = 1
                };
            });
            var names = Enum.GetNames(typeof(AttachmentSectionEnum));
            var attachSectionlist = new object[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                var value = (AttachmentSectionEnum)Enum.Parse(typeof(AttachmentSectionEnum), names[i]);
                attachSectionlist[i] = new { id = value, name = names[i] };
            }
            ViewBag.sectionList = attachSectionlist;
            return View(vmodel);
        }

        // GET: Attachment/Details/5
        public async Task<JsonResult> GetRecords(AttachmentIndexViewModel vmodel)
        {
            int serverCount = 0;
            //Save settings            
            savePagePref(vmodel);
            var query = unitOfWork.attachments.get();
            serverCount = query.Count();
            var lst = await query
                .Skip(vmodel.getStartRecord.Value).Take(vmodel.getRecordCount.Value)
                .OrderByDescending(q => q.createDt)
                .Select(q => new AttachmentViewModel { 
                    id = q.id,
                    sectionId = q.attachmentSectionEnum,
                    isOn = q.isOn,
                    fileName = q.fileName,
                    originalFileName = q.originalFileName,
                    url = fileHelper.generateAssetURL("gallery", q.fileName, true),
                    pictureUrl = fileHelper.generateAssetURL("gallery", q.fileName, false)
                }).ToListAsync();
            
            
            return Json(new { items =lst, itemServerCount = serverCount });
        }

        // GET: Attachment/Create
        public JsonResult GetRecord(int id)
        {
            var model = new AttachmentViewModel();
            model.sectionId = (int)AttachmentSectionEnum.Home_Gallery;
            
            return Json(model);
        }
        // POST: Attachment/Edit
        [HttpPost]
        public async Task<JsonResult> UpdateAttachment([FromBody]AttachmentViewModel model)
        {
            var homePageId = rkbcSetting.Value.HomePageId;
            List<string> errmsg = new List<string>();
            List<string> succmsg = new List<string>();
            Attachment modelObj = new Attachment();
            
            if ((!ModelState.IsValid) || (model.id == 0))
            {
                errmsg.Add("Unabled to update the image.");
            }
            else
            {

                modelObj = await unitOfWork.attachments.get(model.id).FirstOrDefaultAsync();
                if (modelObj == null) errmsg.Add("Unabled to update the image.");
                unitOfWork.attachments.update(modelObj);
                modelObj.isOn = model.isOn;
                modelObj.attachmentSectionEnum = model.sectionId;
                modelObj.pageEnum = (int)setPageEnum(model.sectionId);
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
                sectionId = modelObj.attachmentSectionEnum,
                errmsg = strerrmsg,
                succmsg = strsuccmsg,
            };
            return Json(result);
        }
        protected PageEnum setPageEnum(int attachmentSectionValue)
        {
            var discription = EnumHelper.GetDiscription<AttachmentSectionEnum>((AttachmentSectionEnum)attachmentSectionValue);
            var pageName = discription.Split(' ')[0];
            var value = Enum.Parse<PageEnum>(pageName);
           return value;
        }
        // POST: Attachment/Create
        [HttpPost]
        public async Task<JsonResult> CreateAttachment(IFormFile image)
        {
            var homePageId = rkbcSetting.Value.HomePageId;
            List<string> errmsg = new List<string>();
            List<string> succmsg = new List<string>();
            Attachment modelObj = new Attachment();
            var homePage = unitOfWork.homePages.get(homePageId).First();
            var user = userService.CurrentUserSettings;
            //var file = HttpContext.Request.Form.Files["image"];
            unitOfWork.attachments.add(modelObj);

            modelObj.pageEnum = (int)PageEnum.Home;
            modelObj.attachmentSectionEnum = (int)AttachmentSectionEnum.Home_Gallery;
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
                HttpContext.RiseError(new InvalidOperationException(msg));
            }
            try
            {
                //Adjust image size based on selection width 600
                bitmap = ImageHelper.ScaleImage(bitmap, BlogImageWidthConstants.SmallWidth, null);
                ImageHelper.saveJpegImage(bitmap, assetFileAndPathName, 75L);
                ImageHelper.GenerateThumbnail(bitmap, 150, assetFileAndPathName);
                //Thumbnail width 150;

            }
            catch (Exception e)
            {
                var msg = "Internal error, unable to save the image.";
                ModelState.AddModelError("GalleryImageUrl", msg);
                errmsg.Add(msg);
                HttpContext.RiseError(new InvalidOperationException(msg));
            }
            
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
                    sectionId = modelObj.attachmentSectionEnum,
                    isOn = modelObj.isOn,
                    originalFileName = modelObj.originalFileName,
                    fileName = modelObj.fileName,
                    caption = modelObj.caption,
                    url = fileHelper.generateAssetURL("gallery", modelObj.fileName),
                    pictureUrl = fileHelper.generateAssetURL("gallery", modelObj.fileName)
                    
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
                await unitOfWork.attachments.removeAsync(id);
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