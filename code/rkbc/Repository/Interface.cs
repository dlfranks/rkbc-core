﻿using rkbc.core.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.repository
{
    public interface IEntity
    {
        int id { get; }
    }
    public interface IPage
    {
        int pageId { get; set; }
    }
    public interface ISection
    {
        int sectionId { get; set; }
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
        Task<IQueryable<T>> getAsync();
        Task<IQueryable<T>> getAsync(int id);
        Task addAsync(T entity);
        Task<T> updateAsync(T entity);
        Task removeAsync(int id);
        Task removeAsync(T entity);
    }

    public interface IUnitOfWork : IDisposable
    {
        IRepository<HomePage> homePages { get; }
        IRepository<Attachment> attachments { get; }
        IRepository<UserActivityLog> userActivityLogs { get; }
        ApplicationDbContext getContext();
        void Commit();
        bool tryCommit();
        bool tryConcurrencyCommit();
        bool tryUniqueConstraintCommit();
        void updateCollection<TCOL>(IEnumerable<TCOL> oldList, IEnumerable<TCOL> newList) where TCOL : class, IEntity;
    }
}
