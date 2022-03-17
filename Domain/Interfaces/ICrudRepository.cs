namespace Domain.Interfaces
{
    public interface ICrudRepository<ID, TEntity> : IReadRepository<ID, TEntity> where TEntity : class
    {
        TEntity Add(TEntity model);
        TEntity Update(TEntity model);
        bool Remove(TEntity model);
        bool Remove(ID id);
    }
}
