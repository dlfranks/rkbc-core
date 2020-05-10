using Microsoft.EntityFrameworkCore;
using rkbc.core.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rkbc.core.Repository
{
    public class BasicRepository<T> : IRepositoryAsync<T> where T : class, IEntity
    {
        protected readonly ApplicationDbContext _ctx;
        protected readonly DbSet<T> _set;

        public BasicRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _set = ctx.Set<T>();
        }

        public IQueryable<T> get() { return _set; }
        
        public IQueryable<T> get(int id)
        {
            return _set.Where(q => q.id == id).AsQueryable();
        }
        
        public void add(T entity)
        {
            _set.Add(entity);
            //_ctx.Entry(entity).State = EntityState.Added;
        }

        // TODO -> To enforce proper in memory change tracking where objects can query other objects
        // and receive changes made, there should actually not be an update method.  So, at some
        // point remove this and then fix all places where it is used to update the attached object
        // rather than pushing the updates through this. See EF discussion file.
        public virtual T update(T entity)
        {
            T original = _set.Find(entity.id);
            _ctx.Entry(original).CurrentValues.SetValues(entity);
            //Concurrency check, the original timestamp must be updated, not just the current timestamp.  
            //The current value is ignored.  The reflection below can be removed by adding
            //timeStamp to IEntity, but don't want to do that just yet.
            try
            {
                _ctx.Entry(original).Property("timeStamp").OriginalValue = typeof(T).GetProperty("timeStamp").GetValue(entity, null);
            }
            catch (Exception e) { }

            return (original);
            //T original = _set.Local.Where(q => q.id == entity.id).FirstOrDefault();
            //if (original != null) 
            //    _ctx.Entry(original).CurrentValues.SetValues(entity);
            //else 
            //    _ctx.Entry(entity).State = EntityState.Modified;
        }

        public virtual void remove(T entity) { _set.Remove(entity); }
        public async Task removeAsync(int id)
        {
            
            _set.Remove(await get(id).FirstOrDefaultAsync());
        }
        public void removeRange(IList<T> entities)
        {

            _set.RemoveRange(entities);
        }
    }
    
    
}
