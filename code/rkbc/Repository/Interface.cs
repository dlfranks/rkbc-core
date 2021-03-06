﻿using rkbc.core.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.repository
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string name, string subject, string message);
    }
    public interface IEntity
    {
        int id { get; }
    }
    
    public interface IAuditStamp
    {
        DateTime? createDt { get; set; }
        string createUser { get; set; }
        DateTime? lastUpdDt { get; set; }
        string lastUpdUser { get; set; }
    }
    public interface IRepository<T> where T : class
    {
        IQueryable<T> get();
        IQueryable<T> get(int id);
        void add(T entity);
        T update(T entity);
        void remove(int id);
        void remove(T entity);
    }
    public interface IRepositoryAsync<T> where T: class
    {
        IQueryable<T> get();
        IQueryable<T> get(int id);
        void add(T entity);
        T update(T entity);
        Task removeAsync(int id);
        void remove(T entity);
        void removeRange(IList<T> entities);
    }
    
    public interface IUnitOfWork 
    {
        IRepositoryAsync<HomePage> homePages { get; }
        IRepositoryAsync<PastorPage> pastorPages { get; }
        IRepositoryAsync<Contact> contacts { get; }
        IRepositoryAsync<HomeContentItem> homeContentItems { get; }
        IRepositoryAsync<Attachment> attachments { get; }
        IRepositoryAsync<Blog> blogs { get; }
        IRepositoryAsync<Post> posts { get; }
        IRepositoryAsync<Comment> comments { get; }

        IRepositoryAsync<UserActivityLog> userActivityLogs { get; }
        ApplicationDbContext getContext();
        Task commitAsync();
        void commit();
        Task<bool> tryCommitAsync();
        Task<bool> tryConcurrencyCommitAsync();
        Task<bool> tryUniqueConstraintCommitAsync();
        void updateCollection<TCOL>(IEnumerable<TCOL> oldList, IEnumerable<TCOL> newList) where TCOL : class, IEntity;
    }
    public interface IBlogService
    {
        Task<Blog> getBlog(int blogId);
        Task DeletePost(Post post);

        IAsyncEnumerable<string> GetCategories();

        Task<Post?> GetPostById(string id);

        Task<Post?> GetPostBySlug(string slug);

        IAsyncEnumerable<Post> GetPosts();

        IAsyncEnumerable<Post> GetPosts(int count, int skip = 0);

        IAsyncEnumerable<Post> GetPostsByCategory(string category);

        Task<string> SaveFile(byte[] bytes, string fileName, string? suffix = null);

        Task SavePost(Post post);
    }
}
