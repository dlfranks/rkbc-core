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
    public interface IPage : IEntity
    {
        int siteId { get; set; }
        PageEnum pageId { get; set; }

    }
    
    //public interface IRepository<T> where T : class
    //{
    //    IQueryable<T> get();
    //    IQueryable<T> get(int id);
    //    void add(T entity);
    //    T update(T entity);
    //    void remove(int id);
    //    void remove(T entity);
    //}
    public interface IRepository<T> where T: class
    {
        Task<IEnumerable<T>> getAsync();
        Task<T> getAsync(int id);
        void add(T entity);
        T update(T entity);
        Task removeAsync(int id);
         void remove(T entity);
    }

    public interface IUnitOfWork : IDisposable
    {
        IRepository<HomePage> homePages { get; }
        IRepository<UserActivityLog> userActivityLogs { get; }
        ApplicationDbContext getContext();
        Task commit();
        Task<bool> tryCommit();
        Task<bool> tryConcurrencyCommit();
        Task<bool> tryUniqueConstraintCommit();
        void updateCollection<TCOL>(IEnumerable<TCOL> oldList, IEnumerable<TCOL> newList) where TCOL : class, IEntity;
    }
}
