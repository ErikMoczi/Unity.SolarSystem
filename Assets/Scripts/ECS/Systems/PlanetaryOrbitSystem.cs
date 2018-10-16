using ECS.Components;
using ECS.Systems.Init.PlanetSystem;
using ECS.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    [UpdateAfter(typeof(InitPlanetSystem))]
    [UpdateAfter(typeof(UnityEngine.Experimental.PlayerLoop.FixedUpdate))]
    public sealed class PlanetaryOrbitSystem : JobComponentSystem
    {
#pragma warning disable 649
        private struct Data
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<PlanetaryOrbit> PlanetaryOrbit;
            [WriteOnly] public ComponentDataArray<LifeTime> LifeTime;
            [WriteOnly] public ComponentDataArray<Position> Position;
            [WriteOnly] public BufferArray<Angle> Angle;
        }

        [Inject] private Data _data;
#pragma warning restore 649

        [BurstCompile(Accuracy = Accuracy.Low, Support = Support.Relaxed)]
        private struct Job : IJobParallelFor
        {
            [ReadOnly] public ComponentDataArray<PlanetaryOrbit> PlanetaryOrbit;
            [ReadOnly] public BufferArray<Angle> Angle;
            [ReadOnly] public float FixedDeltaTime;
            public ComponentDataArray<LifeTime> LifeTime;
            public ComponentDataArray<Position> Position;

            public unsafe void Execute(int index)
            {
                var time = LifeTime[index];
                time.Value += FixedDeltaTime;
                LifeTime[index] = time;

                var position = Position[index];
                position.Value = PlanetaryOrbit[index].ParametricOrbit(
                    time.Value,
                    (float*) Angle[index].GetBasePointer()
                );
                Position[index] = position;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                PlanetaryOrbit = _data.PlanetaryOrbit,
                LifeTime = _data.LifeTime,
                Position = _data.Position,
                Angle = _data.Angle,
                FixedDeltaTime = Time.fixedDeltaTime,
            }.Schedule(_data.Length, SystemData.PlanetBatchCount, inputDeps);
        }
    }
}