using System.Text;
using Domain.Builders;
using Microsoft.Extensions.ObjectPool;

namespace Domain.ObjectPoolPolicies;

internal sealed class ImageBuilderPoolPolicy(ObjectPool<StringBuilder> stringBuilderObjectPool)
    : IPooledObjectPolicy<ImageBuilder>
{
    public ImageBuilder Create()
    {
        return new ImageBuilder(stringBuilderObjectPool);
    }

    public bool Return(ImageBuilder builder)
    {
        return builder.TryReset();
    }
}