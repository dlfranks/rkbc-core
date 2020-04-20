using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.models
{
    public enum BlogPostType
    {
        Gallary = 1000,
        Video,
        Sigle

    }
    public class blog
    {
        public int id { get; set; }
        public int postEnum { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string videoLink { get; set; }
        

    }
}
