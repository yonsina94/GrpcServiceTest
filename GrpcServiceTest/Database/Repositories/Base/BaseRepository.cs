using GrpcServiceTest.Database.Entities.Base;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Linq.Expressions;

namespace GrpcServiceTest.Database.Repositories.Base
{
    public interface IBaseRepository<Tentity> where Tentity : class, IBaseEntity
    {
        Task<int> CommitChangesAsync();
        Task<Tentity> GetByIDAsync(object id);
        Task<IQueryable<Tentity>> GetAsync(Expression<Func<Tentity, bool>> where, string includeProperties = "");
        Task<IQueryable<Tentity>> GetAsync(Expression<Func<Tentity, bool>> where, params Expression<Func<Tentity, object>>[] include);
        Task<IQueryable<Tentity>> GetAsync(params Expression<Func<Tentity, object>>[] include);
        Task<IQueryable<Tentity>> GetAllAsync();
        Task<int> CountAsync();
        Task<Tentity> InsertAsync(Tentity entity);
        Task<Tentity> UpdateAsync(Tentity entity);
        Task<Tentity> UpdateAsync(Tentity entity, object id);
        Task UpdatePropertyAsync<Type>(Expression<Func<Tentity, Type>> property, Tentity entity);
        Task DeleteAsync(Tentity Entity);
        Task DeleteAsync(object id);
        Task DeleteAsync(Expression<Func<Tentity, bool>> primaryKeys);
        Task<NpgsqlDataReader> RunAsync(string query);
        Task DeleteRangeAsync(IEnumerable<Tentity> entities);
        Task<IEnumerable<Tentity>> InsertRangeAsync(IEnumerable<Tentity> entities);
        Task<IEnumerable<Tentity>> UpdateRangeAsync(IEnumerable<Tentity> entities);
    }
    public class BaseRepository<Tentity> : IBaseRepository<Tentity> where Tentity : class, IBaseEntity
    {
        protected readonly DatabaseContext Context;

        public BaseRepository(DatabaseContext context)
        {
            Context = context;
        }

        public async Task<int> CommitChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public virtual async Task<Tentity> GetByIDAsync(object id)
        {
            return await Context.Set<Tentity>().FindAsync(id);
        }

        public virtual async Task<IQueryable<Tentity>> GetAsync(Expression<Func<Tentity, bool>> where, string includeProperties = "")
        {
            return await Task.Run(() => {
                var query = Context.Set<Tentity>().AsQueryable();

                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (where != null)
                    query = query.AsExpandable().Where(where);

                return query;
            });
        }

        public virtual async Task<IQueryable<Tentity>> GetAsync(Expression<Func<Tentity, bool>> @where, params Expression<Func<Tentity, object>>[] include)
        {
            return await Task.Run(() =>
            {
                var query = Context.Set<Tentity>().AsQueryable();

                foreach (var includeProperty in include)
                {
                    query = query.Include(includeProperty);
                }

                if (where != null)
                    query = query.AsExpandable().Where(where);

                return query;
            });
        }

        public virtual async Task<IQueryable<Tentity>> GetAsync(params Expression<Func<Tentity, object>>[] include)
        {
            return await Task.Run(() =>
            {
                var query = Context.Set<Tentity>().AsQueryable().AsExpandable();

                foreach (var includeProperty in include)
                {
                    query = query.Include(includeProperty);
                }

                return query;
            });
        }

        public virtual async Task<IQueryable<Tentity>> GetAllAsync()
        {
            return await Task.Run(() => Context.Set<Tentity>().AsQueryable());
        }

        public virtual async Task<int> CountAsync()
        {
            return await Task.Run(() => Context.Set<Tentity>().Count());
        }

        public virtual async Task<Tentity> InsertAsync(Tentity entity)
        {
            await Context.Set<Tentity>().AddAsync(entity);
            return entity;
        }

        public virtual async Task<IEnumerable<Tentity>> InsertRangeAsync(IEnumerable<Tentity> entity)
        {
            await Context.Set<Tentity>().AddRangeAsync(entity);
            return entity;
        }

        public virtual async Task<Tentity> UpdateAsync(Tentity entity)
        {
            return await Task.Run(() =>
            {
                Context.Set<Tentity>().Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;

                return entity;
            });
        }

        public virtual async Task<Tentity> UpdateAsync(Tentity entity, object id)
        {
            var entry = Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                var attachedEntity = await GetByIDAsync(id);

                if (attachedEntity != null)
                {
                    var attachedEntry = Context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    entry.State = EntityState.Modified;
                }
            }
            return entity;
        }

        public virtual async Task<IEnumerable<Tentity>> UpdateRangeAsync(IEnumerable<Tentity> entities)
        {
            return await Task.Run(() => {
                var result = new List<Tentity>();
                foreach (var entity in entities)
                {
                    Context.Set<Tentity>().Attach(entity);
                    Context.Entry(entity).State = EntityState.Modified;
                    result.Add(entity);
                }
                return result;
            });
        }

        public virtual async Task UpdatePropertyAsync<Type>(Expression<Func<Tentity, Type>> property, Tentity entity)
        {
            Context.Set<Tentity>().Attach(entity);
            Context.Entry(entity).Property(property).IsModified = true;
            await Context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Tentity entity)
        {
            await Task.Run(() => Context.Set<Tentity>().Remove(entity));
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<Tentity> entity)
        {
            await Task.Run(() => Context.Set<Tentity>().RemoveRange(entity));
        }


        public virtual async Task DeleteAsync(object id)
        {
            var entity = await GetByIDAsync(id);
            await DeleteAsync(entity);
        }

        public virtual async Task DeleteAsync(Expression<Func<Tentity, bool>> primaryKeys)
        {
            var entity = (await GetAsync(primaryKeys)).FirstOrDefault();
            await DeleteAsync(entity);
        }

        public virtual async Task<NpgsqlDataReader> RunAsync(string query)
        {
            var connection = Context.Database.GetDbConnection();

            var conn = new NpgsqlConnection(connection.ConnectionString);

            using var command = new NpgsqlCommand(query, conn);
            await conn.OpenAsync();

            return await command.ExecuteReaderAsync();
        }
    }
}
