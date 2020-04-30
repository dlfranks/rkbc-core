using rkbc.core.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.config.models
{
    public class BlogSettings
    {
        public int CommentsCloseAfterDays { get; set; } = 10;

        public PostListView ListView { get; set; } = PostListView.TitlesAndExcerpts;

        //public string Owner { get; set; } = "The Owner";

        public int PostsPerPage { get; set; } = 4;
    }
    public class EmailSettings
    {
        public String PrimaryDomain { get; set; }

        public int PrimaryPort { get; set; }

        public String SecondayDomain { get; set; }

        public int SecondaryPort { get; set; }

        public String UsernameEmail { get; set; }

        public String UsernamePassword { get; set; }

        public String FromEmail { get; set; }

        public String ToEmail { get; set; }

        public String CcEmail { get; set; }
    }
    public class RkbcConfig
    {
        public int HomePageId { get; set; }
        public int PastorPageId { get; set; }
        public string Version { get; set; }
        public string DefaultPassword { get; set; }
    }
}