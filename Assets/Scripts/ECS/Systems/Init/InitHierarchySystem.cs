using ECS.Systems.Init.PlanetSystem;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.Jobs;

namespace ECS.Systems.Init
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(InitPlanetSystem))]
    public sealed class InitHierarchySystem : ComponentSystem
    {
#pragma warning disable 649
        private struct Data
        {
            public readonly int Length;
            [ReadOnly] public EntityArray Entities;
            public TransformAccessArray TransformAccessArray;
        }

        [Inject] private Data _data;
#pragma warning restore 649

        private EntityArchetype _entityArchetype;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _entityArchetype = EntityManager.CreateArchetype(typeof(Attach));
        }

        protected override void OnUpdate()
        {
            for (var i = 0; i < _data.Length; i++)
            {
                var parent = _data.TransformAccessArray[i].parent;
                if (parent != null)
                {
                    var gameObjectEntity = parent.GetComponent<GameObjectEntity>();
                    if (gameObjectEntity != null)
                    {
                        PostUpdateCommands.CreateEntity(_entityArchetype);
                        PostUpdateCommands.SetComponent(new Attach
                        {
                            Parent = gameObjectEntity.Entity,
                            Child = _data.Entities[i],
                        });
                    }
                }
            }
        }
    }
}