using ECS.Utils;
using Unity.Entities;

namespace ECS.Components
{
    [InternalBufferCapacity(Scales.N)]
    public struct Angle : IBufferElementData
    {
        public float Value;
    }
}