#if UNITY_EDITOR

#region

using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Math.Geometry;
using Appalachia.Editing.Core.Behaviours;
using Appalachia.Editing.Debugging.Handle;
using Appalachia.Simulation.Trees.Core.Interfaces;
using Appalachia.Simulation.Trees.Core.Shape;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Core.Model
{
    [ExecuteAlways]
    public class LogModel : EditorOnlyFrustumCulledBehaviour, ILogModel
    {
        private ILogDataProvider _container;
        [HideInInspector] public RaycastHit[] _hits = new RaycastHit[24];
        [HideInInspector] public ScriptableObject _containerSO;
        [HideInInspector] public GameObject _visible;
        [HideInInspector] public MeshRenderer _visibleMeshRenderer;
        [HideInInspector] public Mesh _visibleMesh;
        [HideInInspector] public int _visibleLogIndex;

        private ISpeciesGizmoDelegate _gizmos;
        [HideInInspector] public ScriptableObject _gizmosSO;

        [HideInInspector] public bool visible;

        internal bool _missingContainer => container == null;
        
        [DisableIf(nameof(_missingContainer))]
        [BoxGroup("Log Selection"), InlineProperty, HideLabel]
        public LogModelSelection selection;

        [BoxGroup("SceneView")]
        public bool autoUpdateSceneView;


        [FoldoutGroup("Gizmos")]
        [HideReferenceObjectPicker]
        [InlineEditor()]
        [Title("Gizmo Style")]
        public TreeGizmoStyle style;
        
        private bool _showDrawShapeMatrix => selection.HasIndividual;
        
        [BoxGroup("Gizmos/Shape Matrix")]
        [EnableIf(nameof(_showDrawShapeMatrix))]
        public bool drawShapeMatrix;
        
        private bool _showDrawParentShapeMatrix => _showDrawShapeMatrix && drawShapeMatrix;
        
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

        private int maxShapeIndex =>
            container.GetMaxShapeIndex(
                selection.instanceSelection,
                shapeType
            );

        private bool _showShapeMatrixOptions => _showDrawShapeMatrix && drawShapeMatrix;


        [BoxGroup("Shape Info & Matrix Data")]
        [ShowIf(nameof(_showShapeMatrixOptions))]
        [ReadOnly]
        public Matrix4x4 shapeMatrix =>
            container.GetShapeMatrix(
                selection.instanceSelection,
                shapeType,
                ref shapeIndex
            );
        
        public Matrix4x4 parentShapeMatrix
        {
            get { return parentShapeData.effectiveMatrix; }
        }
        
        [BoxGroup("Shape Info & Matrix Data")]
        [ShowIf(nameof(_showShapeMatrixOptions))]
        [ReadOnly]
        public ShapeGeometryData shapeGeometry =>
            container.GetShapeGeometry(
                selection.instanceSelection,
                shapeType,
                ref shapeIndex
            );


        [BoxGroup("Shape Info & Matrix Data")]
        [ShowIf(nameof(_showShapeMatrixOptions))]
        [ReadOnly]
        public ShapeData shapeData =>
            container.GetShapeData(
                selection.instanceSelection,
                shapeType,
                ref shapeIndex
            );
        
        [BoxGroup("Shape Info & Matrix Data")]
        [ShowIf(nameof(_showShapeMatrixOptions))]
        [ReadOnly]
        public ShapeData parentShapeData =>
            container.GetShapeDataByID(
                selection.instanceSelection,
                shapeData.parentShapeID
            );
        
        private ILogDataProvider container
        {
            get
            {
                if (_container == null)
                {
                    if (_containerSO == null)
                    {
                        return null;
                    }

                    _container = _containerSO as ILogDataProvider;
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
        
        [Button(ButtonSizes.Medium)]
        [GUIColor(0.79f, 0.68f, 0.19f)]
        [PropertyOrder(-100)]
        [DisableIf(nameof(_missingContainer))]
        public void OpenLog()
        {
            AssetDatabaseManager.OpenAsset(container as ScriptableObject);
        }

        public GameObject GameObject => gameObject;

        public bool MissingContainer => _missingContainer;

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
                        TreeGizmoStyle.instance.shapeMatrixScale,
                        TreeGizmoStyle.instance.shapeMatrixSphereScale,
                        TreeGizmoStyle.instance.shapeMatrixAlpha,
                        TreeGizmoStyle.instance.shapeMatrixWorldAlpha,
                        TreeGizmoStyle.instance.shapeMatrixSteps,
                        TreeGizmoStyle.instance.shapeMatrixDistanceSteps
                    );


                    if (drawParentShapeMatrix)
                    {
                        SmartHandles.DrawWireMatrix(
                            transform.position,
                            parentShapeMatrix,
                            TreeGizmoStyle.instance.shapeMatrixScale,
                            TreeGizmoStyle.instance.shapeMatrixSphereScale,
                            TreeGizmoStyle.instance.shapeMatrixAlpha,
                            TreeGizmoStyle.instance.shapeMatrixWorldAlpha,
                            TreeGizmoStyle.instance.shapeMatrixSteps,
                            TreeGizmoStyle.instance.shapeMatrixDistanceSteps
                        );
                    }

                    var geom = shapeGeometry;

                    if (geom != null)
                    {
                        if (TreeGizmoStyle.instance.drawShapeMatrixNormals)
                        {
                            _gizmos.DrawNormals(transform, _visibleMesh, geom.modelVertexStart, geom.modelVertexEnd);
                        }

                        if (TreeGizmoStyle.instance.drawShapeMatrixWeld)
                        {
                            var data = container.GetShapeData(
                                selection.instanceSelection,
                                shapeType,
                                ref shapeIndex
                            );
                            
                            var parentShape = container.GetShapeDataByID(
                                selection.instanceSelection,
                                data.parentShapeID
                            );

                            var basePosition = transform.position; 
                            var origin = basePosition + shapeMatrix.MultiplyPoint(Vector3.zero);

                            var vert = _visibleMesh.vertices;
                            var tri = _visibleMesh.triangles;

                            var parentGeo = parentShape.geometry[0];
                            
                            foreach(var triangle in parentGeo.actualTriangles)
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

                                float dot = Vector3.Dot(direction, normal);
                                
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
            }
        }
        
        

        private void Update()
        {
            var repaint = false;

            if (style == null)
            {
                style = TreeGizmoStyle.instance;
            }
            
            if (visible)
            {
                if (_visible == null)
                {
                    if (selection.HasIndividual)
                    {
                        _visible = container.GetLog(selection.instanceSelection);

                        if (_visible == null)
                        {
                            return;
                        }

                        var lodGroup = _visible.GetComponent<LODGroup>();
                        var lods = lodGroup.GetLODs();
                        _visibleMeshRenderer = lods[0].renderers[0] as MeshRenderer;
                        var mf = _visibleMeshRenderer.GetComponent<MeshFilter>();
                        _visibleMesh = mf.sharedMesh;

                        _visibleLogIndex = selection.instanceSelection;

                        _visible.transform.SetParent(transform, false);

                        RecalculateLODBounds(lodGroup);
                        
                        repaint = true;
                    }
                }
                else
                {
                    if (selection.HasIndividual)
                    {
                        if ((_visibleLogIndex != selection.instanceSelection))
                        {
                            DestroyImmediate(_visible);

                            _visible = container.GetLog(selection.instanceSelection);

                            if (_visible != null)
                            {
                                var lodGroup = _visible.GetComponent<LODGroup>();
                                var lods = lodGroup.GetLODs();
                                _visibleMeshRenderer = lods[0].renderers[0] as MeshRenderer;
                                var mf = _visibleMeshRenderer.GetComponent<MeshFilter>();
                                _visibleMesh = mf.sharedMesh;

                                _visibleLogIndex = selection.instanceSelection;

                                _visible.transform.SetParent(transform, false);

                                RecalculateLODBounds(lodGroup);

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
                    DestroyImmediate(transform.GetChild(i).gameObject);
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
                    _container = _containerSO as ILogDataProvider;
                }

                if (_container == null)
                {
                    Debug.LogError("Need to set containers!");
                    DestroyImmediate(gameObject);                    
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
                    Frame(gameObject);
                }

                EditorSceneManager.MarkSceneDirty(gameObject.scene);
                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();
            }
        }

        private static string GetLogModelName(ILogDataProvider container)
        {
            return $"Log Model - {container.GetName()}";
        }

        public static LogModel Find(ILogDataProvider container)
        {
            var gos = FindObjectsOfType<LogModel>();

            for (var i = gos.Length - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    DestroyImmediate(gos[i]);
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

        public static LogModel Create(ILogDataProvider container, ISpeciesGizmoDelegate gizmos)
        {
            var go = new GameObject(GetLogModelName(container));
            go.transform.position = Vector3.zero;
            go.tag = "EditorOnly";
            var model = go.AddComponent<LogModel>();

            model._container = container;
            model._containerSO = container.GetSerializable();
            model._gizmos = gizmos;
            model._gizmosSO = gizmos.GetSerializable();

            model.selection = new LogModelSelection(container)
            {
                instanceSelection = 0,
            };

            return model;
        }
        
        private static void Frame(GameObject go)
        {
            Bounds bounds = new Bounds();
            var first = true;
            
            foreach (var renderer in go.GetComponentsInChildren<MeshRenderer>())
            {
                var mf = renderer.GetComponent<MeshFilter>();
                mf.sharedMesh.RecalculateBounds();
                mf.sharedMesh.UploadMeshData(false);
                
                
                if (first)
                {
                    bounds = renderer.bounds;
                    first = false;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            var target = bounds.center;

            var view = SceneView.lastActiveSceneView;

            var camera = view.camera;

            view.LookAt(target, camera.transform.rotation, bounds.size.y*1.25f);
        }

        private void RecalculateLODBounds(LODGroup lodGroup)
        {
            lodGroup.RecalculateBounds();
        }
        public override EditorOnlyExclusionStyle exclusionStyle => EditorOnlyExclusionStyle.ObjectForceConflict;
    }
}

#endif