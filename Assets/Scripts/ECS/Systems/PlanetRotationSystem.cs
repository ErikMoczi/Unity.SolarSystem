using ECS.Components;
using ECS.Systems.Init.PlanetSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    [UpdateAfter(typeof(InitPlanetSystem))]
    [UpdateAfter(typeof(UnityEngine.Experimental.PlayerLoop.FixedUpdate))]
    public sealed class PlanetRotationSystem : JobComponentSystem
    {
        [BurstCompile(Accuracy = Accuracy.Low, Support = Support.Relaxed)]
        private struct Job : IJobProcessComponentData<PlanetaryOrbit, Rotation>
        {
            [ReadOnly] public float FixedDeltaTime;

            public void Execute([ReadOnly] ref PlanetaryOrbit planetaryOrbit, [WriteOnly] ref Rotation rotation)
            {
                rotation.Value = math.mul(
                    math.normalize(rotation.Value),
                    quaternion.AxisAngle(
                        math.up(),
                        math.radians(360f / planetaryOrbit.RotationPeriod * FixedDeltaTime)
                    )
                );
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                FixedDeltaTime = Time.fixedDeltaTime,
            }.Schedule(this, inputDeps);
        }
    }
}