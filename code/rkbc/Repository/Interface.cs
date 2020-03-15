using rkbc.core.models;
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
        IQueryable<T> get();
        IQueryable<T> get(int id);
        void add(T entity);
        T update(T entity);
        Task removeAsync(int id);
        void remove(T entity);
    }

    public interface IUnitOfWork : IDisposable
    {
        IRepositoryAsync<HomePage> homePages { get; }
        IRepositoryAsync<HomeContentItem> homeContentItems { get; }
        IRepositoryAsync<HomeAttachment> homeAttachments { get; }
        IRepositoryAsync<HomeVideoAttachment> homeVideoAttachments { get; }
        IRepositoryAsync<UserActivityLog> userActivityLogs { get; }
        ApplicationDbContext getContext();
        Task commit();
        Task<bool> tryCommit();
        Task<bool> tryConcurrencyCommit();
        Task<bool> tryUniqueConstraintCommit();
        void updateCollection<TCOL>(IEnumerable<TCOL> oldList, IEnumerable<TCOL> newList) where TCOL : class, IEntity;
    }
}
