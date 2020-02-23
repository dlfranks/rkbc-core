﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(ApplicationDbContext ctx)
        {
            _context = ctx;
        }
        public ApplicationDbContext _context;
        IRepository<UserActivityLog> _userActivityLog = null;
        IRepository<HomePage> _homePage = null;
        public IRepository<HomePage> homePages
        {
            get
            {
                if (_homePage == null)
                    _homePage = new BasicRepository<HomePage>(_context);
                return _homePage;
            }
        }
        public IRepository<UserActivityLog> userActivityLogs
        {
            get
            {
                if(_userActivityLog == null)
                    _userActivityLog = new BasicRepository<UserActivityLog>(_context);
                return _userActivityLog;
            }
        }
        public ApplicationDbContext getContext()
        {
            throw new NotImplementedException();
        }
        public async Task commit()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<bool> tryCommit()
        {
            try { await commit(); }
            catch(Exception e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return (false);
            }
            return (true);
        }

        public async Task<bool> tryConcurrencyCommit()
        {
            try { await commit(); }
            catch (DbUpdateConcurrencyException e)
            {
                return (false);
            }
            return (true);
        }

        public async Task<bool> tryUniqueConstraintCommit()
        {
            try { await commit(); }
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
        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
