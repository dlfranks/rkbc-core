using rkbc.core.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.web.viewmodels
{
    public abstract class PagedResultBase
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstRowOnPage
        {
            get { return (CurrentPage - 1) * PageSize + 1; }
        }
        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }

    public class PagedResult<T> : PagedResultBase where T : class
    {
        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }
    
    public class MissionNews
    {
        public string missionCountry { get; set; }
        public int blogId { get; set; }
        
        public string authorName { get; set; }
        public string blogSlug { get; set; }
    }
    public abstract class LatestPostBase
    {
        public string authorName { get; set; }
        public int blogId { get; set; }
        public int postId { get; set; }
        public string postTitle { get; set; }
        
        public string postSlug { get; set; }
    }
    public class LatestGalleryPost : LatestPostBase
    {
        public string imageFileName { get; set; }
        
    }
    public class LatestVideoPost : LatestPostBase
    {
        public string videoIframe { get; set; }

    }
    public class LatestSinglePost : LatestPostBase
    {
        public string singlePostContent { get; set; }

    }
    public class LatestCommentPost
    {
        public string authorName { get; set; }
        public string postTitle { get; set; }
        public int postId { get; set; }
        public string comment { get; set; }
        
    }
    public class BlogSideViewModel
    {
        public BlogSideViewModel()
        {
            missionNews = new List<MissionNews>();
            latestGalleryList = new List<LatestGalleryPost>();
            latestVideList = new List<LatestVideoPost>();
            latestSingleList = new List<LatestSinglePost>();
            latestCommentList = new List<LatestCommentPost>();
        }
        public List<MissionNews> missionNews { get; set; }
        public List<LatestGalleryPost> latestGalleryList { get; set; }
        public List<LatestVideoPost> latestVideList { get; set; }
        public List<LatestSinglePost> latestSingleList { get; set; }
        public List<LatestCommentPost> latestCommentList { get; set; }
    }
}
