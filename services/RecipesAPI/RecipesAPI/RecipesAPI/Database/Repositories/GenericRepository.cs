using Microsoft.EntityFrameworkCore;
using RecipesAPI.Database.Repositories.Interfaces;
using RecipesAPI.Entities;

namespace RecipesAPI.Database.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _entitySet;

        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _entitySet = dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> Query => _entitySet;

        public void Add(TEntity entity)
        {
            _entitySet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _entitySet.Update(entity);
        }

        public void Upsert(TEntity entity)
        {
            _dbContext.Entry(entity).State = entity.Id == Guid.Empty ? EntityState.Added : EntityState.Modified;

            if (_entitySet.Contains(entity))
            {
                _entitySet.Update(entity);
                return;
            }
            _entitySet.Add(entity);
        }

        public void UpsertMany(TEntity[] entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            _entitySet.Remove(entity);
        }

        public List<TEntity> GetAll()
        {
            return _entitySet.ToList();
        }

        public TEntity GetById(Guid id)
        {
            return _entitySet.FirstOrDefault() ?? throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} with id {id} does not exist.");
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void StartTransaction()
        {
            _dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _dbContext.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            _dbContext.Database.RollbackTransaction();
        }

        public async Task StartTransactionAsync()
        {
            await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _dbContext.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _dbContext.Database.RollbackTransactionAsync();
        }
    }
}
