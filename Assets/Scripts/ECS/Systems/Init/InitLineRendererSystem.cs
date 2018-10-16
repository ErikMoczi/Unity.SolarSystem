using ECS.Components;
using ECS.Systems.Init.PlanetSystem;
using ECS.Utils.Helpers;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems.Init
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(InitPlanetSystem))]
    public sealed class InitLineRendererSystem : ComponentSystem
    {
#pragma warning disable 649
        private struct Data
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<PlanetaryOrbit> PlanetaryOrbit;
            [ReadOnly] public ComponentDataArray<Position> Position;
            [WriteOnly] public ComponentArray<LineRenderer> LineRenderer;
        }

        [Inject] private Data _data;
#pragma warning restore 649

        protected override void OnUpdate()
        {
            for (var i = 0; i < _data.Length; i++)
            {
                LineRendererHelper.BaseInit(_data.Position[i], _data.PlanetaryOrbit[i], _data.LineRenderer[i]);
            }
        }
    }
}