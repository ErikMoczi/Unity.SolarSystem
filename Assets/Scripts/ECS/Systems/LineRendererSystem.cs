using ECS.Components;
using ECS.Systems.Init.PlanetSystem;
using ECS.Utils.Helpers;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ECS.Systems
{
    [UpdateAfter(typeof(InitPlanetSystem))]
    [UpdateAfter(typeof(UnityEngine.Experimental.PlayerLoop.EarlyUpdate))]
    public sealed class LineRendererSystem : ComponentSystem
    {
#pragma warning disable 649
        private struct Data
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<PlanetaryOrbit> PlanetaryOrbit;
            [WriteOnly] public ComponentArray<LineRenderer> LineRenderer;
        }

        [Inject] private Data _data;
#pragma warning restore 649

        protected override void OnUpdate()
        {
            for (var i = 0; i < _data.Length; i++)
            {
                LineRendererHelper.CalculateByCamera(_data.PlanetaryOrbit[i], _data.LineRenderer[i]);
            }
        }
    }
}