#region

using System.Linq;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

#endregion

namespace Appalachia.Simulation.Trees.Rendering
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class TreePrefabInstance : AppalachiaBehaviour<TreePrefabInstance>
    {
        #region Fields and Autoproperties

        public TreePrefabInstanceData instanceData;

        #endregion

        /// <inheritdoc />
        protected override async AppaTask WhenEnabled()
        {
            await base.WhenEnabled();

            using (_PRF_WhenEnabled.Auto())
            {
                var lightProbeProxyVolume = FindObjectsOfType<LightProbeProxyVolume>()
                                           .OrderByDescending(lppv => lppv.boundsGlobal.size)
                                           .FirstOrDefault(
                                                lppv => lppv.boundsGlobal.Contains(transform.position)
                                            );

                var renderers = GetComponentsInChildren<MeshRenderer>();

                foreach (var r in renderers)
                {
                    if ((lightProbeProxyVolume != null) && (r.lightProbeProxyVolumeOverride == null))
                    {
                        r.lightProbeUsage = LightProbeUsage.UseProxyVolume;
                        r.lightProbeProxyVolumeOverride = lightProbeProxyVolume.gameObject;
                    }
                    else
                    {
                        r.lightProbeUsage = LightProbeUsage.BlendProbes;
                    }
#if UNITY_EDITOR

                    GameObjectUtility.SetStaticEditorFlags(r.gameObject, StaticEditorFlags.ContributeGI);
                    r.receiveGI = ReceiveGI.LightProbes;
#endif
                }
            }
        }
    }
}
