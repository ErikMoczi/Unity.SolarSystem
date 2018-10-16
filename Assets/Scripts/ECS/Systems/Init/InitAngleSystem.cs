using ECS.Components;
using ECS.Systems.Init.PlanetSystem;
using ECS.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems.Init
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(InitPlanetSystem))]
    public sealed class InitAngleSystem : JobComponentSystem
    {
#pragma warning disable 649
        private struct Data
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<PlanetaryOrbit> PlanetaryOrbit;
            [WriteOnly] public BufferArray<Angle> Angle;
        }

        [Inject] private Data _data;
#pragma warning restore 649

        [BurstCompile(Accuracy = Accuracy.Low, Support = Support.Relaxed)]
        private struct Job : IJobParallelFor
        {
            [ReadOnly] public ComponentDataArray<PlanetaryOrbit> PlanetaryOrbit;
            [WriteOnly] public BufferArray<Angle> Angles;

            public unsafe void Execute(int index)
            {
                var angles = Angles[index];
                angles.ResizeUninitialized(Scales.N);

                PlanetaryOrbit[index].ThetaRange((float*) angles.GetBasePointer());
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                Angles = _data.Angle,
                PlanetaryOrbit = _data.PlanetaryOrbit,
            }.Schedule(_data.Length, SystemData.PlanetBatchCount, inputDeps);
        }
    }
}