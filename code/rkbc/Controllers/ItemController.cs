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

namespace rkbc.web.Controllers
{
    
    public class ItemController : AppBaseController
    {
        private readonly IItemRepository ItemRepository;
        

        public ItemController(IItemRepository itemRepository, UserService _userService, IUnitOfWork _unitOfWork) : base(_unitOfWork, _userService)
        {
            ItemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<JsonResult> getList()
        {
            var posts = await unitOfWork.posts.get().Select(q => new
            {
                Id = q.id,
                Text = q.title,
                Description = q.content
            }).ToListAsync();
            var items = ItemRepository.GetAll();
            var lst = items.Select(q => new
            {
                Id = q.Id,
                Text = q.Text,
                Description = q.Description
            }).ToList();
            return Json(posts);
        }

        
    }
}
