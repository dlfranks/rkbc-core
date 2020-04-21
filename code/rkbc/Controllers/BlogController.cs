using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using rkbc.core.helper;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.controllers;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{
    public class PostViewModel
    {
        public Post post { get; set; }
        public IFormFile image { get; set; }
        public string embedVideoUrl { get; set;}
        public string imageUrl { get; set; }
    }

}
namespace rkbc.web.controllers
{
    public class PostController : AppBaseController
    {
        protected FileHelper fileHelper;
        public PostController(FileHelper _fileHelper, 
            IUnitOfWork _unitOfWork, 
            UserService _userService) : base(_unitOfWork, _userService)
        {
            fileHelper = _fileHelper;
        }
        protected async Task acceptImage(Post modelObj, IFormFile file)
        {
            var extension = fileHelper.getExtension(file.FileName);
            var fileName = fileHelper.getFileName(file.FileName);
            var assetFileName = fileHelper.newAssetFileName("blog", extension);
            var assetFileAndPathName = fileHelper.mapAssetPath("blog", assetFileName, false);
            System.Drawing.Bitmap bitmap = null;
            try
            {
                bitmap = new System.Drawing.Bitmap(file.OpenReadStream());
            }
            catch (Exception e)
            {
                var msg = "Unable to read image format, please upload either .jpeg or .png images.";
                ModelState.AddModelError("image", msg);
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
                ModelState.AddModelError("image", msg);
                //ElmahCore
            }
            ImageHelper.GenerateThumbnail(bitmap, 150, assetFileAndPathName);
            modelObj.imageFileName = assetFileName;
        }
        protected async Task acceptPost(Post modelObj, PostViewModel vModel)
        {
            modelObj.title = vModel.post.title;
            modelObj.slug = string.IsNullOrWhiteSpace(vModel.post.slug)? Post.CreateSlug(vModel.post.title) : vModel.post.slug.Trim();
            modelObj.excerpt = vModel.post.excerpt;
            modelObj.content = vModel.post.content;
            modelObj.videoURL = vModel.post.videoURL;
            if (modelObj.isPublished)
                modelObj.pubDate = DateTime.UtcNow;
            modelObj.videoURL = vModel.post.videoURL;
            if(vModel.image != null && vModel.image.Length > 0)
                await acceptImage(modelObj, vModel.image);
        }
        protected PostViewModel setupViewModel(Post model)
        {
            PostViewModel vm = new PostViewModel();
            vm.post = model;
            vm.embedVideoUrl = fileHelper.youtubeEmbedUrl(model.videoURL) == null ? "" : fileHelper.youtubeEmbedUrl(model.videoURL);
            vm.imageUrl = fileHelper.generateAssetURL("blog", model.imageFileName);
            return vm;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            
            //permission for create
            
            var blog = new Blog();
            blog.authorId = userService.CurrentUserSettings.userId;
            
            var post = new Post();
            post.blogId = blog.id;
            post.blog = blog;

            var vm = setupViewModel(post);
            return View(vm);
        }
        /// <remarks>This is for redirecting potential existing URLs from the old Miniblog URL format.</remarks>
        [Route("/post/{slug}")]
        [HttpGet]
        public IActionResult Redirects(string slug) => this.LocalRedirectPermanent($"/blog/{slug}");

        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel vModel)
        {
            //permission
            var modelObj = new Post();
            //handle the blog first
            if (vModel.post.blogId == 0)
            {
                modelObj.blog = new Blog()
                {
                    authorId = userService.CurrentUserSettings.userId,
                    createDt = DateTime.UtcNow,
                    lastUpdDt = DateTime.UtcNow

                };
            }
            else
            {
                modelObj.blog.authorId = userService.CurrentUserSettings.userId;
                modelObj.blog.lastUpdDt = DateTime.UtcNow;
            }
            //new Post
            modelObj.blogId = modelObj.blog.id;
            modelObj.createDt = DateTime.UtcNow;
            modelObj.lastModified = DateTime.UtcNow;
            await acceptPost(modelObj, vModel);
            if (!ModelState.IsValid)
            {
                var vm = setupViewModel(modelObj);
                return View(vm);
            }
            return this.Redirect(modelObj.GetEncodedLink());
        }
        public IActionResult Edit(PostViewModel vModel)
        {
            
            return View();
        }
    }
}