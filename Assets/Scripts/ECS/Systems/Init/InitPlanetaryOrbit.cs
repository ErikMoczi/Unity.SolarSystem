using ECS.Components;
using ECS.Systems.Init.PlanetSystem;
using ECS.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Init
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(InitPlanetSystem))]
    public sealed class InitPlanetaryOrbit : JobComponentSystem
    {
        [BurstCompile(Accuracy = Accuracy.Low, Support = Support.Relaxed)]
        private struct Job : IJobProcessComponentData<PlanetaryOrbit, Scale, Rotation>
        {
            public void Execute([WriteOnly] ref PlanetaryOrbit planetaryOrbit, [WriteOnly] ref Scale scale,
                [WriteOnly] ref Rotation rotation)
            {
                planetaryOrbit.Init(ref planetaryOrbit);
                scale.Value = new float3(
                                  planetaryOrbit.Radius,
                                  planetaryOrbit.Radius,
                                  planetaryOrbit.Radius
                              ) * Scales.PlanetScaleFactor;
                rotation.Value = quaternion.Euler(math.radians(planetaryOrbit.AxialTilt), 0f, 0f);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job().Schedule(this, inputDeps);
        }
    }
}