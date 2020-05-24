using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElmahCore;

namespace rkbc.core.repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(ApplicationDbContext ctx)
        {
            _context = ctx;
        }
        public ApplicationDbContext _context;
        IRepositoryAsync<UserActivityLog> _userActivityLog = null;
        IRepositoryAsync<HomePage> _homePage = null;
        IRepositoryAsync<Contact> _contacts = null;
        IRepositoryAsync<PastorPage> _pastorPage = null;
        IRepositoryAsync<Attachment> _attachments = null;
        IRepositoryAsync<Blog> _blogs = null;
        IRepositoryAsync<Post> _posts = null;
        IRepositoryAsync<Comment> _comments = null;

        IRepositoryAsync<HomeContentItem> _homeContentItems = null;

        public IRepositoryAsync<HomeContentItem> homeContentItems
        {
            get
            {
                if (_homeContentItems == null)
                    _homeContentItems = new BasicRepository<HomeContentItem>(_context);
                return _homeContentItems;
            }
        }
        public IRepositoryAsync<Attachment> attachments
        {
            get
            {
                if (_attachments == null)
                    _attachments = new BasicRepository<Attachment>(_context);
                return _attachments;
            }
        }
        
        public IRepositoryAsync<HomePage> homePages
        {
            get
            {
                if (_homePage == null)
                    _homePage = new BasicRepository<HomePage>(_context);
                return _homePage;
            }
        }
        public IRepositoryAsync<PastorPage> pastorPages
        {
            get
            {
                if (_pastorPage == null)
                    _pastorPage = new BasicRepository<PastorPage>(_context);
                return _pastorPage;
            }
        }
        public IRepositoryAsync<Contact> contacts
        {
            get
            {
                if (_contacts == null)
                    _contacts = new BasicRepository<Contact>(_context);
                return _contacts;
            }
        }
        public IRepositoryAsync<UserActivityLog> userActivityLogs
        {
            get
            {
                if(_userActivityLog == null)
                    _userActivityLog = new BasicRepository<UserActivityLog>(_context);
                return _userActivityLog;
            }
        }
        public IRepositoryAsync<Blog> blogs
        {
            get
            {
                if (_blogs == null)
                    _blogs = new BasicRepository<Blog>(_context);
                return _blogs;
            }
        }
        public IRepositoryAsync<Post> posts
        {
            get
            {
                if (_posts == null)
                    _posts = new BasicRepository<Post>(_context);
                return _posts;
            }
        }
        public IRepositoryAsync<Comment> comments
        {
            get
            {
                if (_comments == null)
                    _comments = new BasicRepository<Comment>(_context);
                return _comments;
            }
        }
        public ApplicationDbContext getContext()
        {
            return _context;
        }
        public async Task commitAsync()
        {
            await _context.SaveChangesAsync();
            
        }
        public void commit()
        {
            _context.SaveChanges();
        }
        public async Task<bool> tryCommitAsync()
        {
            try { await commitAsync(); }
            catch(Exception e)
            {
                ElmahExtensions.RiseError(e);
                new InvalidOperationException("exception", e);
                return (false);
            }
            return (true);
        }

        public async Task<bool> tryConcurrencyCommitAsync()
        {
            try { await commitAsync(); }
            catch (DbUpdateConcurrencyException e)
            {
                return (false);
            }
            return (true);
        }

        public async Task<bool> tryUniqueConstraintCommitAsync()
        {
            try { await commitAsync(); }
            catch (DbUpdateException e)
            {
                if (e.InnerException is DbUpdateException)
                    if (e.InnerException.InnerException is SqlException)
                        if ((e.InnerException.InnerException as SqlException).Number == 2627)
                            return (false);
                throw e;
            }
            return (true);
        }

        public void updateCollection<TCOL>(IEnumerable<TCOL> oldList, IEnumerable<TCOL> newList) where TCOL : class, IEntity
        {
            //Update navigational property
            foreach (var item in newList)
            {
                if (item.id == 0)
                    _context.Entry(item).State = EntityState.Added;
                else
                {
                    var tmp = oldList.Where(q => q.id == item.id).FirstOrDefault();
                    _context.Entry(tmp).CurrentValues.SetValues(item);
                    //Concurrency check, the original timestamp must be updated, not just the current timestamp.  
                    //The current value is ignored.  The reflection below can be removed by adding
                    //timeStamp to IEntity, but don't want to do that just yet.
                    try
                    {
                        _context.Entry(tmp).Property("timeStamp").OriginalValue = typeof(TCOL).GetProperty("timeStamp").GetValue(item, null);
                    }
                    catch (Exception e) { }
                }
            }
            //Perform deletes
            var tray = oldList.ToArray();
            foreach (var item in tray)
                if (newList.Where(q => q.id == item.id).Count() == 0)
                    _context.Entry(item).State = EntityState.Deleted;
        }
        //public void Dispose()
        //{
        //    if (_context != null)
        //    {
        //        _context.Dispose();
        //    }
        //    GC.SuppressFinalize(this);
        //}
    }
}
