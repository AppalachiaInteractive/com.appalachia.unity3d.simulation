using Appalachia.Core.Scriptables;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Snapshot
{
    public class SnapshotShaders : SingletonAppalachiaObject<SnapshotShaders>
    {
        public Shader albedo;
        public Shader normal;
        public Shader surface;
        public Shader sample;
    }
}
