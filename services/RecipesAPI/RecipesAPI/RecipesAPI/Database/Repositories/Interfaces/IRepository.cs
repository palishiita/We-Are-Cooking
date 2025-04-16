using RecipesAPI.Entities;

namespace RecipesAPI.Database.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        List<TEntity> GetAll();
        IQueryable<TEntity> Query { get; }
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Upsert(TEntity entity);
        void UpsertMany(TEntity[] entities);
        void Delete(TEntity entity);
        void Save();
    }
}
