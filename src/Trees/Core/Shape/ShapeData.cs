using System;
using System.Collections.Generic;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Core.Seeds;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Core.Shape
{
    [Serializable]
    public abstract class ShapeData
    {
        //public float angle;
        [FormerlySerializedAs("baseAngle")] public float radialAngle;
        public float distributionLikelihood;
        public float distributionScale;

        [FormerlySerializedAs("_visible")] [SerializeField] public bool visible;
        [SerializeField] private bool _hidden;
        [FormerlySerializedAs("_forcedInvisible")] [SerializeField] public bool forcedInvisible;

        public bool hidden
        {
            //get => _hidden;
            set => _hidden = value;
        }

        public bool exportGeometry
        {
            get
            {
                if (!visible)
                {
                    return false;
                }

                if (_hidden)
                {
                    return false;
                }

                if (forcedInvisible)
                {
                    return false;
                }

                return true;
            }
        }

        public List<ShapeGeometryData> geometry;

        public Matrix4x4 matrix;
        public Matrix4x4 effectiveMatrix;

        public bool welded;
        
        public float offset;
        [FormerlySerializedAs("pitch")] public float verticalAngle;
        /*[FormerlySerializedAs("rotation")] public Quaternion localRotation;*/
        
        public float scale;
        public float size;
        public float length;
        public float effectiveScale;
        public float effectiveSize;
        public float effectiveLength;
        public StageType stageType;

        //public InternalSeed seed;
        public float variationSeed;
        public int shapeID = -1;
        public int parentShapeID = -1;
        public int individualID;
        public int hierarchyID;

        public abstract TreeComponentType type { get; }
        
        public bool IsBranch => type == TreeComponentType.Branch;
        public bool IsFruit => type == TreeComponentType.Fruit;
        public bool IsKnot => type == TreeComponentType.Knot;
        public bool IsLeaf => type == TreeComponentType.Leaf;
        public bool IsRoot => type == TreeComponentType.Root;
        public bool IsTrunk => type == TreeComponentType.Trunk;
        public bool IsFungus => type == TreeComponentType.Fungus;
        public bool IsSpline => IsBranch || IsRoot || IsTrunk;

        
    
        public ShapeData Clone()
        {
            var shapeData = GetNew();

            shapeData.shapeID = shapeID;
            shapeData.individualID = individualID;
            shapeData.hierarchyID = hierarchyID;
            shapeData.parentShapeID = parentShapeID;
            //shapeData.seed = seed.Clone();
            shapeData.visible = visible;
            shapeData._hidden = _hidden;
            shapeData.forcedInvisible = forcedInvisible;
            shapeData.geometry = new List<ShapeGeometryData>();
            foreach (var geo in geometry)
            {
                shapeData.geometry.Add(geo.Clone());
            }
            shapeData.size = size;
            shapeData.scale = scale;
            shapeData.length = length;
            shapeData.distributionScale = distributionScale;
            shapeData.distributionLikelihood = distributionLikelihood;
            shapeData.effectiveScale = effectiveScale;
            shapeData.effectiveSize = effectiveSize;
            shapeData.effectiveLength = effectiveLength;
            shapeData.offset = offset;
            shapeData.radialAngle = radialAngle;
            //shapeData.angle = angle;
            shapeData.verticalAngle = verticalAngle;
            shapeData.matrix = matrix;
            shapeData.effectiveMatrix = effectiveMatrix;
            shapeData.stageType = stageType;

            Clone(shapeData);

            return shapeData;
        }

        protected abstract ShapeData GetNew();

        protected abstract void Clone(ShapeData shapeData);

        protected ShapeData(int shapeID, int hierarchyID, int parentShapeID)
        {
            this.shapeID = shapeID;
            this.hierarchyID = hierarchyID;
            this.parentShapeID = parentShapeID;
            
            geometry = new List<ShapeGeometryData>();

            SetUp(null);
        }

        public void SetUp(ISeed seed)
        {
            //angle = 0f;
            radialAngle = 0f;
            distributionLikelihood = 1f;
            distributionScale = 1f;
            visible = true;
            _hidden = false;
            forcedInvisible = false;
            if (geometry == null)
            {
                geometry = new List<ShapeGeometryData>();
            }
            geometry.Clear();
            matrix = Matrix4x4.identity;
            effectiveMatrix = Matrix4x4.identity;
            offset = 0f;
            verticalAngle = 0f;
            scale = 1f;
            size = 1f;
            length = 1f;
            effectiveScale = 1f;
            effectiveSize = 1f;
            effectiveLength = 1f;
            variationSeed = seed?.RandomValue() ?? 0f;
            stageType = StageType.Normal;
            
            SetUpInternal();
        }

        protected virtual void SetUpInternal()
        {
            
        }
    }
}