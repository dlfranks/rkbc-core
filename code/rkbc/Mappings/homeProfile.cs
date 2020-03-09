using AutoMapper;
using rkbc.core.models;
using rkbc.web.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.map.models
{
    public class HomeProfile: Profile
    {
        public HomeProfile()
        {
            CreateMap<HomePage, HomePageViewModel>().ReverseMap();
        }
    }
}
