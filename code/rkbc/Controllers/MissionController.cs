using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.viewmodels;

namespace rkbc.web.controllers
{
    public class MissionController : AppBaseController
    {
        public MissionController(IUnitOfWork _unitOfWork, UserService _userService) : base(_unitOfWork, _userService)
        {

        }
        public async Task<IActionResult> Index(int country, int page = 1)
        {
            //PagedResult<Blog> pagedResult = new PagedResult<Blog>();
            //var missionBlogs = unitOfWork.posts.get().Where(q => q.blog.author.accountType == (int)AccountType.Mission && q.blog.author.countryCode == country).Include("blog").Include("author");
            //var result = await blogService.GetPagedResultForQuery(query, page, pageSize);

            return View();
        }
        
    }
}