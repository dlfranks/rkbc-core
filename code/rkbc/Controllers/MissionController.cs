using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rkbc.core.repository;
using rkbc.core.service;

namespace rkbc.web.controllers
{
    public class MissionController : AppBaseController
    {
        public MissionController(IUnitOfWork _unitOfWork, UserService _userService) : base(_unitOfWork, _userService)
        {

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}