using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElmahCore;
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
using rkbc.web.helpers;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{
    public class MissionNews
    {
        public int countryCode { get; set; }
        public int blogId { get; set; }
        public int postId { get; set; }
        public string authorName { get; set; }
        public string blogSlug { get; set; }
    }
    public abstract class LatestPostBase
    {
        public string authorName { get; set; }
        public int blogId { get; set; }
        public int postId { get; set; }
        public string postTitle { get; set; }

        public string postSlug { get; set; }
    }
    public class LatestPost : LatestPostBase { }
    public class LatestGalleryPost : LatestPostBase
    {
        public string imageFileName { get; set; }

    }
    public class LatestVideoPost : LatestPostBase
    {
        public string videoIframe { get; set; }

    }
    public class LatestSinglePost : LatestPostBase
    {
        public string singlePostContent { get; set; }

    }
    public class LatestCommentPost
    {
        public string authorName { get; set; }
        public string postTitle { get; set; }
        public int postId { get; set; }
        public string comment { get; set; }

    }
    public class BlogSideViewModel
    {
        public BlogSideViewModel()
        {
            //missionNews = new List<MissionNews>();
            //latestGalleryList = new List<LatestGalleryPost>();
            //latestVideList = new List<LatestVideoPost>();
            //latestSingleList = new List<LatestSinglePost>();
            //latestCommentList = new List<LatestCommentPost>();
        }
        public IAsyncEnumerable<MissionNews> missionNews { get; set; }
        public IAsyncEnumerable<LatestGalleryPost> latestGalleryList { get; set; }
        public IAsyncEnumerable<LatestVideoPost> latestVideList { get; set; }
        public IAsyncEnumerable<LatestSinglePost> latestSingleList { get; set; }
        public IAsyncEnumerable<LatestCommentPost> latestCommentList { get; set; }
        public IAsyncEnumerable<LatestPostBase> latestPostList { get; set; }
    }
    public class BlogIndexViewModel
    {
        PagedResult<Post> pagedResult { get; set; }
        BlogSideViewModel blogSideViewModel { get; set; }

    }
    public class PostViewModel
    {
        public Post post { get; set; }
        public IFormFile image { get; set; }
        public string embededVideoUrl { get; set; }
        public string imageUrl { get; set; }
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
        protected void acceptImage(Post modelObj, IFormFile file, out List<string> errmsg)
        {
            errmsg = new List<string>();
            
            // Verify we have an image
            if (file == null)
            {
                errmsg.Add("No file was chosen for an attached image, please select a file!");
                return;
            }
            if (file.Length == 0)
            {
                errmsg.Add("No file was chosen for an attached image, please select a file!");
                return;
            }
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
                errmsg.Add(msg);
                ModelState.AddModelError("image", msg);
                HttpContext.RiseError(new InvalidOperationException(msg));
            }
            try
            {
                //Adjust image size based on selection width 600
                bitmap = ImageHelper.ScaleImage(bitmap, BlogImageWidthConstants.FullSizeWidth, null);
                ImageHelper.saveJpegImage(bitmap, assetFileAndPathName, 75L);
                ImageHelper.GenerateThumbnail(bitmap, 150, assetFileAndPathName);
                //Thumbnail width 150;

            }
            catch (Exception e)
            {
                var msg = "Internal error, unable to save the image.";
                errmsg.Add(msg);
                ModelState.AddModelError("image", msg);
                HttpContext.RiseError(new InvalidOperationException(msg));
                //ElmahCore
            }
            
            modelObj.imageFileName = assetFileName;
        }
        protected void acceptPost(Post modelObj, PostViewModel vModel)
        {
            modelObj.postType = vModel.post.postType;
            modelObj.title = vModel.post.title;
            modelObj.excerpt = vModel.post.excerpt;
            modelObj.content = vModel.post.content;
            modelObj.videoURL = vModel.post.videoURL;
            if (modelObj.isPublished)
                modelObj.pubDate = DateTime.UtcNow;
            modelObj.videoURL = vModel.post.videoURL;
            var errmsg = new List<string>();
            if (ModelState.IsValid)
            {
                if (vModel.image != null && vModel.image.Length > 0)
                {
                    List<string> fileerrmsg;
                    acceptImage(modelObj, vModel.image, out fileerrmsg);
                    foreach (var msg in fileerrmsg) ModelState.AddModelError("", msg);
                }
                else
                {
                    if(modelObj.postType == (int)BlogPostType.Gallery && modelObj.id == 0)
                        ModelState.AddModelError("", "No file was chosen for an attached image, please select a file!");
                }
            }
            
            
        }
        protected PostViewModel setupViewModel(Post model)
        {
            PostViewModel vm = new PostViewModel();
            vm.post = model;
            vm.embededVideoUrl = fileHelper.youtubeEmbedUrl(model.videoURL) == null ? "" : fileHelper.youtubeEmbedUrl(model.videoURL);
            vm.imageUrl = fileHelper.generateAssetURL("blog", model.imageFileName);
            
            return vm;
        }
       public async Task<IActionResult> AllIndex(int page = 1)
        {
            var query = unitOfWork.posts.get();
            query = query.Include("blog").Include("blog.author").Include("comments").OrderByDescending(q => q.lastModified);
            int pageSize = settings.Value.PostsPerPage;
            var result = await blogService.GetPagedResultForQuery(query, page, pageSize);
            ViewBag.title = "Blog";
            return View("Index", result);
        }
        [Route("/Blog/Index/{userId}")]
        [Route("/Blog/{userId}")]
        public async Task<IActionResult> Index(string userId, int page=1)
        {
            var query = unitOfWork.posts.get();
            query = query.Where(q => q.blog.authorId == userId);
            query = query.Include("blog").Include("blog.author").Include("comments").OrderByDescending(q => q.lastModified);
            int pageSize = settings.Value.PostsPerPage;
            var result = await blogService.GetPagedResultForQuery(query, page, pageSize);
            ViewBag.title = "My Blog";
            return View(result);
        }
        [Route("/Mission/Index/{country}")]
        public async Task<IActionResult> MissionIndex(string country, int page = 1)
        {
            CountryEnum value;
            if(!(Enum.TryParse(country.ToString(), true, out value)))
            {
                value = CountryEnum.Dominican_Republic;
            }
           
            var query = unitOfWork.posts.get();
            query = query.Where(q => q.blog.author.countryCode == (int)value && q.blog.author.accountType == (int)AccountType.Mission);
            query = query.Include("blog").Include("blog.author").Include("comments").OrderByDescending(q => q.lastModified);
            int pageSize = settings.Value.PostsPerPage;
            var result = await blogService.GetPagedResultForQuery(query, page, pageSize);
            ViewBag.CountryName = EnumHelper.GetDisplayName<CountryEnum>(value);
            ViewBag.CountryValue = country;
            return View("Mission", result);
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
                blog.blogSlug = await blogService.generateBlogSlug();
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
        [HttpGet]
        public async Task<IActionResult> Post(int postId)
        {
            var currentUser = userService.CurrentUserSettings;
            var post = await unitOfWork.posts.get()
                .Where(p => p.id == postId)
                .Include("blog").Include("blog.author").Include("comments").FirstOrDefaultAsync();

            //return post is null ? this.NotFound() : (IActionResult)this.View(post);
            if (post == null)
                return NotFound();
            else

                return View(post);
            
        }

        /// <remarks>This is for redirecting potential existing URLs from the old Miniblog URL format.</remarks>
        //[Route("/post/{slug}")]
        //[HttpGet]
        //public IActionResult Redirects(string slug)
        //{
            
        //    return this.LocalRedirectPermanent($"/blog/{slug}");
        //}

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
                blog.blogSlug = await blogService.generateBlogSlug();
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
                modelObj.slug = await blogService.generateSlug(blog.id, blog.blogSlug);
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
                return View("Edit", vm);
            }
            await unitOfWork.commitAsync();
            
            return RedirectToAction("Post", new {postid = modelObj.id});
        }
        
        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, Comment comment)
        {
            if (!userService.permissionForBlogEditing(PermissionAction.Delete, userService.CurrentUserSettings.userId, false)) return RedirectToAction("AccessDenied", "Administration");
            var currentUserSettings = userService.CurrentUserSettings;
            var post = await unitOfWork.posts.get(postId).Include("blog").Include("blog.author").Include("comments").FirstOrDefaultAsync();

            
            //if (post is null || !post.AreCommentsOpen(this.settings.Value.CommentsCloseAfterDays))
            if(post is null)
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
            return RedirectToAction("Post", new { postid = post.id });
        }
        
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = userService.CurrentUserSettings.userId;
            if (!userService.permissionForBlogEditing(PermissionAction.Delete, userId, false)) return RedirectToAction("AccessDenied", "Administration");

            var existing = await unitOfWork.posts.get(id).Include("blog").Include("blog.author").Include("comments").FirstOrDefaultAsync();
            if (existing is null)
            {
                return this.NotFound();
            }

            await blogService.DeletePost(existing).ConfigureAwait(false);
            return RedirectToAction("Index", new {userId= userId });
        }

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