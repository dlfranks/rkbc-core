using Microsoft.AspNetCore.Mvc;
using rkbc.core.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.web.viewcomponents
{
    public class BlogIndexViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(List<Post> result)
        {
            return Task.FromResult((IViewComponentResult)View("BlogIndex", result));
        }
    }
}
