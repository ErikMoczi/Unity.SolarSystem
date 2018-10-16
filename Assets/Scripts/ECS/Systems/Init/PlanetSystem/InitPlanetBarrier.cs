using Unity.Entities;

namespace ECS.Systems.Init.PlanetSystem
{
    public sealed partial class InitPlanetSystem
    {
        [DisableAutoCreation]
        [UpdateInGroup(typeof(InitPlanetSystem))]
        private class InitPlanetBarrier : BarrierSystem
        {
        }
    }
}