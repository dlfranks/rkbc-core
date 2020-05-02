using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;

namespace rkbc.web.controllers
{
    public class MissionController : AppBaseController
    {
        public MissionController(IUnitOfWork _unitOfWork, UserService _userService) : base(_unitOfWork, _userService)
        {

        }
        public async Task<IActionResult> Index(int country)
        {

            var missionBlogs = unitOfWork.blogs.get().Where(q => q.author.accountType == (int)AccountType.Mission && q.author.countryCode == country).Include("posts").Include("author").AsAsyncEnumerable();

            return View(missionBlogs);
        }
        
    }
}