using ECS.Utils;
using UnityEngine;

namespace ECS
{
    public sealed class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            GlobalTime.SetDefaultTimeScale();
        }
    }
}