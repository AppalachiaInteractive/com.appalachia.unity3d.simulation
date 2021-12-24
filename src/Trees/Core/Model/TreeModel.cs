#if UNITY_EDITOR

#region

using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Core.Math.Geometry;
using Appalachia.Editing.Debugging.Handle;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Core.Interfaces;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Appalachia.Simulation.Trees.Core.Model
{
    [CallStaticConstructorInEditor]
    [ExecuteAlways]
    public class TreeModel : EditorOnlyFrustumCulledBehaviour, ITreeModel
    {
        static TreeModel()
        {
            TreeGizmoStyle.InstanceAvailable += i => _treeGizmoStyle = i;
        }

        #region Static Fields and Autoproperties

        public static Func<ScriptableObject> _gizmoRetriever;

        private static TreeGizmoStyle _treeGizmoStyle;

        #endregion

        #region Fields and Autoproperties

        private ISpeciesDataProvider _container;
        [HideInInspector] public RaycastHit[] _hits = new RaycastHit[24];
        [HideInInspector] public ScriptableObject _containerSO;
        [HideInInspector] public GameObject _visible;
        [HideInInspector] public MeshRenderer _visibleMeshRenderer;
        [HideInInspector] public Mesh _visibleMesh;
        [HideInInspector] public int _visibleIndividualIndex;
        [HideInInspector] public AgeType _visibleAge;
        [HideInInspector] public StageType _visibleStage;

        private ISpeciesGizmoDelegate _gizmos;
        [HideInInspector] public ScriptableObject _gizmosSO;

        [HideInInspector] public bool visible;

        [DisableIf(nameof(_missingContainer))]
        [BoxGroup("Individual Selection"), InlineProperty, HideLabel]
        public ModelSelection selection;

        [BoxGroup("SceneView")] public bool autoUpdateSceneView;

        [FoldoutGroup("Gizmos")]
        [HideReferenceObjectPicker]
        [InlineEditor]
        [Title("Gizmo Style")]
        public TreeGizmoStyle style;

        [BoxGroup("Gizmos/Shape Matrix")]
        [EnableIf(nameof(_showDrawShapeMatrix))]
        public bool drawShapeMatrix;

        [BoxGroup("Gizmos/Shape Matrix")]
        [EnableIf(nameof(_showDrawParentShapeMatrix))]
        public bool drawParentShapeMatrix;

        [BoxGroup("Gizmos/Shape Matrix")]
        [ShowIf(nameof(drawShapeMatrix))]
        [EnableIf(nameof(_showDrawShapeMatrix))]
        public TreeComponentType shapeType;

        [BoxGroup("Gizmos/Shape Matrix")]
        [ShowIf(nameof(drawShapeMatrix))]
        [EnableIf(nameof(_showDrawShapeMatrix))]
        [PropertyRange(0, nameof(maxShapeIndex))]
        public int shapeIndex;

        [BoxGroup("Gizmos/Mesh")]
        [TitleGroup("Gizmos/Mesh/Vertex Index")]
        [HorizontalGroup("Gizmos/Mesh/Vertex Index/A"), HideLabel]
        [PropertyRange(0, nameof(maxMeshIndex))]
        public int vertexIndex;

        #endregion

        public Matrix4x4 parentShapeMatrix => parentShapeData.effectiveMatrix;

        [BoxGroup("Shape Info & Matrix Data")]
        [ShowIf(nameof(_showShapeMatrixOptions))]
        [ReadOnly]
        public Matrix4x4 shapeMatrix =>
            container.GetShapeMatrix(
                selection.individualSelection,
                selection.ageSelection,
                selection.stageSelection,
                shapeType,
                ref shapeIndex
            );

        [BoxGroup("Shape Info & Matrix Data")]
        [ShowIf(nameof(_showShapeMatrixOptions))]
        [ReadOnly]
        public ShapeData parentShapeData =>
            container.GetShapeDataByID(
                selection.individualSelection,
                selection.ageSelection,
                selection.stageSelection,
                shapeData.parentShapeID
            );

        [BoxGroup("Shape Info & Matrix Data")]
        [ShowIf(nameof(_showShapeMatrixOptions))]
        [ReadOnly]
        public ShapeData shapeData =>
            container.GetShapeData(
                selection.individualSelection,
                selection.ageSelection,
                selection.stageSelection,
                shapeType,
                ref shapeIndex
            );

        [BoxGroup("Shape Info & Matrix Data")]
        [ShowIf(nameof(_showShapeMatrixOptions))]
        [ReadOnly]
        public ShapeGeometryData shapeGeometry =>
            container.GetShapeGeometry(
                selection.individualSelection,
                selection.ageSelection,
                selection.stageSelection,
                shapeType,
                ref shapeIndex
            );

        internal bool _missingContainer => container == null;

        private bool _showDrawParentShapeMatrix => _showDrawShapeMatrix && drawShapeMatrix;

        private bool _showDrawShapeMatrix => selection.HasIndividual;

        private bool _showShapeMatrixOptions => _showDrawShapeMatrix && drawShapeMatrix;

        private bool CanDecrement => vertexIndex > 0;
        private bool CanIncrement => vertexIndex < (maxMeshIndex - 1);

        private int maxMeshIndex => _visibleMesh == null ? 0 : _visibleMesh.vertexCount;

        private int maxShapeIndex =>
            container.GetMaxShapeIndex(
                selection.individualSelection,
                selection.ageSelection,
                selection.stageSelection,
                shapeType
            );

        private ISpeciesDataProvider container
        {
            get
            {
                if (_container == null)
                {
                    if (_containerSO == null)
                    {
                        return null;
                    }

                    _container = _containerSO as ISpeciesDataProvider;
                }

                return _container;
            }
        }

        private ISpeciesGizmoDelegate gizmos
        {
            get
            {
                if (_gizmos == null)
                {
                    if (_gizmosSO == null)
                    {
                        return null;
                    }

                    _gizmos = _gizmosSO as ISpeciesGizmoDelegate;
                }

                return _gizmos;
            }
        }

        #region Event Functions

        private void Update()
        {
            var repaint = false;

            if (style == null)
            {
                style = _treeGizmoStyle;
            }

            if (visible)
            {
                if (_visible == null)
                {
                    if (selection.HasIndividual)
                    {
                        _visible = container.GetIndividual(
                            selection.individualSelection,
                            selection.ageSelection,
                            selection.stageSelection
                        );

                        if (_visible == null)
                        {
                            return;
                        }

                        var lodGroup = _visible.GetComponent<LODGroup>();
                        var lods = lodGroup.GetLODs();
                        _visibleMeshRenderer = lods[0].renderers[0] as MeshRenderer;
                        var mf = _visibleMeshRenderer.GetComponent<MeshFilter>();
                        _visibleMesh = mf.sharedMesh;

                        _visibleIndividualIndex = selection.individualSelection;
                        _visibleAge = selection.ageSelection;
                        _visibleStage = selection.stageSelection;

                        _visible.transform.SetParent(transform, false);

                        RecalculateLODBounds(lodGroup, selection.stageSelection);

                        repaint = true;
                    }
                }
                else
                {
                    if (selection.HasIndividual)
                    {
                        if ((_visibleIndividualIndex != selection.individualSelection) ||
                            (_visibleAge != selection.ageSelection) ||
                            (_visibleStage != selection.stageSelection))
                        {
                            Object.DestroyImmediate(_visible);

                            _visible = container.GetIndividual(
                                selection.individualSelection,
                                selection.ageSelection,
                                selection.stageSelection
                            );

                            if (_visible != null)
                            {
                                var lodGroup = _visible.GetComponent<LODGroup>();
                                var lods = lodGroup.GetLODs();

                                if ((lods.Length > 0) && (lods[0].renderers.Length > 0))
                                {
                                    _visibleMeshRenderer = lods[0].renderers[0] as MeshRenderer;
                                    var mf = _visibleMeshRenderer.GetComponent<MeshFilter>();
                                    _visibleMesh = mf.sharedMesh;

                                    _visibleIndividualIndex = selection.individualSelection;
                                    _visibleAge = selection.ageSelection;
                                    _visibleStage = selection.stageSelection;

                                    _visible.transform.SetParent(transform, false);

                                    RecalculateLODBounds(lodGroup, selection.stageSelection);
                                }
                            }

                            repaint = true;
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < transform.childCount; i++)
                {
                    Object.DestroyImmediate(transform.GetChild(i).gameObject);
                }

                repaint = true;
            }

            if ((selection.container == null) && (container != null))
            {
                selection.container = container;
            }
            else if ((container == null) && (selection.container != null))
            {
                _container = selection.container;
            }
            else if ((selection.container == null) && (container == null))
            {
                if (_containerSO != null)
                {
                    _container = _containerSO as ISpeciesDataProvider;
                }

                if (_container == null)
                {
                    Context.Log.Error("Need to set containers!");
                    Object.DestroyImmediate(gameObject);
                }
            }
            else if (selection.container != container)
            {
                selection.container = container;
            }

            var pos = transform.position;
            foreach (var terrain in Terrain.activeTerrains)
            {
                var terrainData = terrain.terrainData;

                var terrainMin = terrain.GetPosition();
                var terrainMax = terrainMin + terrainData.size;

                if ((pos.x >= terrainMin.x) &&
                    (pos.x < terrainMax.x) &&
                    (pos.z >= terrainMin.z) &&
                    (pos.z < terrainMax.z))
                {
                    var height = terrain.SampleHeight(pos) + terrainMin.y;

                    transform.position = new Vector3(pos.x, height, pos.z);
                    break;
                }
            }

            if (repaint)
            {
                if (autoUpdateSceneView)
                {
                    Frame(true);
                }

                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
        }

        private void OnDrawGizmos()
        {
            if (container == null)
            {
                return;
            }

            if (visible && container.drawGizmos && (selection != null) && selection.HasIndividual)
            {
                gizmos.DrawSpeciesGizmos(container, selection, transform, _visibleMesh);

                if (drawShapeMatrix)
                {
                    SmartHandles.DrawWireMatrix(
                        transform.position,
                        shapeMatrix,
                        _treeGizmoStyle.shapeMatrixScale,
                        _treeGizmoStyle.shapeMatrixSphereScale,
                        _treeGizmoStyle.shapeMatrixAlpha,
                        _treeGizmoStyle.shapeMatrixWorldAlpha,
                        _treeGizmoStyle.shapeMatrixSteps,
                        _treeGizmoStyle.shapeMatrixDistanceSteps
                    );

                    if (drawParentShapeMatrix)
                    {
                        SmartHandles.DrawWireMatrix(
                            transform.position,
                            parentShapeMatrix,
                            _treeGizmoStyle.shapeMatrixScale,
                            _treeGizmoStyle.shapeMatrixSphereScale,
                            _treeGizmoStyle.shapeMatrixAlpha,
                            _treeGizmoStyle.shapeMatrixWorldAlpha,
                            _treeGizmoStyle.shapeMatrixSteps,
                            _treeGizmoStyle.shapeMatrixDistanceSteps
                        );
                    }

                    var geom = shapeGeometry;

                    if (geom != null)
                    {
                        if (_treeGizmoStyle.drawShapeMatrixNormals)
                        {
                            _gizmos.DrawNormals(
                                transform,
                                _visibleMesh,
                                geom.modelVertexStart,
                                geom.modelVertexEnd
                            );
                        }

                        if (_treeGizmoStyle.drawShapeMatrixWeld)
                        {
                            var data = container.GetShapeData(
                                selection.individualSelection,
                                selection.ageSelection,
                                selection.stageSelection,
                                shapeType,
                                ref shapeIndex
                            );

                            var parentShape = container.GetShapeDataByID(
                                selection.individualSelection,
                                selection.ageSelection,
                                selection.stageSelection,
                                data.parentShapeID
                            );

                            var basePosition = transform.position;
                            var origin = basePosition + shapeMatrix.MultiplyPoint(Vector3.zero);

                            var vert = _visibleMesh.vertices;
                            var tri = _visibleMesh.triangles;

                            var parentGeo = parentShape.geometry[0];

                            foreach (var triangle in parentGeo.actualTriangles)
                            {
                                var v0 = basePosition + vert[tri[triangle * 3]];
                                var v1 = basePosition + vert[tri[(triangle * 3) + 1]];
                                var v2 = basePosition + vert[tri[(triangle * 3) + 2]];

                                _gizmos.DrawWeldGizmo(origin, v0, v1, v2);

                                var center = (v0 + v1 + v2) / 3f;
                                var direction = (center - origin).normalized;
                                var edge_ab = v1 - v0;
                                var edge_ac = v2 - v0;
                                var normal = Vector3.Cross(edge_ab, edge_ac);

                                var dot = Vector3.Dot(direction, normal);

                                //var frontFaceHit = dot < 0.0f;
                                //var backfaceHit = dot > 0.0f;

                                var plane = new Plane(normal, center);

                                var nearest = plane.ClosestPointOnPlane(origin);

                                var bary = new Barycentric(v0, v1, v2, nearest);

                                if (!bary.IsInside)
                                {
                                    var v0D = (v0 - nearest).sqrMagnitude;
                                    var v1D = (v1 - nearest).sqrMagnitude;
                                    var v2D = (v2 - nearest).sqrMagnitude;

                                    if ((v0D < v1D) && (v0D < v2D))
                                    {
                                        nearest = v0;
                                    }
                                    else if ((v1D < v0D) && (v1D < v2D))
                                    {
                                        nearest = v1;
                                    }
                                    else
                                    {
                                        nearest = v2;
                                    }
                                }

                                SmartHandles.DrawLine(origin, nearest, Color.red);
                            }
                        }
                    }
                }

                if (_treeGizmoStyle.drawVertexData)
                {
                    gizmos.DrawVertex(transform, _visibleMesh, ref vertexIndex);
                }
            }
        }

        #endregion

        public static TreeModel Create(ISpeciesDataProvider container, ISpeciesGizmoDelegate gizmos)
        {
            var go = new GameObject(GetTreeModelName(container));
            go.transform.position = Vector3.zero;
            go.tag = "EditorOnly";
            var model = go.AddComponent<TreeModel>();

            model._container = container;
            model._containerSO = container.GetSerializable();
            model._gizmos = gizmos;
            model._gizmosSO = gizmos.GetSerializable();

            model.selection = new ModelSelection(container)
            {
                individualSelection = 0, ageSelection = AgeType.Mature, stageSelection = StageType.Normal
            };

            return model;
        }

        public static TreeModel Find(ISpeciesDataProvider container)
        {
            var gos = Object.FindObjectsOfType<TreeModel>();

            for (var i = gos.Length - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    Object.DestroyImmediate(gos[i]);
                }

                if (i == 0)
                {
                    gos[i]._container = container;
                    gos[i]._containerSO = container.GetSerializable();

                    return gos[i];
                }
            }

            return null;
        }

        private static string GetTreeModelName(ISpeciesDataProvider container)
        {
            return ZString.Format("Tree Model - {0}", container.GetSpeciesName());
        }

        [HorizontalGroup("Gizmos/Mesh/Vertex Index/A")]
        [LabelText("Down"), EnableIf(nameof(CanDecrement))]
        [Button]
        private void DecrementVertexIndex()
        {
            vertexIndex -= 1;
        }

        [HorizontalGroup("Gizmos/Mesh/Vertex Index/A")]
        [LabelText("Up"), EnableIf(nameof(CanIncrement))]
        [Button]
        private void IncrementVertexIndex()
        {
            vertexIndex += 1;
        }

        private void RecalculateLODBounds(LODGroup lodGroup, StageType stage)
        {
            var lods = lodGroup.GetLODs();

            for (var i = 0; i < lodGroup.lodCount; i++)
            {
                if ((stage == StageType.Stump) || (stage == StageType.StumpRotted))
                {
                    var step = 1f / lodGroup.lodCount;
                    lods[i].screenRelativeTransitionHeight = 1f - ((1 + i) * step);
                }
            }

            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }

        #region ITreeModel Members

        [Button(ButtonSizes.Medium)]
        [GUIColor(0.79f, 0.68f, 0.19f)]
        [PropertyOrder(-100)]
        [DisableIf(nameof(_missingContainer))]
        public void OpenSpecies()
        {
            AssetDatabaseManager.OpenAsset(container as ScriptableObject);
        }

        public GameObject GameObject => gameObject;

        public bool MissingContainer => _missingContainer;

        #endregion
    }
}

#endif
