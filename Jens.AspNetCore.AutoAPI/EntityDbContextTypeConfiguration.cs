namespace Jens.AspNetCore.AutoAPI;

public class EntityDbContextTypeConfiguration<TDefaultDbContext> : IDbContextProvider
    where TDefaultDbContext : DbContext
{
    private readonly Dictionary<Type, Type> _entityToDbContextMap = new Dictionary<Type, Type>();

    public IReadOnlyDictionary<Type, Type> EntityToDbContextMap => _entityToDbContextMap;
    public Type DefaultDbContext { get; }

    public EntityDbContextTypeConfiguration()
    {
        DefaultDbContext = typeof(TDefaultDbContext);
    }

    public EntityDbContextTypeConfiguration<TDefaultDbContext> With<TDbContext, TEntity>()
        where TDbContext : DbContext
        where TEntity : class, IEntity
    {
        _entityToDbContextMap.Add(typeof(TEntity), typeof(TDbContext));
        return this;
    }

    public Type GetDbContext(Type entityType)
    {
        if (!EntityToDbContextMap.ContainsKey(entityType))
            return DefaultDbContext;
        return EntityToDbContextMap[entityType];
    }
}
