using System;
using System.Collections.Generic;
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
        public JsonResult SaveRecord(int? id)
        {
            bool creating = (id?? 0) <= 0;
            List<string> errmsg = new List<string>();
            List<string> succmsg = new List<string>();
            HomeAttachmentViewModel vm = new HomeAttachmentViewModel();
            HomeAttachment modelObj = new HomeAttachment();
            TryUpdateModelAsync(vm);
            if (creating)
            {
                
            }
            else
            {
                modelObj = unitOfWork.homeAttachments.get().Where(q => q.id == id).FirstOrDefault();

            }
            AcceptPost()
            
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