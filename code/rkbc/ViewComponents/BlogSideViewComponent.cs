using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.web.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.web.viewcomponents
{
    public class BlogSideViewComponent : ViewComponent
    {
        private IUnitOfWork unitOfWork;
        public BlogSideViewComponent(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            BlogSideViewModel model = new BlogSideViewModel();
            

            var latestGalleryBlogs = unitOfWork.posts.get().Where(q => q.postType == (int)BlogPostType.Gallery).OrderByDescending(q => q.lastModified)
                .Select(q => new LatestGalleryPost() {
                    authorName = q.blog.author.firstName + " " + q.blog.author.lastName,
                    blogId = q.blogId,
                    postId = q.id,
                    postTitle = q.title,
                    postSlug = q.slug,
                    imageFileName = q.imageFileName
                }).ToListAsync();

            var latestVideoBlogs = unitOfWork.posts.get().Where(q => q.postType == (int)BlogPostType.Video).OrderByDescending(q => q.lastModified)
                .Select(q => new LatestVideoPost() {
                    authorName = q.blog.author.firstName + " " + q.blog.author.lastName,
                    blogId = q.blogId,
                    postId = q.id,
                    postTitle = q.title,
                    postSlug = q.slug,
                    videoIframe = q.getEmbededVideo()
                }).ToListAsync();

            var latestSingleBlogs = unitOfWork.posts.get().Where(q => q.postType == (int)BlogPostType.Sigle).OrderByDescending(q => q.lastModified)
                .Select(q => new LatestSinglePost() {
                    authorName = q.blog.author.firstName + " " + q.blog.author.lastName,
                    blogId = q.blogId,
                    postId = q.id,
                    postTitle = q.title,
                    postSlug = q.slug,
                    singlePostContent = q.RenderContent()
                }).ToListAsync();

            var latestCommentBlogs = unitOfWork.comments.get().OrderByDescending(q => q.pubDate).OrderByDescending(q => q.pubDate)
                .Select(q => new LatestCommentPost()
                {
                    authorName = q.author.firstName + " " + q.author.lastName,
                    postId = q.id,
                    postTitle = q.post.title,
                    comment = q.content
                }).ToListAsync();

            return View("BlogSide");
        }
    }
}
