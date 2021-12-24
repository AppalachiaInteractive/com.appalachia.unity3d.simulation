#region

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Core.Model
{
#if UNITY_EDITOR

    public class TreeGizmoStyle : SingletonAppalachiaTreeObject<TreeGizmoStyle>
    {
       
        
        [FoldoutGroup("Splines")]
        public bool drawSplines = true;

        [FoldoutGroup("Splines"), PropertyRange(25, 150)]
        public int splineAccuracy = 100;
        
        [FoldoutGroup("Splines")]
        public Color splineColor = new Color(
            0.79f,
            0.68f,
            0.19f
        );
        
        [FoldoutGroup("Splines")]
        [PropertyRange(0f, 1f)]
        public float splineTransparency = .5f;

        [BoxGroup("Splines/Disks")]
        public bool drawSplineDisks = true;

        [BoxGroup("Splines/Disks")]
        [PropertyRange(1, nameof(splineAccuracy))]
        public int splineDiscInterval = 1;

        [BoxGroup("Splines/Disks")]
        [PropertyRange(1f, 3f)]
        public float diskOuterRadiusMultiplier = 1.5f;
        
        [BoxGroup("Splines/Disks")]
        public Color diskInnerColor = new Color(
            0.79f,
            0.68f,
            0.19f
        );
        
        [BoxGroup("Splines/Disks")]
        [PropertyRange(0f, 1f)]
        public float diskInnerTransparency = .5f;
        
        [BoxGroup("Splines/Disks")]
        public Color diskOuterColor = new Color(0.79f, 0.68f, 0.19f);

        [BoxGroup("Splines/Disks")]
        [PropertyRange(0f, 1f)]
        public float diskOuterTransparency = .5f;

        [FoldoutGroup("Nodes")]
        public bool drawNodes = true;

        [FoldoutGroup("Nodes")]
        [PropertyRange(0f, 2f)]
        public float nodeSize = 1f;
        
        [BoxGroup("Nodes/Colors")]
        public Color trunkColor = new Color(0.79f, 0.68f, 0.19f);

        [BoxGroup("Nodes/Colors")]
        public Color rootColor = new Color(0.79f, 0.68f, 0.19f);

        [BoxGroup("Nodes/Colors")]
        public Color branchColor = new Color(0.79f, 0.68f, 0.19f);

        [BoxGroup("Nodes/Colors")]
        public Color leafColor = new Color(0.79f, 0.68f, 0.19f);

        [BoxGroup("Nodes/Colors")]
        public Color knotColor = new Color(0.79f, 0.68f, 0.19f);

        [BoxGroup("Nodes/Colors")]
        public Color fruitColor = new Color(0.79f, 0.68f, 0.19f);

        [BoxGroup("Nodes/Colors")]
        public Color fungusColor = new Color(0.79f, 0.68f, 0.19f);
        
        [BoxGroup("Nodes/Colors")]
        [PropertyRange(0f, 1f)]
        public float nodeOpacity = .5f;


        [FoldoutGroup("Ground Offset")]
        public bool drawGroundOffset;

        [FoldoutGroup("Ground Offset")]
        [PropertyRange(1f, 50f)]
        public float groundOffsetRadius = 3f;

        [FoldoutGroup("Ground Offset")]
        [PropertyRange(1f, 100f)]
        public float groundOffsetRings = 30f;

        [FoldoutGroup("Ground Offset")]
        public Color groundOffsetBaseColor = Color.black;
        
        [FoldoutGroup("Ground Offset")]
        [PropertyRange(0f, 1f)]
        public float groundOffsetBaseOpacity = .5f;

        [FoldoutGroup("Ground Offset")]
        public Color groundOffsetRingColor = Color.red;
        
        [FoldoutGroup("Ground Offset")]
        [PropertyRange(0f, 1f)]
        public float groundOffsetRingOpacity = .5f;
        
        [FoldoutGroup("Normals")]
        public bool drawNormals;

        [FoldoutGroup("Normals")]
        [ShowIf(nameof(drawNormals))]
        public DrawNormalStyle normalStyle = DrawNormalStyle.All;

        [FoldoutGroup("Normals")]
        [PropertyRange(10, 1000)]
        public int normalLimit = 100;

        [FoldoutGroup("Normals")]
        [PropertyRange(0, 20000)]
        public int normalOffset;

        [FoldoutGroup("Normals")]
        [PropertyRange(0.01f, 5f)]
        public float normalLength = 1f;

        [FoldoutGroup("Normals")]
        public bool useNormalAsColor = true;

        [FoldoutGroup("Normals")]
        [HideIf(nameof(useNormalAsColor))]
        public Color normalColor = Color.cyan;

        [FoldoutGroup("Shape Matrix")]
        [PropertyRange(0.01f, 2f)]
        public float shapeMatrixScale = .5f;

        [FoldoutGroup("Shape Matrix")]
        [PropertyRange(0.01f, 2f)]
        public float shapeMatrixSphereScale = .5f;

        [FoldoutGroup("Shape Matrix")]
        [PropertyRange(1, 20)]
        public int shapeMatrixSteps = 5;

        [FoldoutGroup("Shape Matrix")]
        [PropertyRange(0.01f, 2f)]
        public float shapeMatrixDistanceSteps = .5f;

        [FoldoutGroup("Shape Matrix")]
        [PropertyRange(0f, 1f)]
        public float shapeMatrixAlpha = .5f;

        [FoldoutGroup("Shape Matrix")]
        [PropertyRange(0f, 1f)]
        public float shapeMatrixWorldAlpha = .25f;
            
        [FoldoutGroup("Shape Matrix")]
        public bool drawShapeMatrixNormals = true;
        [FoldoutGroup("Shape Matrix")]
        public bool drawShapeMatrixWeld = true;
        
        
        [FoldoutGroup("Shape Labels")]
        public bool drawShapeLabels = true;
        
        [FoldoutGroup("Vertex Data")]
        public bool drawVertexData = true;

        public enum DrawNormalStyle
        {
            All = 0,
            Caps = 1
        }
        
    }
#endif
}
