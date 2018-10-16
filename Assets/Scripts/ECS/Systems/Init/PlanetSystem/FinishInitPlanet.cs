using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems.Init.PlanetSystem
{
    public sealed partial class InitPlanetSystem
    {
        private struct FinishInitPlanet : IJobParallelFor
        {
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public EntityCommandBuffer.Concurrent CommandBuffer;

            public void Execute(int index)
            {
                CommandBuffer.AddComponent(index, Entities[index], new InitializationCompleted());
            }
        }
    }
}