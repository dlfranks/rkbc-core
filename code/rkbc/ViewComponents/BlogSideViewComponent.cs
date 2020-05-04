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

            model.missionNews = unitOfWork.blogs.get().Where(q => q.author.accountType == (int)AccountType.Mission).OrderByDescending(q => q.lastUpdDt)
                .Select(q => new MissionNews()
                {
                    authorName = q.author.firstName + " " + q.author.lastName,
                    blogId = q.id,
                    countryCode = (int)q.author.countryCode
                    
                }).AsAsyncEnumerable();

            model.latestPostList = unitOfWork.posts.get().OrderByDescending(q => q.lastModified)
                .Select(q => new LatestPost()
                {
                    authorName = q.blog.author.firstName + " " + q.blog.author.lastName,
                    blogId = q.blogId,
                    postId = q.id,
                    postTitle = q.title,
                    postSlug = q.slug

                }).AsAsyncEnumerable();


            model.latestGalleryList = unitOfWork.posts.get().Where(q => q.postType == (int)BlogPostType.Gallery).OrderByDescending(q => q.lastModified)
                .Select(q => new LatestGalleryPost() {
                    authorName = q.blog.author.firstName + " " + q.blog.author.lastName,
                    blogId = q.blogId,
                    postId = q.id,
                    postTitle = q.title,
                    postSlug = q.slug,
                    imageFileName = q.imageFileName
                }).AsAsyncEnumerable();

            model.latestVideList = unitOfWork.posts.get().Where(q => q.postType == (int)BlogPostType.Video).OrderByDescending(q => q.lastModified)
                .Select(q => new LatestVideoPost() {
                    authorName = q.blog.author.firstName + " " + q.blog.author.lastName,
                    blogId = q.blogId,
                    postId = q.id,
                    postTitle = q.title,
                    postSlug = q.slug,
                    videoIframe = q.getEmbededVideo()
                }).AsAsyncEnumerable();

            model.latestSingleList = unitOfWork.posts.get().Where(q => q.postType == (int)BlogPostType.Sigle).OrderByDescending(q => q.lastModified)
                .Select(q => new LatestSinglePost() {
                    authorName = q.blog.author.firstName + " " + q.blog.author.lastName,
                    blogId = q.blogId,
                    postId = q.id,
                    postTitle = q.title,
                    postSlug = q.slug,
                    singlePostContent = q.RenderContent()
                }).AsAsyncEnumerable();

            model.latestCommentList = unitOfWork.comments.get().OrderByDescending(q => q.pubDate)
                .Select(q => new LatestCommentPost()
                {
                    authorName = q.author.firstName + " " + q.author.lastName,
                    postId = q.postId,
                    postTitle = q.post.title,
                    comment = q.content
                }).AsAsyncEnumerable();


            return View("BlogSide", model);
        }
    }
}
