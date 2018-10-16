using System.Collections.Generic;
using ECS.Utils.Extensions;
using Unity.Entities;

namespace ECS.Systems.Init.PlanetSystem
{
    public sealed partial class InitPlanetSystem
    {
        private List<ComponentSystemBase> _initSystems;
        private InitPlanetBarrier _initPlanetBarrier;

        private InitPlanetBarrier SetUpPlanetBarrier()
        {
            var initPlanetBarrier = World.GetOrCreateManager<InitPlanetBarrier>();
            initPlanetBarrier.Enabled = false;
            return initPlanetBarrier;
        }

        private void SyncInitBarrier()
        {
            _initPlanetBarrier.OneTimeRun();
        }

        private void RunInitSystems()
        {
            for (var i = 0; i < _initSystems.Count; i++)
            {
                _initSystems[i].OneTimeRun();
            }
        }
    }
}