using Appalachia.Simulation.Trees.Core;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Snapshot
{
    public class SnapshotShaders : SingletonAppalachiaTreeObject<SnapshotShaders>
    {
        public Shader albedo;
        public Shader normal;
        public Shader surface;
        public Shader sample;
    }
}
