
using System.Collections.Generic;
using UnityEngine;
using static WorldSettings;
using static Library.Legacy.BlockTypesInfoGetter;

public class ChunkMeshGenerator
{
	private readonly Chunk _c;

	//Mesh\\
	public List<Vector3> MVert;//									MeshVertices
	public List<int> MTri;//										MeshTriangles
	public List<Vector3> MUVs;//									MeshUVs
	public List<Color32> MCol;//									MeshColors

	public ChunkMeshGenerator(Chunk chunk)
	{
		_c = chunk;
		MVert = new List<Vector3>();
		MTri = new List<int>();
		MUVs = new List<Vector3>();
		MCol = new List<Color32>();
	}

	public void Generate()
	{
		ClearChunkMesh();

		GenerateGlobalIllumination();

		GenerateChunkMeshSliceX(NEGATIVE_FACE);
		GenerateChunkMeshSliceX(POSITIVE_FACE);
		GenerateChunkMeshSliceY(NEGATIVE_FACE);
		GenerateChunkMeshSliceY(POSITIVE_FACE);
		GenerateChunkMeshSliceZ(NEGATIVE_FACE);
		GenerateChunkMeshSliceZ(POSITIVE_FACE);

		SetChunkMeshData(MVert.ToArray(), MTri.ToArray(), MUVs.ToArray(), MCol.ToArray());
	}

	private void ClearChunkMesh()
	{
		MVert.Clear();
		MTri.Clear();
		MUVs.Clear();
		MCol.Clear();
	}

