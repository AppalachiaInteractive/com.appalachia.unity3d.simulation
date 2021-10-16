using System;
using Appalachia.Simulation.Trees.Generation.Distribution;
using Appalachia.Simulation.Trees.Generation.Geometry.Leaves;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using TreeEditor;

namespace Appalachia.Simulation.Trees.Extensions
{
	public static class TreeEditorExtensions
	{
		public static DistributionVerticalMode ToVertical(this TreeGroup.DistributionMode mode)
		{
			switch (mode)
			{
				case TreeGroup.DistributionMode.Random:
					return DistributionVerticalMode.Random;
				case TreeGroup.DistributionMode.Alternate:
					return DistributionVerticalMode.Equal;
				case TreeGroup.DistributionMode.Opposite:
					return DistributionVerticalMode.Equal;
				case TreeGroup.DistributionMode.Whorled:
					return DistributionVerticalMode.Equal;
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}

		public static DistributionRadialMode ToRadial(this TreeGroup.DistributionMode mode)
		{
			switch (mode)
			{
				case TreeGroup.DistributionMode.Random:
					return DistributionRadialMode.Random;
				case TreeGroup.DistributionMode.Alternate:
					return DistributionRadialMode.Equal;
				case TreeGroup.DistributionMode.Opposite:
					return DistributionRadialMode.Equal;
				case TreeGroup.DistributionMode.Whorled:
					return DistributionRadialMode.Equal;
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}
		
		
		public static int ToClusterCount(this TreeGroup.DistributionMode mode, int nodes)
		{
			switch (mode)
			{
				case TreeGroup.DistributionMode.Random:
					return 1;
				case TreeGroup.DistributionMode.Alternate:
					return 2;
				case TreeGroup.DistributionMode.Opposite:
					return 2;
				case TreeGroup.DistributionMode.Whorled:
					return nodes;
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}
		
		public static int ToStepOffset(this TreeGroup.DistributionMode mode)
		{
			switch (mode)
			{
				case TreeGroup.DistributionMode.Random:
					return 0;
				case TreeGroup.DistributionMode.Alternate:
					return 0;
				case TreeGroup.DistributionMode.Opposite:
					return 90;
				case TreeGroup.DistributionMode.Whorled:
					return 0;
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}


		
		/*
		public static LockSettings ToSettings(this TreeGroup.LockFlag lockFlags)
		{
			var settings = new LockSettings()
			{
				lockAlignment = ((lockFlags & TreeGroup.LockFlag.LockAlignment) != 0),
				lockPosition = ((lockFlags & TreeGroup.LockFlag.LockPosition) != 0),
				lockShape = ((lockFlags & TreeGroup.LockFlag.LockShape) != 0),
			};

			return settings;
		}
		*/

		public static LeafGeometryMode ToInternal(this TreeGroupLeaf.GeometryMode mode)
		{
			switch (mode)
			{
				case TreeGroupLeaf.GeometryMode.PLANE:
					return LeafGeometryMode.DiamondLengthCut;
				
				case TreeGroupLeaf.GeometryMode.CROSS:
					return LeafGeometryMode.DiamondPyramid;
				
				case TreeGroupLeaf.GeometryMode.TRI_CROSS:
					return LeafGeometryMode.DiamondPyramid;
				
				case TreeGroupLeaf.GeometryMode.BILLBOARD:
					return LeafGeometryMode.Billboard;
				
				case TreeGroupLeaf.GeometryMode.MESH:
					return LeafGeometryMode.Mesh;
				
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}

		public static BranchGeometryMode ToInternal(this TreeGroupBranch.GeometryMode mode)
		{
			switch (mode)
			{
				case TreeGroupBranch.GeometryMode.Branch:
					return BranchGeometryMode.Branch;
				
				case TreeGroupBranch.GeometryMode.BranchFrond:
					return BranchGeometryMode.BranchFrond;
				
				case TreeGroupBranch.GeometryMode.Frond:
					return BranchGeometryMode.Frond;
				
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}
	}

	
	
}