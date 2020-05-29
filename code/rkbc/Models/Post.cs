using Microsoft.Extensions.Localization;
using rkbc.core.repository;
using rkbc.web.constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace rkbc.core.models
{
    public static class BlogImageWidthConstants
    {
        public const int ThumbnailWidth = 150;
        public const int ThumbnailWidthLandscape = 200;
        public const int SmallWidth = 600;
        public const int FullSizeWidth = 1200;
    }
    public enum PostListView
    {
        TitlesOnly,

        TitlesAndExcerpts,

        FullPosts
    }
    public enum BlogPostType
    {
        Sigle = 1000,
        Gallery,
        Video

    }
    public class Blog : IEntity
    {
        public int id { get; set; }
        public string authorId { get; set; }
        public virtual ApplicationUser author { get; set; }
        public DateTime createDt { get; set; } 
        public DateTime lastUpdDt { get; set; }
        public string blogSlug { get; set; }
        public virtual ICollection<Post> posts {get; set;}
    }
    public class Post : IEntity
    {
        //private readonly IStringLocalizer<Post> localizer;
        //public Post(IStringLocalizer<Post> _localizer)
        //{
        //    localizer = _localizer;
        //}
        [Required]
        public int id { get; set; }
        [Required]
        public int blogId { get; set; }
        public virtual Blog blog {get; set;}
        public string imageFileName { get; set; }
        public string videoURL { get; set; }
        
        [Display(Name = "Content")]
        public string content { get; set; } = string.Empty;

        [Display(Name = "Excerpt")]
        public string excerpt { get; set; } = string.Empty;

        [Required]
        [Display(Name="Post Type")]
        public int postType { get; set; } = (int)BlogPostType.Sigle;

        [Display(Name = "Is Published?")]
        public bool isPublished { get; set; } = true;
        public DateTime createDt { get; set; }
        public DateTime lastModified { get; set; } = DateTime.UtcNow;

        public DateTime pubDate { get; set; } = DateTime.UtcNow;
        public string slug { get; set; } = string.Empty;
        public int views { get; set; } = 0;
        [Required]
        [Display(Name = "Subject")]
        public string title { get; set; } = string.Empty;

        public IList<Comment> comments { get; } = new List<Comment>();

        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "The slug should be lower case.")]
        public static string CreateSlug(string title)
        {
            title = title?.ToLowerInvariant().Replace(
                Constants.Space, Constants.Dash, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
            title = RemoveDiacritics(title);
            title = RemoveReservedUrlCharacters(title);

            return title.ToLowerInvariant();
        }

        public bool AreCommentsOpen(int commentsCloseAfterDays) =>
            this.pubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;

        public string GetEncodedLink()
        {

            return $"/blog/{this.blog.blogSlug}/{System.Net.WebUtility.UrlEncode(this.slug)}/";
        }

        public string GetLink() => $"/blog/{this.blog.blogSlug}/{this.slug}/";

        public bool IsVisible() => this.pubDate <= DateTime.UtcNow && this.isPublished;

        public string RenderContent()
        {
            var result = this.content;

            // Set up lazy loading of images/iframes
            if (!string.IsNullOrEmpty(result))
            {
                // Set up lazy loading of images/iframes
                //var replacement = " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
                //var pattern = "(<img.*?)(src=[\\\"|'])(?<src>.*?)([\\\"|'].*?[/]?>)";
                //result = Regex.Replace(result, pattern, m => m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value);

                // Youtube content embedded using this syntax: [youtube:xyzAbc123]
                //var video = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
                //result = Regex.Replace(
                //    result,
                //    @"\[youtube:(.*?)\]",
                //    m => string.Format(CultureInfo.InvariantCulture, video, m.Groups[1].Value));
            }

            return result;
        }
        public string getImageLink()
        {
           
            if (string.IsNullOrEmpty(this.imageFileName)) return "";
            else return "/assets/blog/" + this.imageFileName;
        }
        protected string youtubeEmbedUrl()
        {
            string videoId = "";
            if (!String.IsNullOrWhiteSpace(this.videoURL))
            {
                videoId = Regex.Match(this.videoURL, "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+").Groups[1].Value;
            }
            return ("https://www.youtube.com/embed/" + videoId);
        }
        public string getEmbededVideo()
        {
            var video = "<div class=\"video\"><iframe style=\"width:100%;\" title=\"YouTube embed\" src=\"" + youtubeEmbedUrl() + "\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
            return video;
        }
        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private static string RemoveReservedUrlCharacters(string text)
        {
            var reservedCharacters = new List<string> { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

            foreach (var chr in reservedCharacters)
            {
                text = text.Replace(chr, string.Empty, StringComparison.OrdinalIgnoreCase);
            }

            return text;
        }
    }
}
