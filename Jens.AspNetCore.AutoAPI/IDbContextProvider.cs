namespace Jens.AspNetCore.AutoAPI;

public interface IDbContextProvider
{
    Type GetDbContext(Type entityType);
}
