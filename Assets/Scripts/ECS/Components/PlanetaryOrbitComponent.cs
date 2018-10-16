using System;
using ECS.Utils;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Components
{
    public enum PlanetType
    {
        Planet,
        Moon
    }

    [Serializable]
    public struct PlanetaryOrbit : IComponentData
    {
#pragma warning disable 649
        [SerializeField] private float _eccentricity;
        [SerializeField] private float _rPericenter;
        [SerializeField] private float _orbitalPeriod;
        [SerializeField] private float _radius;
        [SerializeField] private float _axialTilt;
        [SerializeField] private float _rotPeriod;
        [SerializeField] private float _longitudeAscendingNode;
        [SerializeField] private float _massScale;
#pragma warning restore 649

        public PlanetType PlanetType;
        public float Eccentricity => _eccentricity;
        public float AxialTilt => _axialTilt;
        public float LongitudeAscendingNode => _longitudeAscendingNode;
        public float MassScale => _massScale;

        [NonSerialized] public float Radius;
        [NonSerialized] public float RPericenter;
        [NonSerialized] public float OrbitalPeriod;
        [NonSerialized] public float RotationPeriod;
        [NonSerialized] public float OrbitDt;
        [NonSerialized] public float CosOmega;
        [NonSerialized] public float SinOmega;
        [NonSerialized] public float Surface;
        [NonSerialized] public float K;

        public void Init(ref PlanetaryOrbit planetaryOrbit)
        {
            planetaryOrbit.Radius = PlanetType == PlanetType.Planet ? _radius : _radius / Scales.PlanetScaleFactor;
            planetaryOrbit.RPericenter = PlanetType == PlanetType.Planet ? _rPericenter * Scales.Au2Mu : _rPericenter;
            planetaryOrbit.OrbitalPeriod = _orbitalPeriod * Scales.Y2Tmu;
            planetaryOrbit.RotationPeriod = _rotPeriod * Scales.Y2Tmu;
            planetaryOrbit.OrbitDt = _orbitalPeriod * Scales.Y2Tmu / (2 * (Scales.N - 1));
            planetaryOrbit.CosOmega = math.cos(_longitudeAscendingNode);
            planetaryOrbit.SinOmega = math.sin(_longitudeAscendingNode);
            planetaryOrbit.Surface = math.sqrt(-(1 + _eccentricity) / math.pow(-1 + _eccentricity, 3)) *
                                     (float) math.PI * RPericenter * RPericenter;
            planetaryOrbit.K = 2 * planetaryOrbit.Surface /
                               (math.pow(1 + _eccentricity, 2) * OrbitalPeriod * RPericenter * RPericenter);
        }

        public unsafe void ThetaRange(float* angles)
        {
            var w = 0f;
            angles[0] = w;
            for (var i = 0; i < Scales.N - 2; i++)
            {
                var k1 = OrbitDt * Dthdt(w);
                var k2 = OrbitDt * Dthdt(w + k1 / 2);
                var k3 = OrbitDt * Dthdt(w + k2 / 2);
                var k4 = OrbitDt * Dthdt(w + k3);
                w += (k1 + 2 * k2 + 2 * k3 + k4) / 6;
                angles[i + 1] = w;
            }

            angles[Scales.N - 1] = (float) math.PI;
        }

        public unsafe float3 ParametricOrbit(float time, float* angles)
        {
            var theta = ThetaInt(time, angles);
            return ParametricOrbit(theta);
        }

        public float3 ParametricOrbit(float theta)
        {
            var costTheta = math.cos(theta);
            var sinTheta = math.sin(theta);

            var x = RPericenter * (1 + _eccentricity) / (1 + _eccentricity * costTheta) * costTheta;
            var z = RPericenter * (1 + _eccentricity) / (1 + _eccentricity * costTheta) * sinTheta;

            var xp = CosOmega * x - SinOmega * z;
            var yp = SinOmega * x + CosOmega * z;

            return new float3(xp, 0f, yp);
        }

        #region InternalCalculation

        private float Dthdt(float th)
        {
            return K * math.pow(1 + _eccentricity * math.cos(th), 2);
        }

        private unsafe float ThetaInt(float time, float* angles)
        {
            float theta0;
            time = time % OrbitalPeriod;

            if (time <= OrbitalPeriod / 2)
            {
                var i = time / OrbitDt;
                var i0 = math.clamp(math.floor(i), 0, Scales.N - 1);
                var i1 = math.clamp(math.ceil(i), 0, Scales.N - 1);

                if (math.abs(i0 - i1) < 0.0000000001f)
                {
                    theta0 = angles[(int) i0];
                }
                else
                {
                    theta0 = (angles[(int) i0] - angles[(int) i1]) / (i0 - i1) * i +
                             (i0 * angles[(int) i1] - angles[(int) i0] * i1) / (i0 - i1);
                }
            }
            else
            {
                time = -time + OrbitalPeriod;
                var i = time / OrbitDt;
                var i0 = math.clamp(math.floor(i), 0, Scales.N - 1);
                var i1 = math.clamp(math.ceil(i), 0, Scales.N - 1);

                if (math.abs(i0 - i1) < 0.0000000001f)
                {
                    theta0 = -angles[(int) i0] + 2 * (float) math.PI;
                }
                else
                {
                    theta0 = -(
                                 (angles[(int) i0] - angles[(int) i1]) / (i0 - i1) * i +
                                 (i0 * angles[(int) i1] - angles[(int) i0] * i1) / (i0 - i1)
                             ) + 2 * (float) math.PI;
                }
            }

            return theta0;
        }

        #endregion
    }

    [DisallowMultipleComponent]
    public class PlanetaryOrbitComponent : ComponentDataWrapper<PlanetaryOrbit>
    {
    }
}