namespace Jens.AspNetCore.AutoAPI;

public class InterceptorProvider : IInterceptorProvider
{
    private readonly IServiceProvider _serviceProvider;

    public InterceptorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TInterceptor? GetInterceptor<TInterceptor>()
        where TInterceptor : class
        => _serviceProvider.GetService(typeof(TInterceptor)) as TInterceptor;
}