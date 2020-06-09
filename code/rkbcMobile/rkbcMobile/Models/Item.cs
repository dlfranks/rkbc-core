using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace rkbcMobile.Models
{
    public enum BlogPostType
    {
        Sigle = 1000,
        Gallery,
        Video

    }
    public class CommmentItem
    {
        public int commentId { get; set; }
        public int postId { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string content { get; set; }
    }
    public class Item
    {
        public string Id { get; set; }
        public int postType { get; set; }
        public string imageUrl { get; set; }
        public string videoURL { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string getVideoId()
        {
            var videoId = "";
            if ((postType == (int)BlogPostType.Video) && !String.IsNullOrWhiteSpace(this.videoURL))
            {
                videoId = Regex.Match(this.videoURL, "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+").Groups[1].Value;
            }
            return videoId;
        }
    }
    public class PostItem
    {
        public int postId { get; set; }
        public int blogId { get; set; }
        public string imageFileName { get; set; }
        public string videoURL { get; set; }
        public string content { get; set; }
        public string excerpt { get; set; }
        public int postType { get; set; }
        public bool isPublished { get; set; } = true;
        public DateTime createDt { get; set; }
        public DateTime lastModified { get; set; }
        public DateTime pubDate { get; set; }
        public string slug { get; set; }
        public int views { get; set; }
        public string title { get; set; }
        public IList<CommmentItem> comments { get; set; }

        public string getVideoId()
        {
            var videoId = "";
            if ((postType == (int)BlogPostType.Video) && !String.IsNullOrWhiteSpace(this.videoURL))
            {
                videoId = Regex.Match(this.videoURL, "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+").Groups[1].Value;
            }
            return videoId;
        }

        public bool isGalleryType()
        {
            return postType == (int)BlogPostType.Gallery ? true : false;
        }
        public bool isVideoType()
        {
            return postType == (int)BlogPostType.Video ? true : false;
        }
        public bool isSigleType()
        {
            return postType == (int)BlogPostType.Sigle ? true : false;
        }
    }
}