﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using rkbc.core.models;
using rkbc.core.repository;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using rkbc.core.helper;
using Microsoft.EntityFrameworkCore;
using rkbc.web.viewmodels;

namespace rkbc.core.service
{
    public class BlogService
    {
        private readonly IHttpContextAccessor contextAccessor;
        private FileHelper fileHelper;
        private IUnitOfWork unitOfWork;
        private UserService userService;

        [SuppressMessage(
                "Usage",
                "SecurityIntelliSenseCS:MS Security rules violation",
                Justification = "Path not derived from user input.")]
        public BlogService(IUnitOfWork _unitOfWork, IWebHostEnvironment env,
                IHttpContextAccessor contextAccessor, FileHelper _fileHelper, UserService _userService)
        {
            if (env is null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            this.contextAccessor = contextAccessor;
            this.unitOfWork = _unitOfWork;
            this.fileHelper = _fileHelper;
            this.userService = _userService;
        }
        public async Task<string> generateBlogSlug(string blogSlug = null)
        {
            string userName = userService.CurrentUserSettings.email.Split('@')[0];
            bool hasSlug = true;
            if (string.IsNullOrWhiteSpace(blogSlug)) blogSlug = userName;
            int count = 1;
            while (hasSlug)
            {
                hasSlug = await unitOfWork.blogs.get().AnyAsync(q => q.blogSlug.Trim().ToLower() == blogSlug.Trim().ToLower());
                if (hasSlug)
                {
                    blogSlug = blogSlug + count.ToString();
                    count++;
                }
                
            }
            return blogSlug.Trim().ToLower();
            
        }
        public async Task<string> generateSlug(int blogId, string slug)
        {
            int endChar = 7;
            bool hasSlug = true;
            var title = slug;
            if (title.Length < 7) endChar = title.Length;
            
            if (String.IsNullOrEmpty(slug)) slug = title.Substring(0, endChar);

            int count = 1;
            while (hasSlug)
            {
                hasSlug = await unitOfWork.posts.get().Where(q => q.blogId == blogId).AnyAsync(q => q.slug.Trim().ToLower() == slug.Trim().ToLower());
                if (hasSlug)
                {
                    slug = slug + count.ToString();
                    count++;
                }
                
            }
            return slug.Trim().ToLower();

        }
        public async Task DeletePost(Post post)
        {
            if (post is null)
            {
                throw new ArgumentNullException(nameof(post));
            }
            if (post.postType == (int)BlogPostType.Gallery)
                fileHelper.deleteAsset("blog", post.imageFileName, false);

            unitOfWork.posts.remove(post);
            try
            {
                await unitOfWork.commitAsync();
            }
            catch (Exception ex)
            {
               
                throw new InvalidOperationException("Attempted to delete an post that does not exist.", ex);
            }
        }
        public async Task<PagedResult<Post>> GetPagedResultForQuery(IQueryable<Post> query, int page, int pageSize)
        {
            var result = new PagedResult<Post>();

            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            var skip = (page - 1) * pageSize;
            result.Results = await query.Skip(skip).Take(pageSize).ToListAsync();

            return result;
        }

        //[SuppressMessage(
        //    "Globalization",
        //    "CA1308:Normalize strings to uppercase",
        //    Justification = "Consumer preference.")]
        //public virtual IAsyncEnumerable<string> GetCategories()
        //{
        //    var isAdmin = this.IsAdmin();

        //    return this.cache
        //        .Where(p => p.IsPublished || isAdmin)
        //        .SelectMany(post => post.Categories)
        //        .Select(cat => cat.ToLowerInvariant())
        //        .Distinct()
        //        .ToAsyncEnumerable();
        //}

        /// <remarks>Overload for getPosts method to retrieve all posts.</remarks>
        public virtual IAsyncEnumerable<Post> GetPosts(int count, int skip = 0)
        {
           
            var posts = unitOfWork.posts.get()
                .Include("blog").Include("blog.author").Include("comments")
                .Skip(skip).Take(count)
                .AsAsyncEnumerable();

            return posts;
        }
        public virtual IAsyncEnumerable<Post> GetPosts(string userId, int count, int skip = 0)
        {
            var posts = unitOfWork.posts.get().Where(q => q.blog.authorId == userId)
                .Skip(skip).Take(count)
                .Include("blog").Include("blog.author").Include("comments")
                .AsAsyncEnumerable();
            //var context = unitOfWork.getContext();
            //var posts = context.Posts
            //    Where(q => q.blog.authorId == userId)
            //    .Include("blog").Include("blog.author").Include("comments")
            //.Skip(skip).Take(count).AsAsyncEnumerable();

            return posts;
        }

        public virtual async Task<Post> GetPostById(int id)
        {
            var isUser = this.isUser();
            var post = await unitOfWork.posts.get(id).Include("blog").Include("author").Include("comments").FirstOrDefaultAsync();

            return await Task.FromResult(
                post is null || post.pubDate > DateTime.UtcNow || (!post.isPublished && !isUser)
                ? null
                : post).ConfigureAwait(true);
        }

        public virtual async Task<Post> GetPostBySlug(string slug)
        {
            var isUser = this.isUser();
            var post = await unitOfWork.posts.get().Where(p => p.slug.Equals(slug, StringComparison.OrdinalIgnoreCase)).Include("blog").Include("author").Include("comments").FirstOrDefaultAsync();

            return await Task.FromResult(
                post is null || post.pubDate> DateTime.UtcNow || (!post.isPublished && !isUser)
                ? null
                : post);
        }

        ///// <remarks>Overload for getPosts method to retrieve all posts.</remarks>
        //public virtual async IAsyncEnumerable<Post> GetPosts()
        //{
        //    var isUser = this.isUser();

        //    var posts = await unitOfWork.posts.get().Include("blog").Include("author").Include("comments").ToListAsync();

        //    return posts;
        //}

        //public virtual IAsyncEnumerable<Post> GetPosts(int count, int skip = 0)
        //{
        //    var isAdmin = this.IsAdmin();

        //    var posts = this.cache
        //        .Where(p => p.PubDate <= DateTime.UtcNow && (p.IsPublished || isAdmin))
        //        .Skip(skip)
        //        .Take(count)
        //        .ToAsyncEnumerable();

        //    return posts;
        //}

        //public virtual IAsyncEnumerable<Post> GetPostsByCategory(string category)
        //{
        //    var isAdmin = this.IsAdmin();

        //    var posts = from p in this.cache
        //                where p.PubDate <= DateTime.UtcNow && (p.IsPublished || isAdmin)
        //                where p.Categories.Contains(category, StringComparer.OrdinalIgnoreCase)
        //                select p;

        //    return posts.ToAsyncEnumerable();
        //}

        //[SuppressMessage(
        //    "Usage",
        //    "SecurityIntelliSenseCS:MS Security rules violation",
        //    Justification = "Caller must review file name.")]
        //public async Task<string> SaveFile(byte[] bytes, string fileName, string? suffix = null)
        //{
        //    if (bytes is null)
        //    {
        //        throw new ArgumentNullException(nameof(bytes));
        //    }

        //    suffix = CleanFromInvalidChars(suffix ?? DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));

        //    var ext = Path.GetExtension(fileName);
        //    var name = CleanFromInvalidChars(Path.GetFileNameWithoutExtension(fileName));

        //    var fileNameWithSuffix = $"{name}_{suffix}{ext}";

        //    var absolute = Path.Combine(this.folder, FILES, fileNameWithSuffix);
        //    var dir = Path.GetDirectoryName(absolute);

        //    Directory.CreateDirectory(dir);
        //    using (var writer = new FileStream(absolute, FileMode.CreateNew))
        //    {
        //        await writer.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        //    }

        //    return $"/{POSTS}/{FILES}/{fileNameWithSuffix}";
        //}

        //public async Task SavePost(Post post)
        //{
        //    if (post is null)
        //    {
        //        throw new ArgumentNullException(nameof(post));
        //    }

        //    var filePath = this.GetFilePath(post);
        //    post.LastModified = DateTime.UtcNow;

        //    var doc = new XDocument(
        //                    new XElement("post",
        //                        new XElement("title", post.Title),
        //                        new XElement("slug", post.Slug),
        //                        new XElement("pubDate", FormatDateTime(post.PubDate)),
        //                        new XElement("lastModified", FormatDateTime(post.LastModified)),
        //                        new XElement("excerpt", post.Excerpt),
        //                        new XElement("content", post.Content),
        //                        new XElement("ispublished", post.IsPublished),
        //                        new XElement("categories", string.Empty),
        //                        new XElement("comments", string.Empty)
        //                    ));

        //    var categories = doc.XPathSelectElement("post/categories");
        //    foreach (var category in post.Categories)
        //    {
        //        categories.Add(new XElement("category", category));
        //    }

        //    var comments = doc.XPathSelectElement("post/comments");
        //    foreach (var comment in post.Comments)
        //    {
        //        comments.Add(
        //            new XElement("comment",
        //                new XElement("author", comment.Author),
        //                new XElement("email", comment.Email),
        //                new XElement("date", FormatDateTime(comment.PubDate)),
        //                new XElement("content", comment.Content),
        //                new XAttribute("isAdmin", comment.IsAdmin),
        //                new XAttribute("id", comment.ID)
        //            ));
        //    }

        //    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
        //    {
        //        await doc.SaveAsync(fs, SaveOptions.None, CancellationToken.None).ConfigureAwait(false);
        //    }

        //    if (!this.cache.Contains(post))
        //    {
        //        this.cache.Add(post);
        //        this.SortCache();
        //    }
        //}

        protected bool isUser() => this.contextAccessor.HttpContext?.User?.Identity.IsAuthenticated == true;

        //protected void SortCache() => this.cache.Sort((p1, p2) => p2.pubDate.CompareTo(p1.pubDate));

        private static string CleanFromInvalidChars(string input)
        {
            // ToDo: what we are doing here if we switch the blog from windows to unix system or
            // vice versa? we should remove all invalid chars for both systems

            var regexSearch = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()));
            var r = new Regex($"[{regexSearch}]");
            return r.Replace(input, string.Empty);
        }

