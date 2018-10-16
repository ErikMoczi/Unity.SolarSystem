using ECS.Components;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Utils.Helpers
{
    public static class LineRendererHelper
    {
        private const int Length = 250;

        public static void BaseInit(Position position, PlanetaryOrbit planetaryOrbit, LineRenderer lineRenderer)
        {
            var width = planetaryOrbit.Radius * 2f;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.positionCount = Length;

            SetPositions(position, planetaryOrbit, lineRenderer);
        }

        public static void CalculateByCamera(PlanetaryOrbit planetaryOrbit, LineRenderer lineRenderer)
        {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            var camPlaneDist = Camera.main.transform.position.y;
            var scaleLR = math.abs((new Vector3(camPlaneDist, camPlaneDist, camPlaneDist) / Scales.ThresDist).x);
            var width2 = Mathf.Min(65f, 1.5f * planetaryOrbit.Radius * scaleLR);
            lineRenderer.startWidth = width2;
            lineRenderer.endWidth = width2;
        }

        private static void SetPositions(Position position, PlanetaryOrbit planetaryOrbit, LineRenderer lineRenderer)
        {
            for (var i = 0; i < Length; i++)
            {
                lineRenderer.SetPosition(
                    i,
                    position.Value + planetaryOrbit.ParametricOrbit(
                        2 * (float) math.PI / (Length - 1) * i
                    )
                );
            }
        }
    }
}