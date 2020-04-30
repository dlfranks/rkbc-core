using Microsoft.AspNetCore.Mvc;
using rkbc.web.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.web.viewcomponents
{
    public class PagerViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(PagedResultBase result)
        {
            return Task.FromResult((IViewComponentResult)View("Default", result));
        }
    }
}