        private static string FormatDateTime(DateTime dateTime)
        {
            const string UTC = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";

            return dateTime.Kind == DateTimeKind.Utc
                ? dateTime.ToString(UTC, CultureInfo.InvariantCulture)
                : dateTime.ToUniversalTime().ToString(UTC, CultureInfo.InvariantCulture);
        }
        

        //private static void LoadCategories(Post post, XElement doc)
        //{
        //    var categories = doc.Element("categories");
        //    if (categories is null)
        //    {
        //        return;
        //    }

        //    post.Categories.Clear();
        //    categories.Elements("category").Select(node => node.Value).ToList().ForEach(post.Categories.Add);
        //}

        //private static void LoadComments(Post post, XElement doc)
        //{
        //    var comments = doc.Element("comments");

        //    if (comments is null)
        //    {
        //        return;
        //    }

        //    foreach (var node in comments.Elements("comment"))
        //    {
        //        var comment = new Comment
        //        {
        //            ID = ReadAttribute(node, "id"),
        //            Author = ReadValue(node, "author"),
        //            Email = ReadValue(node, "email"),
        //            IsAdmin = bool.Parse(ReadAttribute(node, "isAdmin", "false")),
        //            Content = ReadValue(node, "content"),
        //            PubDate = DateTime.Parse(ReadValue(node, "date", "2000-01-01"),
        //                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
        //        };

