using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{
    public class HomeAttachmentViewModel
    {
        public int id { get; set; }
        public string url { get; set; }
        public string originalFileName { get; set; }
       
    }

}

namespace rkbc.web.controllers
{
    public class AttachmentController : Controller
    {
        protected IUnitOfWork unitOfwork;
        protected IWebHostEnvironment env;
        public AttachmentController(IUnitOfWork wrk, IWebHostEnvironment _env)
        {
            this.unitOfwork = wrk;
            this.env = _env;
        }
        // GET: Attachment
        public ActionResult Index()
        {
            
            var lst = unitOfwork.homeAttachments.get().Where(q => q.homePageId == (int)PageEnum.Home && q.sectionId == (int)SectionEnum.Home_Gallery)
                .Select(q => new HomeAttachmentViewModel()
                {
                    id = q.id,
                    url = System.IO.Path.Combine(env.WebRootPath, q.fileName),
                    originalFileName = q.originalFileName
                }).ToList();
            return View(lst);
        }

        // GET: Attachment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Attachment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Attachment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Attachment/Edit/5
        public ActionResult Edit(int id)
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