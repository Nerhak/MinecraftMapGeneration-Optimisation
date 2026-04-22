#if false
using UnityEngine;
using Unity.Collections;
using static Library.Legacy.Legacy;
using static Library.Legacy.IndexConverter;
using static WorldSettings;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;

public struct GenerateChunkMeshSliceXJob : IJob
{
	[ReadOnly]
	public NativeArray<BlockTypes> Blocks;
	[ReadOnly]
	public NativeArray<bool> BlockIsOpaque;
	[ReadOnly]
	public bool IsNegFace;

	public NativeList<Vector3> MeshSliceVertices;
	public NativeList<int> MeshSliceTriangles;
	public NativeList<Vector3> MeshSliceUVs;
	public NativeList<Color32> MeshSliceColors;

	public void Execute()
	{
		GenerateChunkMeshSliceX(IsNegFace);
	}

	private void GenerateChunkMeshSliceX(bool isNegFace)
	{
		bool[] faceIsMerged;

		int c1DIndex = 0;
		for (int bX = 0; bX < CHUNK_SIZE; bX++)
		{
			faceIsMerged = new bool[CHUNK_SIZE_SQUARED];
			for (int bY = 0; bY < CHUNK_SIZE; bY++)
			{
				for (int bZ = 0; bZ < CHUNK_SIZE; bZ++)
				{
					var blockType = Blocks[c1DIndex];
					if (BlockFaceIsMergeable(bX, bY, bZ, X_AXIS, isNegFace, blockType, faceIsMerged[bY * CHUNK_SIZE + bZ]))
					{
						GetXSliceQuadSize(bX, bY, bZ, blockType, faceIsMerged, isNegFace,
							out int quadSizeWorkAxis1, out int quadSizeWorkAxis2);

						AddSliceQuadToChunkMesh(X_AXIS, Y_AXIS, Z_AXIS, quadSizeWorkAxis1, quadSizeWorkAxis2, bX, bY, bZ, isNegFace,
							GetTextureArrayLayerFaceFromBlockType(blockType, "left"),
							GetTextureArrayLayerFaceFromBlockType(blockType, "right"));

						MarkQuadAsMerged(quadSizeWorkAxis1, quadSizeWorkAxis2, bY, bZ, faceIsMerged);
					}
					c1DIndex++;
				}
			}
		}
	}

	private bool BlockFaceIsMergeable(int bX, int bY, int bZ, int workAxis, bool isNegFace, BlockTypes bType, bool faceIsMerged)
	{
		if (bType != BlockTypes.Air && !faceIsMerged)
		{
			switch (workAxis)
			{
				case X_AXIS:
					{
						if (isNegFace)
							return !NeighboringBlockIsOpaque(bX - 1, bY, bZ);
						else
							return !NeighboringBlockIsOpaque(bX + 1, bY, bZ);
					}
				case Y_AXIS:
					{
						if (isNegFace)
							return !NeighboringBlockIsOpaque(bX, bY - 1, bZ);
						else
							return !NeighboringBlockIsOpaque(bX, bY + 1, bZ);
					}
				case Z_AXIS:
					{
						if (isNegFace)
							return !NeighboringBlockIsOpaque(bX, bY, bZ - 1);
						else
							return !NeighboringBlockIsOpaque(bX, bY, bZ + 1);
					}
			}
		}
		return (false);
	}

	private bool NeighboringBlockIsOpaque(int x, int y, int z)
	{
		return (GetBlockIsOpaqueBoolAtBlockIndex(x, y, z));
	}

	private bool GetBlockIsOpaqueBoolAtBlockIndex(int x, int y, int z)
	{
		if (BlockIndexIsOutsideOfCurrentChunk(x, y, z))
			return (false);
		else
			return BlockIsOpaque[ConvertBlock3DIndexTo1DMultiThread(x, y, z, CHUNK_SIZE, CHUNK_SIZE_SQUARED)];
	}

	private bool BlockIndexIsOutsideOfCurrentChunk(int x, int y, int z)
	{
		if (x < 0 || x >= CHUNK_SIZE ||
			y < 0 || y >= CHUNK_SIZE ||
			z < 0 || z >= CHUNK_SIZE)
			return (true);
		return (false);
	}

