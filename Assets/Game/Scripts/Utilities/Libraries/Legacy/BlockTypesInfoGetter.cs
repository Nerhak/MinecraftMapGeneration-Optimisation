using UnityEditor.PackageManager;
using UnityEngine;

namespace Library.Legacy
{
	public static class BlockTypesInfoGetter
	{
		public static bool GetBlockIsOpaqueBoolFromBlockType(BlockTypes blockType)
		{
			switch (blockType)
			{
				case BlockTypes.Air:
					return false;
				case BlockTypes.Bedrock:
					return true;
				case BlockTypes.Dirt:
					return true;
				case BlockTypes.DirtWithGrass:
					return true;
				case BlockTypes.Stone:
					return true;
				case BlockTypes.TreeTrunk:
					return true;
				case BlockTypes.TreeLeaf:
					return false;
				case BlockTypes.Glass:
					return false;
				default:
					{
						Debug.Log($"GetBlockIsOpaqueBoolFromBlockType:\nblockType[{blockType}] NOT IMPLEMENTED");
						return false;
					}
			}
		}

		public static float GetBlocksLightAbsorptionFromBlockType(BlockTypes blockType)
		{
			switch (blockType)
			{
				case BlockTypes.Air:
					return 0f;
				case BlockTypes.Bedrock:
					return 1f;
				case BlockTypes.Dirt:
					return 1f;
				case BlockTypes.DirtWithGrass:
					return 1f;
				case BlockTypes.Stone:
					return 1f;
				case BlockTypes.TreeTrunk:
					return 1f;
				case BlockTypes.TreeLeaf:
					return 0.2f;
				case BlockTypes.Glass:
					return 0f;
				default:
					{
						Debug.Log($"GetBlocksLightAbsorptionFromBlockType:\nblockType[{blockType}] NOT IMPLEMENTED");
						return 0f;
					}
			}
		}

		public static sbyte GetBlocksHPFromBlockType(BlockTypes blockType)
		{
			switch (blockType)
			{
				case BlockTypes.Air:
					return -1;
				case BlockTypes.Bedrock:
					return -1;
				case BlockTypes.Dirt:
					return 4;
				case BlockTypes.DirtWithGrass:
					return 4;
				case BlockTypes.Stone:
					return 6;
				case BlockTypes.TreeTrunk:
					return 6;
				case BlockTypes.TreeLeaf:
					return 1;
				case BlockTypes.Glass:
					return 3;
				default:
					{
						Debug.Log($"GetBlocksHPFromBlockType:\nblockType[{blockType}] NOT IMPLEMENTED");
						return -1;
					}
			}
		}

		public static TexArrLayer GetTextureArrayLayerFaceFromBlockType(BlockTypes blockType, string face)
		{
			switch (blockType)
			{
				case BlockTypes.Bedrock:
					return TexArrLayer.Bedrock;
				case BlockTypes.Dirt:
					return TexArrLayer.Dirt;
				case BlockTypes.DirtWithGrass:
					{
						switch (face)
						{
							case "left":
								return TexArrLayer.DirtWithGrassSide;
							case "right":
								return TexArrLayer.DirtWithGrassSide;
							case "down":
								return TexArrLayer.Dirt;
							case "up":
								return TexArrLayer.DirtWithGrassTop;
							case "back":
								return TexArrLayer.DirtWithGrassSide;
							case "front":
								return TexArrLayer.DirtWithGrassSide;
							default:
								return 0;
						}
					}
				case BlockTypes.Stone:
					return TexArrLayer.Stone;
				case BlockTypes.TreeTrunk:
					{
						switch (face)
						{
							case "left":
								return TexArrLayer.TreeTrunkSide;
							case "right":
								return TexArrLayer.TreeTrunkSide;
							case "down":
								return TexArrLayer.TreeTrunkBottomTop;
							case "up":
								return TexArrLayer.TreeTrunkBottomTop;
							case "back":
								return TexArrLayer.TreeTrunkSide;
							case "front":
								return TexArrLayer.TreeTrunkSide;
							default:
								return 0;
						}
					}
				case BlockTypes.TreeLeaf:
					return TexArrLayer.TreeLeaf;
				case BlockTypes.Glass:
					return TexArrLayer.Glass;
				default:
					{
						Debug.Log($"GetTextureArrayLayerFaceFromBlockType:\nblocktype[{blockType}] NOT IMPLEMENTED");
						return 0;
					}
			}
		}
	}
}