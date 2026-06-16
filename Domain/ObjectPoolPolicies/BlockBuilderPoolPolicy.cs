using System.Text;
using Domain.Builders;
using Microsoft.Extensions.ObjectPool;

namespace Domain.ObjectPoolPolicies;

internal sealed class BlockBuilderPoolPolicy(ObjectPool<StringBuilder> stringBuilderObjectPool)
    : IPooledObjectPolicy<BlockBuilder>
{
    public BlockBuilder Create()
    {
        return new BlockBuilder(stringBuilderObjectPool);
    }

    public bool Return(BlockBuilder obj)
    {
        return true;
    }
}