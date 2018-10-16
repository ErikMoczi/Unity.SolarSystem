using System.Collections.Generic;
using ECS.Components;
using ECS.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems.Init.PlanetSystem
{
    [UpdateAfter(typeof(UnityEngine.Experimental.PlayerLoop.Initialization))]
    public sealed partial class InitPlanetSystem : ComponentSystem
    {
#pragma warning disable 649
        private struct Data
        {
            public readonly int Length;
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<PlanetaryOrbit> PlanetaryOrbit;
            [ReadOnly] public SubtractiveComponent<InitializationCompleted> Missing;
        }

        [Inject] private Data _data;
#pragma warning restore 649

        private EntityCommandBuffer.Concurrent _commandBuffer;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            _initPlanetBarrier = SetUpPlanetBarrier();
            _commandBuffer = _initPlanetBarrier.CreateCommandBuffer().ToConcurrent();

            _initSystems = new List<ComponentSystemBase>
            {
                World.GetOrCreateManager<InitPlanetaryOrbit>(),
                World.GetOrCreateManager<InitAngleSystem>(),
                World.GetOrCreateManager<InitHierarchySystem>(),
                World.GetOrCreateManager<InitLineRendererSystem>(),
            };

            for (var i = 0; i < _initSystems.Count; i++)
            {
                _initSystems[i].Enabled = false;
            }
        }

        protected override void OnUpdate()
        {
            new InitBaseComponents
            {
                Entities = _data.Entities,
                CommandBuffer = _commandBuffer,
            }.Schedule(_data.Length, SystemData.PlanetBatchCount).Complete();

            new FinishInitPlanet
            {
                Entities = _data.Entities,
                CommandBuffer = _commandBuffer,
            }.Schedule(_data.Length, SystemData.PlanetBatchCount).Complete();

            SyncInitBarrier();
            RunInitSystems();
        }
    }
}