	private void GetXSliceQuadSize(int bX, int bY, int bZ, BlockTypes blockType, bool[] faceIsMerged, bool isNegFace,
		out int quadSizeWorkAxis1, out int quadSizeWorkAxis2)
	{
		int offX, offY, offZ;

		for (offX = bX, offY = bY, offZ = bZ, offZ++;
			offZ < CHUNK_SIZE && BlocksCanBeMerged(blockType, offX, offY, offZ, faceIsMerged[offY * CHUNK_SIZE + offZ], X_AXIS, isNegFace);
			offZ++)
		{ }

		quadSizeWorkAxis2 = offZ - bZ;

		for (offX = bX, offY = bY, offZ = bZ, offY++;
			offY < CHUNK_SIZE && BlocksCanBeMerged(blockType, offX, offY, offZ, faceIsMerged[offY * CHUNK_SIZE + offZ], X_AXIS, isNegFace);
			offY++)
		{
			for (offZ = bZ;
				offZ < CHUNK_SIZE &&
				BlocksCanBeMerged(blockType, offX, offY, offZ, faceIsMerged[offY * CHUNK_SIZE + offZ], X_AXIS, isNegFace) &&
				offZ - bZ < quadSizeWorkAxis2;
				offZ++)
			{ }

			if (offZ - bZ < quadSizeWorkAxis2)
				break;
			else
				offZ = bZ;
		}
		quadSizeWorkAxis1 = offY - bY;
	}

	private bool BlocksCanBeMerged(BlockTypes blockType, int offX, int offY, int offZ, bool offBlockFaceIsMerged, int workAxis0, bool isNegFace)
	{
		var offBlockType = Blocks[ConvertBlock3DIndexTo1DMultiThread(offX, offY, offZ, CHUNK_SIZE, CHUNK_SIZE_SQUARED)];
		if (blockType == offBlockType &&
			BlockFaceIsMergeable(offX, offY, offZ, workAxis0, isNegFace, offBlockType, offBlockFaceIsMerged))
			return (true);
		return (false);
	}

	private void AddSliceQuadToChunkMesh(int workAxis0, int workAxis1, int workAxis2, int quadSizeWorkAxis1, int quadSizeWorkAxis2,
	int posX, int posY, int posZ, bool isNegFace, int textureArrayLayerIDNegFace, int textureArrayLayerIDPosFace)
	{
		AddSliceQuadVerticesToChunkMesh(workAxis0, workAxis1, workAxis2, quadSizeWorkAxis1, quadSizeWorkAxis2, posX, posY, posZ, isNegFace);
		AddQuadTrianglesToChunkMesh(isNegFace);
		AddQuadUvsToChunkMesh(workAxis0, quadSizeWorkAxis1, quadSizeWorkAxis2, textureArrayLayerIDNegFace, textureArrayLayerIDPosFace, isNegFace);
		AddQuadColorsToChunkMesh();
	}

	private void MarkQuadAsMerged(int quadSizeWorkAxis1, int quadSizeWorkAxis2, int posWorkAxis1, int posWorkAxis2, bool[] faceIsMerged)
	{
		int preCalcMergOffX;
		for (int x = 0; x < quadSizeWorkAxis1; x++)
		{
			preCalcMergOffX = (posWorkAxis1 + x) * CHUNK_SIZE;
			for (int y = 0; y < quadSizeWorkAxis2; y++)
			{
				faceIsMerged[preCalcMergOffX + posWorkAxis2 + y] = true;
			}
		}
	}

	private void AddSliceQuadVerticesToChunkMesh(int sliceAxis, int workAxis1, int workAxis2, int quadSizeWorkAxis, int quadSizeWorkAxis2, int posX, int posY, int posZ, bool isNegFace)
	{
		var workAxis1Offset = new Vector3Int { [workAxis1] = quadSizeWorkAxis };
		var workAxis2Offset = new Vector3Int { [workAxis2] = quadSizeWorkAxis2 };
		var frontOffsetPos = new Vector3Int((int)posX, (int)posY, (int)posZ);
		frontOffsetPos[sliceAxis] += isNegFace ? 0 : 1;

		MeshSliceVertices.Add(frontOffsetPos);
		MeshSliceVertices.Add(frontOffsetPos + workAxis1Offset);
		MeshSliceVertices.Add(frontOffsetPos + workAxis1Offset + workAxis2Offset);
		MeshSliceVertices.Add(frontOffsetPos + workAxis2Offset);
	}

