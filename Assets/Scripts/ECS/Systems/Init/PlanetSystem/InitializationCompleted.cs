using Unity.Entities;

namespace ECS.Systems.Init.PlanetSystem
{
    public sealed partial class InitPlanetSystem
    {
        private struct InitializationCompleted : ISystemStateComponentData
        {
        }
    }
}