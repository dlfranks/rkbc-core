﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace rkbc.Controllers
{
    public class PastorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}