namespace Jens.AspNetCore.AutoAPI;

public interface IInterceptorProvider
{
    TInterceptor? GetInterceptor<TInterceptor>()
        where TInterceptor : class;
}
