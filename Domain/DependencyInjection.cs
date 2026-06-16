using Domain.Builders;
using Domain.ObjectPoolPolicies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;

namespace Domain;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddDomain(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton<PageBuilderPoolPolicy>();
        builder.Services.TryAddSingleton<ObjectPool<PageBuilder>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = serviceProvider.GetRequiredService<PageBuilderPoolPolicy>();
            return provider.Create(policy);
        });
        
        builder.Services.TryAddSingleton<ImageBuilderPoolPolicy>();
        builder.Services.TryAddSingleton<ObjectPool<ImageBuilder>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = serviceProvider.GetRequiredService<ImageBuilderPoolPolicy>();
            return provider.Create(policy);
        });
        
        builder.Services.TryAddSingleton<BlockBuilderPoolPolicy>();
        builder.Services.TryAddSingleton<ObjectPool<BlockBuilder>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = serviceProvider.GetRequiredService<BlockBuilderPoolPolicy>();
            return provider.Create(policy);
        });
        
        builder.Services.TryAddSingleton<LineBuilderPoolPolicy>();
        builder.Services.TryAddSingleton<ObjectPool<LineBuilder>>(static serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
            var policy = serviceProvider.GetRequiredService<LineBuilderPoolPolicy>();
            return provider.Create(policy);
        });
        
        return builder;
    }
}