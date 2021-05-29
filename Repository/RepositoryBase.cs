using System;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MusalaSoft.GatewayApi.Data;

namespace MusalaSoft.GatewayApi.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T: class
    {
        protected ApplicationDbContext RepositoryContext { get; set; }

        protected RepositoryBase(ApplicationDbContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            return await this.RepositoryContext.Set<T>().AsNoTracking<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await this.RepositoryContext.Set<T>().AsNoTracking<T>().Where(expression).ToListAsync();
        }

        public void Create(T entity)
        {
            this.RepositoryContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.RepositoryContext.Entry(entity).State = EntityState.Modified;
            this.RepositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
        }

        public bool IsExist(long id) => id > 0 && this.RepositoryContext.Set<T>().Find(id) != null;


        public async Task SaveAsync()
        {
            await this.RepositoryContext.SaveChangesAsync();
        }
    }
}