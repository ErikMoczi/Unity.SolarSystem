using ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace ECS.Systems.Init.PlanetSystem
{
    public sealed partial class InitPlanetSystem
    {
        private struct InitBaseComponents : IJobParallelFor
        {
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public EntityCommandBuffer.Concurrent CommandBuffer;

            public void Execute(int index)
            {
                CommandBuffer.AddComponent(index, Entities[index], new Position());
                CommandBuffer.AddComponent(index, Entities[index], new Rotation());
                CommandBuffer.AddComponent(index, Entities[index], new Scale());
                CommandBuffer.AddComponent(index, Entities[index], new CopyTransformToGameObject());

                CommandBuffer.AddComponent(index, Entities[index], new LifeTime());
                CommandBuffer.AddBuffer<Angle>(index, Entities[index]);
            }
        }
    }
}