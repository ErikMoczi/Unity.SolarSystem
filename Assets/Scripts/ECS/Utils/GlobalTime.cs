using ECS.Systems;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Utils
{
    public static class GlobalTime
    {
        private const float DefaultTimeScale = 1f;
        private const float MinCurrentTimeScale = 0.125f;
        private const float MaxCurrentTimeScale = 64f;
        private const float TimeScaleFactor = 2.0f;
        private const float FixedDeltaTimeFactor = 0.02f;

        private static bool _running = true;
        public static float CurrentTimeScale { get; private set; }

        public static bool Running
        {
            get => _running;
            set
            {
                if (value && !_running)
                {
                    _running = true;
                    Resume();
                }

                if (!value && _running)
                {
                    _running = false;
                    Pause();
                }
            }
        }

        private static void Pause()
        {
            World.Active.GetExistingManager<PlanetaryOrbitSystem>().Enabled = false;
            World.Active.GetExistingManager<PlanetRotationSystem>().Enabled = false;
        }

        private static void Resume()
        {
            World.Active.GetExistingManager<PlanetaryOrbitSystem>().Enabled = true;
            World.Active.GetExistingManager<PlanetRotationSystem>().Enabled = true;
        }

        public static void SetDefaultTimeScale()
        {
            CurrentTimeScale = DefaultTimeScale;
            ClampTimeScale();
        }

        public static void IncreaseTimeScale()
        {
            CurrentTimeScale *= TimeScaleFactor;
            ClampTimeScale();
        }

        public static void DecreaseTimeScale()
        {
            CurrentTimeScale /= TimeScaleFactor;
            ClampTimeScale();
        }

        public static void ClampTimeScale()
        {
            CurrentTimeScale = math.clamp(CurrentTimeScale, MinCurrentTimeScale, MaxCurrentTimeScale);
            Time.timeScale = CurrentTimeScale;
            Time.fixedDeltaTime = FixedDeltaTimeFactor * Time.timeScale;
        }
    }
}