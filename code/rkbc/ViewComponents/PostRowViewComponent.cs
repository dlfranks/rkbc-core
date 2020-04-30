using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rkbc.core.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.web.viewcomponents
{
    public class PostRowViewComponent : ViewComponent
    {
        private IUnitOfWork unitOfWork;
        public PostRowViewComponent(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IViewComponentResult Invoke(string blogSlug, int skip, int postsPerPage)
        {
            var query = unitOfWork.posts.get();
            if(!string.IsNullOrWhiteSpace(blogSlug) && blogSlug != "all")
                query = query.Where(q => q.blog.blogSlug == blogSlug);

            var posts = query.Include("blog").Include("blog.author").Include("comments")
                .Skip(skip).Take(postsPerPage).ToListAsync();
            return View("PostRow", posts);
        }
    }
}
