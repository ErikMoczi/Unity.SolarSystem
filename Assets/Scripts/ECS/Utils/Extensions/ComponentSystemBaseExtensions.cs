using Unity.Entities;

namespace ECS.Utils.Extensions
{
    public static class ComponentSystemBaseExtensions
    {
        public static void OneTimeRun(this ComponentSystemBase system)
        {
            system.Enabled = true;
            system.Update();
            system.Enabled = false;
        }
    }
}