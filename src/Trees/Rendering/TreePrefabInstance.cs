#region

using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;



#endregion

namespace Appalachia.Simulation.Trees.Rendering
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class TreePrefabInstance : MonoBehaviour
    {
        public TreePrefabInstanceData instanceData;

        public void OnEnable()
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

                UnityEditor.GameObjectUtility.SetStaticEditorFlags(
                    r.gameObject,
                    UnityEditor.StaticEditorFlags.ContributeGI
                );
                r.receiveGI = ReceiveGI.LightProbes;
#endif
            }
        }
    }
}
