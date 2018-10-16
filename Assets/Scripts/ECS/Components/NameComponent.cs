using System;
using Unity.Entities;
using UnityEngine;

namespace ECS.Components
{
    public enum PlanetName
    {
        Mercury,
        Venus,
        Earth,
        Mars,
        Jupiter,
        Saturn,
        Uranus,
        Neptune,
        Moon
    }

    [Serializable]
    public struct Name : IComponentData
    {
        public PlanetName Value;
    }

    [DisallowMultipleComponent]
    public class NameComponent : ComponentDataWrapper<Name>
    {
    }
}