using Appalachia.Core.Scriptables;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Snapshot
{
    public class SnapshotShaders : SelfSavingSingletonScriptableObject<SnapshotShaders>
    {
        public Shader albedo;
        public Shader normal;
        public Shader surface;
        public Shader sample;
    }
}
