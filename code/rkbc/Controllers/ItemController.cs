using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rkbc.core.models;
using rkbc.core.repository;
using Microsoft.EntityFrameworkCore;
using rkbc.web.controllers;
using rkbc.core.service;
using System.Text.Json;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{
    public class CommmentMobileViewModel
    {
        public int commentId { get; set; }
        public int postId { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string content { get; set; } = string.Empty;
    }
    public class PostMobileViewModel {
        public int postId { get; set; }
        public int blogId { get; set; }
        public string imageFileName { get; set; }
        public string videoURL { get; set; }
        public string content { get; set; }
        public string excerpt { get; set; }
        public int postType { get; set; }
        public bool isPublished { get; set; } = true;
        public DateTime createDt { get; set; }
        public DateTime lastModified { get; set; }
        public DateTime pubDate { get; set; }
        public string slug { get; set; }
        public int views { get; set; }
        public string title { get; set; }
        public IList<CommmentMobileViewModel> comments { get; set; }
    }
}
namespace rkbc.web.Controllers
{
    
    public class ItemController : AppBaseController
    {
        private readonly IItemRepository ItemRepository;
        private IUrlHelper urlHelper;

        public ItemController(IItemRepository itemRepository, 
            UserService _userService, 
            IUnitOfWork _unitOfWork,
            IUrlHelper _urlHelper) : base(_unitOfWork, _userService)
        {
            ItemRepository = itemRepository;
            urlHelper = _urlHelper;
        }

        [HttpGet]
        public async Task<JsonResult> getList()
        {
            
            var domain = urlHelper.DomainName();
            var posts = await unitOfWork.posts.get().Select(q => new PostMobileViewModel()
            {
                postId = q.id,
                blogId = q.blogId,
                imageFileName = q.imageFileName,
                videoURL = q.videoURL,
                content = q.content,
                excerpt = q.excerpt,
                postType = q.postType,
                isPublished = q.isPublished,
                createDt = q.createDt,
                lastModified = q.lastModified,
                pubDate = q.pubDate,
                slug = q.slug,
                views = q.views,
                title = q.title,
                comments = q.comments.Select(c => new CommmentMobileViewModel()
                {
                    commentId = c.id,
                    postId = c.postId,
                    authorId = c.authorId,
                    authorName = c.author.firstName + " " + c.author.lastName,
                    content = c.content
                }).ToList()

            }).ToListAsync();
            //var posts = await unitOfWork.posts.get().Select(q => new
            //{
            //    Id = q.id,
            //    Text = q.title,
            //    imageUrl = q.getImageLink(),
            //    videoURL = q.videoURL,
            //    postType = q.postType,
            //    Description = q.content
            //}).ToListAsync();
            //var jsonStr = JsonSerializer.Serialize(posts);
           return Json(posts);
        }

        
    }
}
