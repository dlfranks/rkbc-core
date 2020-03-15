using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.extensions;

namespace rkbc.web.controllers
{
    public abstract class AppBaseController : Controller
    {
        protected IUnitOfWork unitOfWork;
        protected UserService userService;
        public AppBaseController(IUnitOfWork _unitOfWork, UserService _userService)
        {
            this.unitOfWork = _unitOfWork;
            this.userService = _userService;
        }
        private string getPagePrefName(string key)
        {
            var ctl = (string)ControllerContext.RouteData.Values["Controller"];
            if (String.IsNullOrEmpty(key)) key = "Preferences";
            return (ctl + "_" + key);
        }
        protected void savePagePref<T>(T pageSettings, string key = null)
        {
            HttpContext.Session.SetObject(getPagePrefName(key), pageSettings);
        }
        protected T loadPagePref<T>(string key = null, Func<T> defaultPref = null) where T : new()
        {
            T vmodel;
            var str = getPagePrefName(key);
            var value = HttpContext.Session.GetObject<T>(str);
            if (value != null)
                vmodel = (T)value;
            else
            {
                if (defaultPref == null)
                    vmodel = new T();
                else
                    vmodel = defaultPref.Invoke();
            }
            return (vmodel);
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ViewBag.UserSettings = userService.CurrentUserSettings;
            ViewBag.userService = userService;
        }
    }
}