namespace Domain.Interfaces
{
    public interface IReadRepository<ID, TEntity> where TEntity : class
    {
        TEntity Find(TEntity model);
        TEntity Find(ID id);
        bool Exists(TEntity model);
        bool Exists(ID id);
    }
}
