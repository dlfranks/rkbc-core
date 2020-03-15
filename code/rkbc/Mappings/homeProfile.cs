using AutoMapper;
using rkbc.core.models;
using rkbc.web.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.map.models
{
    public class AutoMapping: Profile
    {
        public AutoMapping()
        {
            CreateMap<HomePageViewModel, HomePage>();
        }
    }
}