	private void AddQuadTrianglesToChunkMesh(bool isNegFace)
	{
		if (!isNegFace)
		{
			MeshSliceTriangles.Add(MeshSliceVertices.Length - 4); //If there is a problem, look the lenght, which was count before
			MeshSliceTriangles.Add(MeshSliceVertices.Length - 3);
			MeshSliceTriangles.Add(MeshSliceVertices.Length - 2);

			MeshSliceTriangles.Add(MeshSliceVertices.Length - 4);
			MeshSliceTriangles.Add(MeshSliceVertices.Length - 2);
			MeshSliceTriangles.Add(MeshSliceVertices.Length - 1);
		}
		else
		{
			MeshSliceTriangles.Add(MeshSliceVertices.Length - 2);
			MeshSliceTriangles.Add(MeshSliceVertices.Length - 3);
			MeshSliceTriangles.Add(MeshSliceVertices.Length - 4);

			MeshSliceTriangles.Add(MeshSliceVertices.Length - 1);
			MeshSliceTriangles.Add(MeshSliceVertices.Length - 2);
			MeshSliceTriangles.Add(MeshSliceVertices.Length - 4);
		}
	}

	private void AddQuadUvsToChunkMesh(int workAxis0, int quadWorkAxis1, int quadWorkAxis2, int textureArrayLayerIDNegFace, int textureArrayLayerIDPosFace, bool isNegFace)
	{
		var textureArrayLayerIDFace = isNegFace ? textureArrayLayerIDNegFace : textureArrayLayerIDPosFace;
		switch (workAxis0)
		{
			case X_AXIS:
				{
					MeshSliceUVs.Add(new Vector3(0, 0, textureArrayLayerIDFace));
					MeshSliceUVs.Add(new Vector3(0, quadWorkAxis1, textureArrayLayerIDFace));
					MeshSliceUVs.Add(new Vector3(quadWorkAxis2, quadWorkAxis1, textureArrayLayerIDFace));
					MeshSliceUVs.Add(new Vector3(quadWorkAxis2, 0, textureArrayLayerIDFace));
					break;
				}
			case Y_AXIS:
				{
					MeshSliceUVs.Add(new Vector3(0, 0, textureArrayLayerIDFace));
					MeshSliceUVs.Add(new Vector3(0, quadWorkAxis1, textureArrayLayerIDFace));
					MeshSliceUVs.Add(new Vector3(quadWorkAxis2, quadWorkAxis1, textureArrayLayerIDFace));
					MeshSliceUVs.Add(new Vector3(quadWorkAxis2, 0, textureArrayLayerIDFace));
					break;
				}
			case Z_AXIS:
				{
					MeshSliceUVs.Add(new Vector3(quadWorkAxis1, 0, textureArrayLayerIDFace));
					MeshSliceUVs.Add(new Vector3(0, 0, textureArrayLayerIDFace));
					MeshSliceUVs.Add(new Vector3(0, quadWorkAxis2, textureArrayLayerIDFace));
					MeshSliceUVs.Add(new Vector3(quadWorkAxis1, quadWorkAxis2, textureArrayLayerIDFace));
					break;
				}
		}
	}

	private void AddQuadColorsToChunkMesh()
	{
		MeshSliceColors.Add(new Color32(0, 0, 0, 255));
		MeshSliceColors.Add(new Color32(0, 0, 0, 255));
		MeshSliceColors.Add(new Color32(0, 0, 0, 255));
		MeshSliceColors.Add(new Color32(0, 0, 0, 255));
	}

	public static bool GetBlockIsOpaqueBoolFromBlockType(BlockTypes blocktype)
	{
		switch (blocktype)
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
			default:
				{
					Debug.Log($"GetBlockIsOpaqueBoolFromBlockType:\nblockType[{blocktype}] NOT IMPLEMENTED");
					return false;
				}
		}
	}
}
#endif