        //        post.Comments.Add(comment);
        //    }
        //}

        //private static string ReadAttribute(XElement element, XName name, string defaultValue = "") =>
        //    element.Attribute(name) is null ? defaultValue : element.Attribute(name)?.Value ?? defaultValue;

        //private static string ReadValue(XElement doc, XName name, string defaultValue = "") =>
        //    doc.Element(name) is null ? defaultValue : doc.Element(name)?.Value ?? defaultValue;

        //[SuppressMessage(
        //    "Usage",
        //    "SecurityIntelliSenseCS:MS Security rules violation",
        //    Justification = "Path not derived from user input.")]
        //private string GetFilePath(Post post) => Path.Combine(this.folder, $"{post.ID}.xml");

        //private void Initialize()
        //{
        //    this.LoadPosts();
        //    this.SortCache();
        //}

        //[SuppressMessage(
        //    "Globalization",
        //    "CA1308:Normalize strings to uppercase",
        //    Justification = "The slug should be lower case.")]
        //private void LoadPosts()
        //{

        //    if (!Directory.Exists(this.folder))
        //    {
        //        Directory.CreateDirectory(this.folder);
        //    }

        //    // Can this be done in parallel to speed it up?
        //    foreach (var file in Directory.EnumerateFiles(this.folder, "*.xml", SearchOption.TopDirectoryOnly))
        //    {
        //        var doc = XElement.Load(file);

        //        var post = new Post
        //        {
        //            ID = Path.GetFileNameWithoutExtension(file),
        //            Title = ReadValue(doc, "title"),
        //            Excerpt = ReadValue(doc, "excerpt"),
        //            Content = ReadValue(doc, "content"),
        //            Slug = ReadValue(doc, "slug").ToLowerInvariant(),
        //            PubDate = DateTime.Parse(ReadValue(doc, "pubDate"), CultureInfo.InvariantCulture,
        //                DateTimeStyles.AdjustToUniversal),
        //            LastModified = DateTime.Parse(
        //                ReadValue(
        //                    doc,
        //                    "lastModified",
        //                    DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
        //                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
        //            IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true")),
        //        };

        //        LoadCategories(post, doc);
        //        LoadComments(post, doc);
        //        this.cache.Add(post);
        //    }
        //}
    }
}
