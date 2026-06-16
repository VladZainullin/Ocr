using System.Text;
using Domain.Builders;
using Microsoft.Extensions.ObjectPool;

namespace Domain.ObjectPoolPolicies;

internal sealed class LineBuilderPoolPolicy(ObjectPool<StringBuilder> stringBuilderObjectPool)
    : IPooledObjectPolicy<LineBuilder>
{
    public LineBuilder Create()
    {
        return new LineBuilder(stringBuilderObjectPool);
    }

    public bool Return(LineBuilder obj)
    {
        return true;
    }
}