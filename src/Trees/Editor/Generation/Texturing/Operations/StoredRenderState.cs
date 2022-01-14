using Appalachia.Core.Objects.Root;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Operations
{
    public class StoredRenderState : AppalachiaSimpleBase
    {
        internal StoredRenderState()
        {
            GL.PushMatrix();
            fogState = RenderSettings.fog;
            srgbWrite = GL.sRGBWrite;
            Unsupported.SetRenderSettingsUseFogNoDirty(false);
            texture = RenderTexture.active;
        }

        #region Fields and Autoproperties

        private bool fogState;
        private bool srgbWrite;
        private RenderTexture texture;

        #endregion

        internal void Restore()
        {
            GL.PopMatrix();
            GL.sRGBWrite = srgbWrite;
            Unsupported.SetRenderSettingsUseFogNoDirty(fogState);
            RenderTexture.active = texture;
        }
    }
}
