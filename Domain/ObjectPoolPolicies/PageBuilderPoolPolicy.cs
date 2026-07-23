using System.Text;
using Domain.Builders;
using Microsoft.Extensions.ObjectPool;

namespace Domain.ObjectPoolPolicies;

internal sealed class PageBuilderPoolPolicy(ObjectPool<StringBuilder> stringBuilderObjectPool)
    : IPooledObjectPolicy<PageBuilder>
{
    public PageBuilder Create()
    {
        return new PageBuilder(stringBuilderObjectPool);
    }

    public bool Return(PageBuilder builder)
    {
        return builder.TryReset();
    }
}