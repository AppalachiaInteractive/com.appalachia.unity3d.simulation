using System;
using System.Collections.Generic;
using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.Generation.Spline;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Snapshot
{
    [CallStaticConstructorInEditor]
    public static class SnapshotRenderer
    {
        public enum SnapshotMode
        {
            Lit,
            Albedo,
            Normal,
            Surface,
            Sample
        }

        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static SnapshotRenderer()
        {
            SnapshotShaders.InstanceAvailable += i => _snapshotShaders = i;
        }

        #region Static Fields and Autoproperties

        private static Dictionary<Color, GUIStyle> backgrounds = new Dictionary<Color, GUIStyle>();

        private static SnapshotShaders _snapshotShaders;

        #endregion

        public static Color GetBackgroundForRenderMode(SnapshotMode mode)
        {
            switch (mode)
            {
                case SnapshotMode.Albedo:
                    return new Color(0.0f, 0.0f, 0.0f, 0.0f);
                case SnapshotMode.Normal:
                    return new Color(0.5f, 0.5f, 1.0f, 1.0f);
                case SnapshotMode.Surface:
                    return new Color(0.0f, 1.0f, 0.0f, 0.0f);
                case SnapshotMode.Lit:
                case SnapshotMode.Sample:
                    return Color.white;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public static Texture RenderMeshPreview(
            PreviewRenderUtility previewRenderUtility,
            BranchDataContainer branchData,
            BranchSnapshotParameters parameters,
            Rect rect)
        {
            return RenderMeshPreview(
                previewRenderUtility,
                branchData,
                parameters,
                rect,
                GUIStyle.none,
                Color.black,
                true
            );
        }

        public static Texture RenderMeshPreview(
            PreviewRenderUtility previewRenderUtility,
            BranchDataContainer branchData,
            BranchSnapshotParameters parameters,
            Rect rect,
            Color backgroundColor)
        {
            var backround = GetBackgroundGUIStyle(backgroundColor);

            return RenderMeshPreview(
                previewRenderUtility,
                branchData,
                parameters,
                rect,
                backround,
                backgroundColor,
                true
            );
        }

        public static Texture RenderMeshPreview(
            BranchDataContainer branchData,
            BranchSnapshotParameters parameters,
            Vector2 textureSize,
            SnapshotMode mode)
        {
            var bgColor = GetBackgroundForRenderMode(mode);

            var backround = GetBackgroundGUIStyle(bgColor);

            SetReplacementShader(branchData.renderUtility, mode);

            var rect = new Rect(0, 0, textureSize.x, textureSize.y);

            var sRGB = mode == SnapshotMode.Albedo;

            foreach (var material in branchData.branchAsset.materials)
            {
                if (sRGB)
                {
                    material.EnableKeyword("_GAMMA_TO_LINEAR");
                }
                else
                {
                    material.DisableKeyword("_GAMMA_TO_LINEAR");
                }
            }

            return RenderMeshPreview(
                branchData.renderUtility,
                branchData,
                parameters,
                rect,
                backround,
                bgColor,
                sRGB
            );
        }

        public static void SetReplacementShader(PreviewRenderUtility renderUtility, SnapshotMode mode)
        {
            switch (mode)
            {
                case SnapshotMode.Lit:
                    renderUtility.camera.ResetReplacementShader();
                    break;
                case SnapshotMode.Albedo:
                    renderUtility.camera.SetReplacementShader(_snapshotShaders.albedo, "");
                    break;
                case SnapshotMode.Normal:
                    renderUtility.camera.SetReplacementShader(_snapshotShaders.normal, "");
                    break;
                case SnapshotMode.Surface:
                    renderUtility.camera.SetReplacementShader(_snapshotShaders.surface, "");
                    break;
                case SnapshotMode.Sample:
                    renderUtility.camera.SetReplacementShader(_snapshotShaders.sample, "");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private static GUIStyle GetBackgroundGUIStyle(Color backgroundColor)
        {
            if (backgrounds == null)
            {
                backgrounds = new Dictionary<Color, GUIStyle>();
            }

            if (!backgrounds.ContainsKey(backgroundColor))
            {
                var bg = new GUIStyle();

                backgrounds.Add(backgroundColor, bg);
            }

            var style = backgrounds[backgroundColor];

            if (style.normal.background == null)
            {
                style.normal.background = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
                style.normal.background.SetPixel(0, 0, backgroundColor);
                style.normal.background.Apply();
                style.border = new RectOffset(0, 0, 0, 0);

                backgrounds[backgroundColor] = style;
            }

            return style;
        }

        private static Texture RenderMeshPreview(
            PreviewRenderUtility previewRenderUtility,
            BranchDataContainer branchData,
            BranchSnapshotParameters parameters,
            Rect rect,
            GUIStyle guiStyle,
            Color backgroundColor,
            bool sRGB)
        {
            previewRenderUtility.BeginPreview(rect, guiStyle);

            RenderMeshPreviewInternal(previewRenderUtility, branchData, parameters, backgroundColor, sRGB);

            var resultRender = previewRenderUtility.EndPreview();

            return resultRender;
        }

        private static void RenderMeshPreviewInternal(
            PreviewRenderUtility previewRenderUtility,
            BranchDataContainer branchData,
            BranchSnapshotParameters parameters,
            Color backgroundColor,
            bool sRGB)
        {
            if ((branchData == null) ||
                (branchData.branchAsset == null) ||
                (branchData.branchAsset.mesh == null) ||
                (previewRenderUtility == null) ||
                (branchData.branch == null) ||
                (branchData.branch.shapes == null) ||
                (branchData.branch.shapes.trunkShapes == null) ||
                (branchData.branch.shapes.trunkShapes.Count == 0))
            {
                return;
            }

            Vector3 cameraPosition;
            Quaternion cameraRotation;
            Vector3 modelPosition;
            Quaternion modelRotation;
            float cameraSize;
            float cameraHalfSize;
            float cameraDistance;

            if (parameters.lockCameraPerspective)
            {
                cameraPosition = parameters.lockedCameraPosition;
                cameraRotation = parameters.lockedCameraRotation;
                modelPosition = parameters.lockedModelPosition;
                modelRotation = parameters.lockedModelRotation;
                cameraSize = parameters.lockedCameraSize;
                cameraHalfSize = cameraSize * .5f;
                cameraDistance = parameters.lockedCameraDistance;
            }
            else
            {
                var bounds = branchData.branchAsset.mesh.bounds;

                if (parameters.focusOnTrunk)
                {
                    var trunk = branchData.branch.shapes.trunkShapes[0];

                    var half = SplineModeler.GetPositionAtTime(trunk.spline, .5f);
                    bounds.center = half; //trunk.matrix.MultiplyPoint(half);
                }

                var target = bounds.center;

                modelRotation = Quaternion.Euler(parameters.rotationOffset.y, 0, 0) *
                                Quaternion.Euler(0,                           parameters.rotationOffset.x, 0);

                modelPosition = modelRotation * -target;

                cameraSize = bounds.size.magnitude;
                cameraHalfSize = bounds.extents.magnitude;
                cameraDistance = 4.0f * cameraSize * (2 - parameters.translationDistance);

                cameraPosition = -Vector3.forward * cameraDistance;
                cameraPosition.x = bounds.max.x * -parameters.translationXOffset;
                cameraPosition.y = bounds.max.y * parameters.translationYOffset;
                cameraRotation = Quaternion.identity;

                parameters.lockedCameraPosition = cameraPosition;
                parameters.lockedCameraRotation = cameraRotation;
                parameters.lockedModelPosition = modelPosition;
                parameters.lockedModelRotation = modelRotation;
                parameters.lockedCameraSize = cameraSize;
                parameters.lockedCameraDistance = cameraDistance;
            }

            previewRenderUtility.camera.orthographicSize = cameraSize * (2 - parameters.translationDistance);
            previewRenderUtility.camera.nearClipPlane = cameraDistance - (cameraHalfSize * 1.1f);
            previewRenderUtility.camera.farClipPlane = cameraDistance + (cameraHalfSize * 1.1f);
            previewRenderUtility.camera.orthographic = parameters.orthographic;
            previewRenderUtility.camera.fieldOfView = parameters.fieldOfView;
            var transform = previewRenderUtility.camera.transform;
            transform.position = cameraPosition;
            transform.rotation = cameraRotation;

            previewRenderUtility.lights[0].intensity = parameters.primaryIntensity;
            previewRenderUtility.lights[0].transform.rotation = parameters.primaryRotation;
            previewRenderUtility.lights[1].intensity = parameters.secondaryIntensity;
            previewRenderUtility.lights[1].transform.rotation = parameters.secondaryRotation;

            previewRenderUtility.ambientColor = parameters.ambientColor;
            previewRenderUtility.camera.clearFlags = CameraClearFlags.Nothing;

            var old_sRGB = GL.sRGBWrite;

            GL.sRGBWrite = false;
            GL.Clear(true, true, backgroundColor);
            GL.sRGBWrite = sRGB;

            for (var i = 0; i < branchData.branchAsset.mesh.subMeshCount; i++)
            {
                var oldFog = RenderSettings.fog;

                Unsupported.SetRenderSettingsUseFogNoDirty(false);

                if (branchData.branchAsset.materials.Length > i)
                {
                    var material = branchData.branchAsset.materials[i];

                    if (material != null)
                    {
                        previewRenderUtility.DrawMesh(
                            branchData.branchAsset.mesh,
                            modelPosition,
                            modelRotation,
                            material,
                            i,
                            null
                        );

                        previewRenderUtility.Render();
                    }
                }

                Unsupported.SetRenderSettingsUseFogNoDirty(oldFog);
            }

            GL.sRGBWrite = old_sRGB;
        }
    }
}
