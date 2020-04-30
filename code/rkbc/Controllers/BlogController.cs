using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using rkbc.config.models;
using rkbc.core.helper;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.constant;
using rkbc.web.controllers;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{
    public class BlogIndexViewModel
    {
        public string blogSlug { get; set; }
        public int currentPage { get; set; }
        public int postsPerPage { get; set; }
        public int totalPosts { get; set; }
        public int skip { get; set; }
        public string search { get; set; }
    }
    public class PostViewModel
    {
        public Post post { get; set; }
        public IFormFile image { get; set; }
        public string embededVideoUrl { get; set; }
        public string imageUrl { get; set; }
    }
    public abstract class PagedResultBase
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstRowOnPage
        {
            get { return (CurrentPage - 1) * PageSize + 1; }
        }
        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }

    public class PagedResult<T> : PagedResultBase where T : class
    {
        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}
namespace rkbc.web.controllers
{
    public class BlogController : AppBaseController
    {
        private FileHelper fileHelper;
        private BlogService blogService;
        private readonly IOptionsSnapshot<BlogSettings> settings;
        private IUrlHelper urlHelper;
        public BlogController(FileHelper _fileHelper, IOptionsSnapshot<BlogSettings> _settings,
            IUnitOfWork _unitOfWork, 
            UserService _userService,
            BlogService _blogService,
            IUrlHelper _urlHelper) : base(_unitOfWork, _userService)
        {
            fileHelper = _fileHelper;
            blogService = _blogService;
            settings = _settings;
            urlHelper = _urlHelper;
        }
        protected void acceptImage(Post modelObj, IFormFile file)
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
        protected void acceptPost(Post modelObj, PostViewModel vModel)
        {
            modelObj.postType = vModel.post.postType;
            modelObj.title = vModel.post.title;
            modelObj.slug = string.IsNullOrWhiteSpace(vModel.post.slug)? rkbc.core.models.Post.CreateSlug(vModel.post.title) : vModel.post.slug.Trim();
            modelObj.excerpt = vModel.post.excerpt;
            modelObj.content = vModel.post.content;
            modelObj.videoURL = vModel.post.videoURL;
            if (modelObj.isPublished)
                modelObj.pubDate = DateTime.UtcNow;
            modelObj.videoURL = vModel.post.videoURL;
            if(vModel.image != null && vModel.image.Length > 0)
                acceptImage(modelObj, vModel.image);
        }
        protected PostViewModel setupViewModel(Post model)
        {
            PostViewModel vm = new PostViewModel();
            vm.post = model;
            vm.embededVideoUrl = fileHelper.youtubeEmbedUrl(model.videoURL) == null ? "" : fileHelper.youtubeEmbedUrl(model.videoURL);
            vm.imageUrl = fileHelper.generateAssetURL("blog", model.imageFileName);
            
            return vm;
        }
        [Route("/Blog/Index")]
        //[OutputCache(Profile = "default")]
        public async Task<IActionResult> Index(string id)
        {
            Blog blog = null; string blogSlug = "all";
            if(!string.IsNullOrEmpty(id))
            {
                blog = await unitOfWork.blogs.get().Where(q => q.authorId == id).FirstOrDefaultAsync();
                if (blog == null) return this.NotFound();
                blogSlug = blog.blogSlug;
            }
            
            
            return this.LocalRedirectPermanent($"/blog/{blogSlug}/{0}");
        }
        [Route("/blog/{blogSlug}/{page:int?}")]
        //[OutputCache(Profile = "default")]
        public async Task<IActionResult> Index([FromRoute] string blogSlug, int page = 0)
        {
            var blogIndexViewModel = new BlogIndexViewModel();
            int postCount = 0;
            if (blogSlug == "all")
                postCount = await unitOfWork.posts.get().CountAsync();
            else
                postCount = await unitOfWork.posts.get().Where(q => q.blog.blogSlug == blogSlug).CountAsync();

            blogIndexViewModel.blogSlug = blogSlug;
            blogIndexViewModel.currentPage = page + 1;
            blogIndexViewModel.skip = this.settings.Value.PostsPerPage * page;
            blogIndexViewModel.postsPerPage = this.settings.Value.PostsPerPage;
            blogIndexViewModel.search = null;
            blogIndexViewModel.totalPosts = postCount;
            this.ViewData[Constants.prev] = $"/blog/{blogSlug}/{page - 1}/";
            this.ViewData[Constants.next] = $"/blog/{blogSlug}/{page + 1}/";

            //PageResult
            var query = unitOfWork.posts.get();
            query = query.Include("blog").Include("blog.author").Include("comments")
                .Skip(this.settings.Value.PostsPerPage * page).Take(this.settings.Value.PostsPerPage);
            var posts = query.ToList();
            var pageResult = new PagedResult<Post>();
            pageResult.CurrentPage = page;
            pageResult.PageCount = (postCount / 2);
            pageResult.PageSize = 2;
            pageResult.RowCount = 1;
            pageResult.Results = posts;

            return View(pageResult);

            
        }
        public IActionResult LoadPosts(string blogSlug, int page)
        {
            var query = unitOfWork.posts.get();
            if (!string.IsNullOrWhiteSpace(blogSlug) && blogSlug != "all")
                query = query.Where(q => q.blog.blogSlug == blogSlug);

            query = query.Include("blog").Include("blog.author").Include("comments")
                .Skip(this.settings.Value.PostsPerPage * page).Take(this.settings.Value.PostsPerPage);

            return ViewComponent("PostRow");
        }
        [Route("/blog/edit/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!userService.permissionForBlogEditing(PermissionAction.Delete, userService.CurrentUserSettings.userId, false)) return RedirectToAction("AccessDenied", "Administration");
            //permission for create
            var currentUserSetting = userService.CurrentUserSettings;
            Blog blog = await unitOfWork.blogs.get().Where(q => q.authorId == currentUserSetting.userId).FirstOrDefaultAsync();
            if(blog == null)
            {
                blog = new Blog();
                blog.authorId = userService.CurrentUserSettings.userId;
                blog.createDt = DateTime.UtcNow;
                blog.lastUpdDt = DateTime.UtcNow;
            }
            Post post;
            if (id == 0)
            {
                post = new Post();
                post.blogId = blog.id;
                post.blog = blog;
            }
            else
            {
                post = await unitOfWork.posts.get(id).Include("blog").Include("blog.author").Include("comments").FirstOrDefaultAsync();
            }
            
            var vm = setupViewModel(post);
            return View(vm);
        }
        [Route("/blog/{blogSlug}/{slug?}")]
        public async Task<IActionResult> Post(string blogSlug, string slug)
        {
            var currentUser = userService.CurrentUserSettings;
            var post = await unitOfWork.posts.get()
                .Where(p => p.slug == slug)
                .Include("blog").Include("blog.author").Include("comments").FirstOrDefaultAsync();

            //return post is null ? this.NotFound() : (IActionResult)this.View(post);
            if (post == null)
                return NotFound();
            else
                
                return View(post);
        }
        /// <remarks>This is for redirecting potential existing URLs from the old Miniblog URL format.</remarks>
        [Route("/post/{slug}")]
        [HttpGet]
        public IActionResult Redirects(string slug)
        {
            
            return this.LocalRedirectPermanent($"/blog/{slug}");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePost(PostViewModel vModel)
        {
            if (!userService.permissionForBlogEditing(PermissionAction.Delete, userService.CurrentUserSettings.userId, false)) return RedirectToAction("AccessDenied", "Administration");
            //permission
            Blog blog = new Blog();
            var modelObj = new Post();
            var currentUserSetting = userService.CurrentUserSettings;
            //handle the blog first
            if (vModel.post.blogId == 0)
            {
                blog.authorId = currentUserSetting.userId;
                blog.createDt = DateTime.UtcNow;
                blog.lastUpdDt = DateTime.UtcNow;
                if (string.IsNullOrWhiteSpace(currentUserSetting.email))
                    throw new InvalidOperationException("Can't find user's email");
                blog.blogSlug = blogService.generateBlogSlug(currentUserSetting.email.Split("@")[0]);
                unitOfWork.blogs.add(blog);
                if (!(await unitOfWork.tryCommitAsync()))
                {
                    ModelState.AddModelError("", "Failed to create a blog");
                }
                
            }
            else
            {
                blog = await unitOfWork.blogs.get().Where(q => q.authorId == currentUserSetting.userId).FirstOrDefaultAsync();
                blog.authorId = userService.CurrentUserSettings.userId;
                blog.lastUpdDt = DateTime.UtcNow;
            }
            if (vModel.post.id == 0)
            {
                //new Post
                modelObj.blogId = blog.id;
                modelObj.blog = blog;
                modelObj.createDt = DateTime.UtcNow;
                modelObj.lastModified = DateTime.UtcNow;
                unitOfWork.posts.add(modelObj);
            }
            else
            {
                modelObj = await unitOfWork.posts.get(vModel.post.id).Include("blog").FirstOrDefaultAsync();
                modelObj.blog.lastUpdDt = DateTime.UtcNow;
                modelObj.lastModified = DateTime.UtcNow;
                unitOfWork.posts.update(modelObj);
            }
            acceptPost(modelObj, vModel);
            if (!ModelState.IsValid)
            {
                var vm = setupViewModel(modelObj);
                return View(vm);
            }
            await unitOfWork.commitAsync();
            return this.Redirect(modelObj.GetEncodedLink());
        }
        [Route("/blog/comment/{postId}")]
        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, Comment comment)
        {
            if (!userService.permissionForBlogEditing(PermissionAction.Delete, userService.CurrentUserSettings.userId, false)) return RedirectToAction("AccessDenied", "Administration");
            var currentUserSettings = userService.CurrentUserSettings;
            var post = await unitOfWork.posts.get(postId).Include("blog").Include("blog.author").Include("comments").FirstOrDefaultAsync();

            
            if (post is null || !post.AreCommentsOpen(this.settings.Value.CommentsCloseAfterDays))
            {
                return this.NotFound();
            }

            if (comment is null)
            {
                throw new ArgumentNullException(nameof(comment));
            }
            if (User.Identity.IsAuthenticated)
            {
                comment.authorId = currentUserSettings.userId;
                comment.name = currentUserSettings.fullName;
                comment.email = currentUserSettings.email;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(comment.name))
                    ModelState.AddModelError("name", "Required");
                if (string.IsNullOrWhiteSpace(comment.email))
                    ModelState.AddModelError("email", "Required");
            }
            if (!this.ModelState.IsValid)
            {
                return this.View(nameof(Post), post);
            }
           
            comment.postId = post.id;
            comment.post = post;
            comment.content = comment.content;
            

            // the website form key should have been removed by javascript unless the comment was
            // posted by a spam robot
            post.comments.Add(comment);
            await unitOfWork.commitAsync().ConfigureAwait(false);
            return this.Redirect($"{post.GetEncodedLink()}#{comment.id}");
        }
        [Route("/blog/deletepost/{id}")]
        [HttpPost, Authorize, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            if(!userService.permissionForBlogEditing(PermissionAction.Delete, userService.CurrentUserSettings.userId, false)) return RedirectToAction("AccessDenied", "Administration");

            var existing = await unitOfWork.posts.get(id).Include("blog").Include("blog.author").Include("comments").FirstOrDefaultAsync();
            if (existing is null)
            {
                return this.NotFound();
            }

            await blogService.DeletePost(existing).ConfigureAwait(false);
            return this.Redirect("/");
        }

        [Route("/blog/comment/{postId}/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int postId, int commentId)
        {
            var post = await blogService.GetPostById(postId).ConfigureAwait(false);

            if (post is null)
            {
                return this.NotFound();
            }

            var comment = post.comments.FirstOrDefault(c => c.id == commentId);

            if (comment is null)
            {
                return this.NotFound();
            }

            unitOfWork.comments.remove(comment);
            await unitOfWork.commitAsync();

            return this.Redirect($"{post.GetEncodedLink()}#comments");
        }
    }
}