using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rkbc.core.repository;
using rkbc.core.service;

namespace rkbc.web.controllers
{ 
    public class SchoolController : AppBaseController
    {
        public SchoolController(IUnitOfWork _unitOfWork, UserService _userService) : base(_unitOfWork, _userService)
        {

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}