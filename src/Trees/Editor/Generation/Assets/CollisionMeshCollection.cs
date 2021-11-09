using System;
using System.Collections.Generic;
using Appalachia.Core.Math.Geometry;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Utility.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    [Serializable]
    public class CollisionMeshCollection
    {
        public const string gameObject_baseName   = "COLLIDERS";
        public const string gameObject_woodName   = "COLLIDERS-WOOD";
        public const string gameObject_fruitName  = "COLLIDERS-FRUIT";
        public const string gameObject_fungusName = "COLLIDERS-FUNGUS";

        private const string meshSuffix = "_COLLISION-MESH";
        private const string meshSuffix_wood = "_COLLISION-MESH-WOOD-";

        public Mesh collisionMesh;
        public List<Mesh> decompositionMeshes;
        public List<SphereColliderData> fruitSpheres;
        public List<SphereColliderData> fungusSpheres;

        [SerializeField] private string decompositionMeshNamePart;
        
        public void CreateCollisionMeshes(string meshName)
        {
            collisionMesh = new Mesh {name = $"{meshName}{meshSuffix}"};
            decompositionMeshNamePart = $"{meshName}{meshSuffix_wood}";
        }

        public bool IsCorrectlyApplied(GameObject go, TreeGlobalSettings globals)
        {
            if (go.name != gameObject_baseName)
            {
               AppaLog.Warning("Prefab update required: Wrong collider parent name.");
                return false;
            }

            var tc = go.GetComponents<Collider>();

            if (tc.Length > 0)
            {
               AppaLog.Warning("Prefab update required: Extra mesh collider.");
                return false;
            }
            
            var requiredChildCount = 1;

            var requireFruit = false;
            var requireFungus = false;

            if ((fruitSpheres != null) && (fruitSpheres.Count > 0))
            {
                requireFruit = true;
                requiredChildCount += 1;
            }

            if ((fungusSpheres != null) && (fungusSpheres.Count > 0))
            {
                requireFungus = true;
                requiredChildCount += 1;
            }

            var childCount = go.transform.childCount;

            if (childCount != requiredChildCount)
            {
               AppaLog.Warning("Prefab update required: Wrong mesh collider count.");
                return false;
            }

            {
                var child = go.transform.GetChild(0);
                if (child.name != gameObject_woodName)
                {
                   AppaLog.Warning("Prefab update required: Wrong mesh collider child name.");
                    return false;
                }

                var colliders = child.GetComponents<MeshCollider>();
                var models = decompositionMeshes;

                if (colliders.Length != models.Count)
                {
                   AppaLog.Warning("Prefab update required: Wrong mesh collider count.");
                   return false;
                }                

                colliders = child.GetComponents<MeshCollider>();

                if (models.Count == 1)
                {
                    if (collisionMesh.vertexCount <= decompositionMeshes[0].vertexCount)
                    {
                        if (colliders[0].sharedMesh != collisionMesh)
                        {
                           AppaLog.Warning("Prefab update required: Collision mesh not correct.");
                            return false;
                        }
                    }
                    else
                    {
                        if (colliders[0].sharedMesh != decompositionMeshes[0])
                        {
                           AppaLog.Warning("Prefab update required: Collision mesh not correct.");
                            return false;
                        }
                    }

                    if (colliders[0].cookingOptions != (MeshColliderCookingOptions) ~0)
                    {
                       AppaLog.Warning("Prefab update required: Collision mesh cooking settings not correct.");
                        return false;
                    }
                    if (colliders[0].convex != true) 
                    {
                       AppaLog.Warning("Prefab update required: Mesh collider not convex.");
                        return false;
                        
                    }

                    if (colliders[0].sharedMaterial != globals.woodMaterial)
                    {
                       AppaLog.Warning("Prefab update required: Wrong material on wood sphere.");
                        return false;
                    }
                }
                else
                {
                    for (var i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].cookingOptions != (MeshColliderCookingOptions) ~0)
                        {
                           AppaLog.Warning("Prefab update required: Collision mesh cooking settings not correct.");
                            return false;
                        }

                        if (colliders[i].sharedMesh != models[i])
                        {
                           AppaLog.Warning("Prefab update required: Collision mesh not correct.");
                            return false;
                        }
                        if (colliders[i].convex != true)
                        {
                           AppaLog.Warning("Prefab update required: Mesh collider not convex.");
                            return false;
                        }

                        if (colliders[i].sharedMaterial != globals.woodMaterial)
                        {
                           AppaLog.Warning("Prefab update required: Wrong material on wood collider.");
                            return false;
                        }
                    }
                }
            }

            if (requireFungus)
            {
                var child = go.transform.GetChild(1);
                
                child.name = gameObject_fungusName;
                
                var colliders = child.GetComponents<SphereCollider>();
                var models = fruitSpheres;

                if (colliders.Length != models.Count)
                {
                   AppaLog.Warning("Prefab update required: Wrong fruit sphere count.");
                    return false;
                }

                colliders = child.GetComponents<SphereCollider>();

                for (var i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].center != models[i].center)
                    {
                       AppaLog.Warning("Prefab update required: Bad center on fruit sphere.");
                        return false;
                    }

                    if (Math.Abs(colliders[i].radius - models[i].radius) > float.Epsilon)
                    {
                       AppaLog.Warning("Prefab update required: Bad radius on fruit sphere.");
                        return false;
                    }

                    if (colliders[i].sharedMaterial != globals.mushroomMaterial)
                    {
                       AppaLog.Warning("Prefab update required: Wrong material on fungus sphere.");
                        return false;
                    }
                }
            }
            
            if (requireFruit)
            {
                var child = go.transform.GetChild(2);

                child.name = gameObject_fruitName;

                var colliders = child.GetComponents<SphereCollider>();
                var models = fungusSpheres;

                if (colliders.Length != models.Count)
                {
                   AppaLog.Warning("Prefab update required: Wrong fruit sphere count.");
                    return false;
                }

                colliders = child.GetComponents<SphereCollider>();

                for (var i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].center != models[i].center)
                    {
                       AppaLog.Warning("Prefab update required: Bad center on fruit sphere.");
                        return false;
                    }
                    if (Math.Abs(colliders[i].radius - models[i].radius) > float.Epsilon)
                    {
                       AppaLog.Warning("Prefab update required: Bad radius on fruit sphere.");
                        return false;
                    }

                    if (colliders[i].sharedMaterial != globals.fruitMaterial)
                    {
                       AppaLog.Warning("Prefab update required: Wrong material on fruit sphere.");
                        return false;
                    }
                }
            }
            
            return true;
        }

        public void SaveMeshes(Func<Mesh, Mesh> saveFunc)
        {
            var main = (collisionMesh != null) && (collisionMesh.vertexCount > 0);

            if (main)
            {
                CleanMesh(collisionMesh);
                collisionMesh = saveFunc(collisionMesh);
            }

            if (decompositionMeshes != null)
            {
                for (var i = 0; i < decompositionMeshes.Count; i++)
                {
                    CleanMesh(decompositionMeshes[i]);
                    decompositionMeshes[i].name = $"{decompositionMeshNamePart}{i}";
                    decompositionMeshes[i]  = saveFunc(decompositionMeshes[i]);
                }
            }
        }
        

        private static void CleanMesh(Mesh m)
        {
            m.colors = null;
            m.uv = null;
            m.uv2 = null;
            m.uv3 = null;
            m.uv4 = null;
            m.Optimize();
        }

        public void Apply(GameObject go, TreeGlobalSettings globals)
        {
            go.name = gameObject_baseName;
            var tc = go.GetComponents<Collider>();

            for (var i = tc.Length - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(tc[i]);
            }

            
            var requiredChildCount = 1;

            var requireFruit = false;
            var requireFungus = false;

            if ((fruitSpheres != null) && (fruitSpheres.Count > 0))
            {
                requireFruit = true;
                requiredChildCount += 1;
            }

            if ((fungusSpheres != null) && (fungusSpheres.Count > 0))
            {
                requireFungus = true;
                requiredChildCount += 1;
            }

            var childCount = go.transform.childCount;

            if (childCount > requiredChildCount)
            {
                for (var i = childCount - 1; i >= requiredChildCount; i--)
                {
                    Object.DestroyImmediate(go.transform.GetChild(i).gameObject);
                }
            }
            else if (childCount < requiredChildCount)
            {
                for (var i = childCount; i < requiredChildCount; i++)
                {
                    var newChild = new GameObject();
                    newChild.transform.SetParent(go.transform, false);
                }
            }

            {
                var child = go.transform.GetChild(0);
                child.name = gameObject_woodName;

                var colliders = child.GetComponents<MeshCollider>();
                var models = decompositionMeshes;
                
                if (colliders.Length > models.Count)
                {
                    for (var i = colliders.Length - 1; i >= models.Count; i--)
                    {
                        Object.DestroyImmediate(colliders[i]);
                    }
                }
                else if (colliders.Length < models.Count)
                {
                    for (var i = colliders.Length; i < models.Count; i++)
                    {
                        child.gameObject.AddComponent<MeshCollider>();
                    }
                }

                colliders = child.GetComponents<MeshCollider>();
                
                if (models.Count == 1)
                {
                    if (collisionMesh.vertexCount <= decompositionMeshes[0].vertexCount)
                    {
                        colliders[0].sharedMesh = collisionMesh;
                    }
                    else
                    {
                        colliders[0].sharedMesh = decompositionMeshes[0];
                    }
                    
                    colliders[0].cookingOptions = (MeshColliderCookingOptions) ~0;
                    colliders[0].convex = true;
                    colliders[0].sharedMaterial = globals.woodMaterial;
                }
                else
                {
                    for (var i = 0; i < colliders.Length; i++)
                    {
                        colliders[i].sharedMesh = models[i];
                        colliders[i].cookingOptions = (MeshColliderCookingOptions) ~0;
                        colliders[i].convex = true;
                        colliders[i].sharedMaterial = globals.woodMaterial;
                    }
                }

              
            }

            if (requireFungus)
            {
                var child = go.transform.GetChild(1);
                
                child.name = gameObject_fungusName;
                
                var colliders = child.GetComponents<SphereCollider>();
                var models = fruitSpheres;

                if (colliders.Length > models.Count)
                {
                    for (var i = colliders.Length - 1; i >= models.Count; i--)
                    {
                        Object.DestroyImmediate(colliders[i]);
                    }
                }
                else if (colliders.Length < models.Count)
                {
                    for (var i = colliders.Length; i < models.Count; i++)
                    {
                        child.gameObject.AddComponent<SphereCollider>();
                    }
                }

                colliders = child.GetComponents<SphereCollider>();

                for (var i = 0; i < colliders.Length; i++)
                {
                    colliders[i].center = models[i].center;
                    colliders[i].radius = models[i].radius;
                    colliders[i].sharedMaterial = globals.mushroomMaterial;
                }
            }
            
            if (requireFruit)
            {
                var child = go.transform.GetChild(2);

                child.name = gameObject_fruitName;

                var colliders = child.GetComponents<SphereCollider>();
                var models = fungusSpheres;

                if (colliders.Length > models.Count)
                {
                    for (var i = colliders.Length - 1; i >= models.Count; i--)
                    {
                        Object.DestroyImmediate(colliders[i]);
                    }
                }
                else if (colliders.Length < models.Count)
                {
                    for (var i = colliders.Length; i < models.Count; i++)
                    {
                        child.gameObject.AddComponent<SphereCollider>();
                    }
                }

                colliders = child.GetComponents<SphereCollider>();

                for (var i = 0; i < colliders.Length; i++)
                {
                    colliders[i].center = models[i].center;
                    colliders[i].radius = models[i].radius;
                    colliders[i].sharedMaterial = globals.fruitMaterial;
                }
            }
        }
        
        /*public static bool GenerateCollisionMeshForStage(TreeColliderType colliderType, StageType stage)
        {
            switch (colliderType)
            {
                case TreeColliderType.Trunk:
                {
                    return true;
                }
                case TreeColliderType.Branches:
                {
                    switch (stage)
                    {
                        case StageType.Felled:
                        case StageType.FelledBare:
                        case StageType.FelledBareRotted:
                        case StageType.DeadFelled:
                        case StageType.DeadFelledRotted:
                        case StageType.Normal:
                            return true;
                        case StageType.Stump:
                        case StageType.StumpRotted:
                        case StageType.Dead:
                        default:
                            return false;
                    }
                }
                case TreeColliderType.Fungus:
                {
                    return true;
                }
                    
                case TreeColliderType.Fruit:
                    
                    switch (stage)
                    {
                        case StageType.Felled:
                        case StageType.Normal:
                            return true;
                        case StageType.Stump:
                        case StageType.StumpRotted:
                        case StageType.FelledBare:
                        case StageType.FelledBareRotted:
                        case StageType.DeadFelled:
                        case StageType.DeadFelledRotted:
                        case StageType.Dead:
                        default:
                            return false;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(colliderType), colliderType, null);
            }

        }*/

        /*public Mesh GetCollisionMeshByType(TreeColliderType colliderType)
        {
            switch (colliderType)
            {
                case TreeColliderType.Trunk:
                    return trunkCollisionMesh;
                case TreeColliderType.Branches:
                    return branchesCollisionMesh;
                case TreeColliderType.Fungus:
                    return fungusCollisionMesh;    
                case TreeColliderType.Fruit:
                    return fruitCollisionMesh;
                default:
                    throw new ArgumentOutOfRangeException(nameof(colliderType), colliderType, null);
            }
        }*/
        
    }
}