	private void GenerateGlobalIllumination()
	{
		//1. I have to know which chunk is the highest for every X/Z combination.
		//2. Start from that chunk.
		//3. When a chunk is modified, check for light at bY[0], if there are any, need to update the chunk below. cycle.
		//4. Set a ray at 100% light power.
		//5. starting from top, go down and set every block to that value.
		//6. If a block the ray passes through has an absoption value, check it against the ray
		//lightMaxValue(1) - 0.2 -> 0.8 < currentLightValue(1) -> CurrentLightValue = 0.8
		//lightMaxValue(1) - 0.2 -> 0.8 == currentLightValue(0.8) -> CurrentLightValue.
		//lightMaxValue(1) - 0.5 -> 0.5 < currentLightValue(0.8) -> currentLightValue = 0.5
		//7. If currentLightValue = 0, stop, let it default to 0.05f.
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
					var blockType = _c.Blocks[c1DIndex];
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

	private void GenerateChunkMeshSliceY(bool isNegFace)
	{
		bool[] faceIsMerged;

		int preCalcY, preCalcYZ, c1DIndex;
		for (int bY = 0; bY < CHUNK_SIZE; bY++)
		{
			preCalcY = bY * CHUNK_SIZE;
			faceIsMerged = new bool[CHUNK_SIZE_SQUARED];
			for (int bZ = 0; bZ < CHUNK_SIZE; bZ++)
			{
				preCalcYZ = preCalcY + bZ;
				for (int bX = 0; bX < CHUNK_SIZE; bX++)
				{
					c1DIndex = preCalcYZ + bX * CHUNK_SIZE_SQUARED;
					var blockType = _c.Blocks[c1DIndex];
					if (BlockFaceIsMergeable(bX, bY, bZ, Y_AXIS, isNegFace, blockType, faceIsMerged[bZ * CHUNK_SIZE + bX]))
					{
						GetYSliceQuadSize(bX, bY, bZ, blockType, faceIsMerged, isNegFace,
							out int quadSizeWorkAxis1, out int quadSizeWorkAxis2);

						AddSliceQuadToChunkMesh(Y_AXIS, Z_AXIS, X_AXIS, quadSizeWorkAxis1, quadSizeWorkAxis2, bX, bY, bZ, isNegFace,
							GetTextureArrayLayerFaceFromBlockType(blockType, "down"),
							GetTextureArrayLayerFaceFromBlockType(blockType, "up"));

						MarkQuadAsMerged(quadSizeWorkAxis1, quadSizeWorkAxis2, bZ, bX, faceIsMerged);
					}
				}
			}
		}
	}

	private void GenerateChunkMeshSliceZ(bool isNegFace)
	{
		bool[] faceIsMerged;

		int preCalcXZ, c1DIndex;
		for (int bZ = 0; bZ < CHUNK_SIZE; bZ++)
		{
			faceIsMerged = new bool[CHUNK_SIZE_SQUARED];
			for (int bX = 0; bX < CHUNK_SIZE; bX++)
			{
				preCalcXZ = bZ + bX * CHUNK_SIZE_SQUARED;
				for (int bY = 0; bY < CHUNK_SIZE; bY++)
				{
					c1DIndex = preCalcXZ + bY * CHUNK_SIZE;
					var blockType = _c.Blocks[c1DIndex];
					if (BlockFaceIsMergeable(bX, bY, bZ, Z_AXIS, isNegFace, blockType, faceIsMerged[bX * CHUNK_SIZE + bY]))
					{
						GetZSliceQuadSize(bX, bY, bZ, blockType, faceIsMerged, isNegFace,
							out int quadSizeWorkAxis1, out int quadSizeWorkAxis2);

						AddSliceQuadToChunkMesh(Z_AXIS, X_AXIS, Y_AXIS, quadSizeWorkAxis1, quadSizeWorkAxis2, bX, bY, bZ, isNegFace,
							GetTextureArrayLayerFaceFromBlockType(blockType, "back"),
							GetTextureArrayLayerFaceFromBlockType(blockType, "front"));

						MarkQuadAsMerged(quadSizeWorkAxis1, quadSizeWorkAxis2, bX, bY, faceIsMerged);
					}
				}
			}
		}
	}

	private bool BlockFaceIsMergeable(int bX, int bY, int bZ, int workAxis, bool isNegFace, BlockTypes bType, bool faceIsMerged)
	{
		//Maybe there
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
		return false;
	}

	private bool NeighboringBlockIsOpaque(int x, int y, int z)
	{
		return GetBlockIsOpaqueBoolAtBlockIndex(x, y, z);
	}

	private bool GetBlockIsOpaqueBoolAtBlockIndex(int x, int y, int z)
	{
		if (BlockIndexIsOutsideOfCurrentChunk(x, y, z))
			return false;
		else
			return _c.BlockIsOpaque[GetBlock1DIndexFrom3DIndex(x, y, z)];
	}

	private static bool BlockIndexIsOutsideOfCurrentChunk(int x, int y, int z)
	{
		if (x < 0 || x >= CHUNK_SIZE ||
			y < 0 || y >= CHUNK_SIZE ||
			z < 0 || z >= CHUNK_SIZE)
			return true;
		return false;
	}

	private int GetBlock1DIndexFrom3DIndex(int x, int y, int z)
	{
		return x * CHUNK_SIZE_SQUARED + y * CHUNK_SIZE + z;
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

	private void GetYSliceQuadSize(int bX, int bY, int bZ, BlockTypes blockType, bool[] faceIsMerged, bool isNegFace,
		out int quadSizeWorkAxis1, out int quadSizeWorkAxis2)
	{
		int offX, offY, offZ;

		for (offX = bX, offY = bY, offZ = bZ, offX++;
			offX < CHUNK_SIZE && BlocksCanBeMerged(blockType, offX, offY, offZ, faceIsMerged[offZ * CHUNK_SIZE + offX], Y_AXIS, isNegFace);
			offX++)
		{ }

		quadSizeWorkAxis2 = offX - bX;

		for (offX = bX, offY = bY, offZ = bZ, offZ++;
			offZ < CHUNK_SIZE && BlocksCanBeMerged(blockType, offX, offY, offZ, faceIsMerged[offZ * CHUNK_SIZE + offX], Y_AXIS, isNegFace);
			offZ++)
		{
			for (offX = bX;
				offX < CHUNK_SIZE &&
				BlocksCanBeMerged(blockType, offX, offY, offZ, faceIsMerged[offZ * CHUNK_SIZE + offX], Y_AXIS, isNegFace) &&
				offX - bX < quadSizeWorkAxis2;
				offX++)
			{ }

			if (offX - bX < quadSizeWorkAxis2)
				break;
			else
				offX = bX;
		}
		quadSizeWorkAxis1 = offZ - bZ;
	}

	private void GetZSliceQuadSize(int bX, int bY, int bZ, BlockTypes blockType, bool[] faceIsMerged, bool isNegFace,
		out int quadSizeWorkAxis1, out int quadSizeWorkAxis2)
	{
		int offX, offY, offZ;

		for (offX = bX, offY = bY, offZ = bZ, offY++;
			offY < CHUNK_SIZE && BlocksCanBeMerged(blockType, offX, offY, offZ, faceIsMerged[offX * CHUNK_SIZE + offY], Z_AXIS, isNegFace);
			offY++)
		{ }

		quadSizeWorkAxis2 = offY - bY;

		for (offX = bX, offY = bY, offZ = bZ, offX++;
			offX < CHUNK_SIZE && BlocksCanBeMerged(blockType, offX, offY, offZ, faceIsMerged[offX * CHUNK_SIZE + offY], Z_AXIS, isNegFace);
			offX++)
		{
			for (offY = bY;
				offY < CHUNK_SIZE &&
				BlocksCanBeMerged(blockType, offX, offY, offZ, faceIsMerged[offX * CHUNK_SIZE + offY], Z_AXIS, isNegFace) &&
				offY - bY < quadSizeWorkAxis2;
				offY++)
			{ }

			if (offY - bY < quadSizeWorkAxis2)
				break;
			else
				offY = bY;
		}
		quadSizeWorkAxis1 = offX - bX;
	}

	private bool BlocksCanBeMerged(BlockTypes blockType, int offX, int offY, int offZ, bool offBlockFaceIsMerged, int workAxis0, bool isNegFace)
	{
		var offBlockType = _c.Blocks[GetBlock1DIndexFrom3DIndex(offX, offY, offZ)];
		if (blockType == offBlockType &&
			BlockFaceIsMergeable(offX, offY, offZ, workAxis0, isNegFace, offBlockType, offBlockFaceIsMerged))
			return true;
		return false;
	}

	private void AddSliceQuadToChunkMesh(int workAxis0, int workAxis1, int workAxis2, int quadSizeWorkAxis1, int quadSizeWorkAxis2,
		int posX, int posY, int posZ, bool isNegFace, TexArrLayer textureArrayLayerIDNegFace, TexArrLayer textureArrayLayerIDPosFace)
	{
		AddSliceQuadVerticesToChunkMesh(workAxis0, workAxis1, workAxis2, quadSizeWorkAxis1, quadSizeWorkAxis2, posX, posY, posZ, isNegFace);
		AddQuadTrianglesToChunkMesh(isNegFace);
		AddQuadUvsToChunkMesh(workAxis0, quadSizeWorkAxis1, quadSizeWorkAxis2, textureArrayLayerIDNegFace, textureArrayLayerIDPosFace, isNegFace);
		AddQuadColorsToChunkMesh();
	}

	private void AddSliceQuadVerticesToChunkMesh(int sliceAxis, int workAxis1, int workAxis2, int quadSizeWorkAxis, int quadSizeWorkAxis2, int posX, int posY, int posZ, bool isNegFace)
	{
		var workAxis1Offset = new Vector3Int { [workAxis1] = quadSizeWorkAxis };
		var workAxis2Offset = new Vector3Int { [workAxis2] = quadSizeWorkAxis2 };
		var frontOffsetPos = new Vector3Int((int)posX, (int)posY, (int)posZ);
		frontOffsetPos[sliceAxis] += isNegFace ? 0 : 1;

		MVert.Add(frontOffsetPos);
		MVert.Add(frontOffsetPos + workAxis1Offset);
		MVert.Add(frontOffsetPos + workAxis1Offset + workAxis2Offset);
		MVert.Add(frontOffsetPos + workAxis2Offset);
	}

	private void AddQuadTrianglesToChunkMesh(bool isNegFace)
	{
		if (!isNegFace)
		{
			MTri.Add(MVert.Count - 4);
			MTri.Add(MVert.Count - 3);
			MTri.Add(MVert.Count - 2);

			MTri.Add(MVert.Count - 4);
			MTri.Add(MVert.Count - 2);
			MTri.Add(MVert.Count - 1);
		}
		else
		{
			MTri.Add(MVert.Count - 2);
			MTri.Add(MVert.Count - 3);
			MTri.Add(MVert.Count - 4);

			MTri.Add(MVert.Count - 1);
			MTri.Add(MVert.Count - 2);
			MTri.Add(MVert.Count - 4);
		}
	}

	private void AddQuadUvsToChunkMesh(int workAxis0, int quadWorkAxis1, int quadWorkAxis2, TexArrLayer textureArrayLayerIDNegFace, TexArrLayer textureArrayLayerIDPosFace, bool isNegFace)
	{
		var textureArrayLayerIDFace = (int)(isNegFace ? textureArrayLayerIDNegFace : textureArrayLayerIDPosFace);
		switch (workAxis0)
		{
			case X_AXIS:
				{
					MUVs.Add(new Vector3(0, 0, textureArrayLayerIDFace));
					MUVs.Add(new Vector3(0, quadWorkAxis1, textureArrayLayerIDFace));
					MUVs.Add(new Vector3(quadWorkAxis2, quadWorkAxis1, textureArrayLayerIDFace));
					MUVs.Add(new Vector3(quadWorkAxis2, 0, textureArrayLayerIDFace));
					break;
				}
			case Y_AXIS:
				{
					MUVs.Add(new Vector3(0, 0, textureArrayLayerIDFace));
					MUVs.Add(new Vector3(0, quadWorkAxis1, textureArrayLayerIDFace));
					MUVs.Add(new Vector3(quadWorkAxis2, quadWorkAxis1, textureArrayLayerIDFace));
					MUVs.Add(new Vector3(quadWorkAxis2, 0, textureArrayLayerIDFace));
					break;
				}
			case Z_AXIS:
				{
					MUVs.Add(new Vector3(quadWorkAxis1, 0, textureArrayLayerIDFace));
					MUVs.Add(new Vector3(0, 0, textureArrayLayerIDFace));
					MUVs.Add(new Vector3(0, quadWorkAxis2, textureArrayLayerIDFace));
					MUVs.Add(new Vector3(quadWorkAxis1, quadWorkAxis2, textureArrayLayerIDFace));
					break;
				}
		}
	}

	//redo with lighting system
	private void AddQuadColorsToChunkMesh()
	{
		MCol.Add(new Color32(0, 0, 0, 100));
		MCol.Add(new Color32(0, 0, 0, 100));
		MCol.Add(new Color32(0, 0, 0, 100));
		MCol.Add(new Color32(0, 0, 0, 100));
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

	private void SetChunkMeshData(Vector3[] meshVertices, int[] meshTriangles, Vector3[] meshUVs, Color32[] meshColors)
	{
		//Mesh Filter & Mesh
		Mesh mesh;
		(mesh = _c.MeshFilter.mesh).Clear();
		if (meshVertices.Length > MESH_VERTICES_QUANTITY_LIMIT)
			mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = meshVertices;
		mesh.triangles = meshTriangles;
		mesh.SetUVs(0, meshUVs);
		mesh.colors32 = meshColors;
		mesh.RecalculateNormals();
		//Mesh Renderer
		_c.MeshRenderer.material = World.I.WorldMaterial;
		//Mesh Collider
		_c.MeshCollider.sharedMesh = mesh;
		_c.MeshCollider.material = World.I.ChunkPhysicMaterial;
	}
}
