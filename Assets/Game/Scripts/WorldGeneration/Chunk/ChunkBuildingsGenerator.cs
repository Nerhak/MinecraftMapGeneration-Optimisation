using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldSettings;
using static ChunkGenerationMethods;
using static Library.Legacy.IndexConverter;
using static Library.Legacy.PerlinNoiseMultiThread;
using static Library.Legacy.BlockTypesInfoGetter;
using Unity.Collections;

public class ChunkBuildingsGenerator
{
	private Chunk _c;

	public ChunkBuildingsGenerator(Chunk c)
	{
		_c = c;
	}

	public void Generate()
	{
		NativeArray<int> bWSHAWPXZ = PopulateChunkBiomeWorldSurfaceHeightAtWorldPositionArray(_c);

		for (int bX = 0; bX < CHUNK_SIZE; bX++)
		{
			for (int bY = 0; bY < CHUNK_SIZE; bY++)
			{
				for (int bZ = 0; bZ < CHUNK_SIZE; bZ++)
				{
					GenerateTerrainData(bX, bY, bZ, bWSHAWPXZ[ConvertBlock2DIndexTo1D(bX, bZ)]);
				}
			}
		}

		bWSHAWPXZ.Dispose();
	}

	private void GenerateTerrainData(int bX, int bY, int bZ, int bWSHAWPXZ)
	{
		ConvertBlock3DIndexToWorldPosition(bX, bY, bZ, _c.CWPZ, _c.CWPY, _c.CWPZ,
			out int bWPX, out int bWPY, out int bWPZ);

		switch (bWPY)
		{
			case var _ when (bWPY > bWSHAWPXZ):
				{
					GenerateAboveSurfaceBuildings(bX, bY, bZ, bWPX, bWPY, bWPZ, bWSHAWPXZ);
					break;
				}
		}
	}

	private void GenerateAboveSurfaceBuildings(int bX, int bY, int bZ, int bWPX, int bWPY, int bWPZ, int bWSHAWPXZ)
	{
		switch (bWPY)
		{
			case var _ when (bWPY == bWSHAWPXZ + 1): //Touching ground
				{
					if (isTreeTrunk(bWPX, bWPZ))
					{
						//var execTime = Time.realtimeSinceStartup;
						GenerateTree(bX, bY, bZ, World.I.BldDatLst.Tree.XNegSize, World.I.BldDatLst.Tree.XPosSize,
							World.I.BldDatLst.Tree.YNegSize, World.I.BldDatLst.Tree.YPosSize, World.I.BldDatLst.Tree.ZNegSize,
							World.I.BldDatLst.Tree.ZPosSize, World.I.BldDatLst.Tree.BlocksPosition, World.I.BldDatLst.Tree.BlocksTypes);
						//Debug.Log($"GenerateTree:{Time.realtimeSinceStartup - execTime}");
					}
					break;
				}
		}
	}

	private bool isTreeTrunk(int bWPX, int bWPZ)
	{
		return (MapToNewRange(0, 1, FractalBrownianMotion2D(bWPX, bWPZ, 0.99f, 1, 1, 1)) < 0.1f); //To store somewhere for ref
	}

	private void GenerateTree(int bX, int bY, int bZ, int xNegSize, int xPosSize, int yNegSize, int yPosSize, int zNegSize,
		int zPosSize, List<Vector3Int> blocksPositions, List<BlockTypes> blocksTypes)
	{
		Chunk[] storedChunks = StoreNeighborChunks(bX, bY, bZ, xNegSize, xPosSize, yNegSize, yPosSize, zNegSize, zPosSize);
		Chunk currC;

		List<BuildingModification> buildMod = new List<BuildingModification>();

		int i;
		int blocksPositionsCount = blocksPositions.Count;
		for (i = 0; i < blocksPositionsCount; i++)
		{
			var currBX = bX + blocksPositions[i].x;
			var currBY = bY + blocksPositions[i].y;
			var currBZ = bZ + blocksPositions[i].z;
			currC = GetCurrentChunk(currBX, currBY, currBZ, storedChunks);
			BoundBlockPositionsToChunkSize(ref currBX, ref currBY, ref currBZ);
			var b1DIndex = ConvertBlock3DIndexTo1D(currBX, currBY, currBZ);
			if (currC.Blocks[b1DIndex] == BlockTypes.Air)
				buildMod.Add(new BuildingModification(b1DIndex, blocksTypes[i], currC));
			else
				break;
		}

		if (i == blocksPositionsCount)
		{
			var buildingModificationsCount = buildMod.Count;
			for (int j = 0; j < buildingModificationsCount; j++)
			{
				//Might be optimized if we store the infos, to test
				var blockType = buildMod[j].BlockType;
				buildMod[j].C.Blocks[buildMod[j].B1DIndex] = blockType;
				buildMod[j].C.BlockIsOpaque[buildMod[j].B1DIndex] = GetBlockIsOpaqueBoolFromBlockType(blockType);
				buildMod[j].C.BlocksHP[buildMod[j].B1DIndex] = GetBlocksHPFromBlockType(blockType);
			}
		}
	}

	private Chunk[] StoreNeighborChunks(int bX, int bY, int bZ, int xDimNeg, int xDimPos, int yDimNeg, int yDimPos,
		int zDimNeg, int zDimPos)
	{
		Chunk[] storedChunks = new Chunk[27];

		bool overflowsLeft = bX + xDimNeg < 0;
		bool overflowsRight = bX + xDimPos >= CHUNK_SIZE;
		bool overflowsDown = bY + yDimNeg < 0;
		bool overflowsUp = bY + yDimPos >= CHUNK_SIZE;
		bool overflowsBack = bZ + zDimNeg < 0;
		bool overflowsFront = bZ + zDimPos >= CHUNK_SIZE;

		if (overflowsLeft)
		{
			if (overflowsDown)
			{
				if (overflowsBack)
					storedChunks[0] = StoreChunk(-1, -1, -1);
				storedChunks[1] = StoreChunk(-1, -1, 0);
				if (overflowsFront)
					storedChunks[2] = StoreChunk(-1, -1, 1);
			}
			if (overflowsBack)
				storedChunks[3] = StoreChunk(-1, 0, -1);
			storedChunks[4] = StoreChunk(-1, 0, 0);
			if (overflowsFront)
				storedChunks[5] = StoreChunk(-1, 0, 1);
			if (overflowsUp)
			{
				if (overflowsBack)
					storedChunks[6] = StoreChunk(-1, 1, -1);
				storedChunks[7] = StoreChunk(-1, 1, 0);
				if (overflowsFront)
					storedChunks[8] = StoreChunk(-1, 1, 1);
			}
		}
		if (overflowsDown)
		{
			if (overflowsBack)
				storedChunks[9] = StoreChunk(0, -1, -1);
			storedChunks[10] = StoreChunk(0, -1, 0);
			if (overflowsFront)
				storedChunks[11] = StoreChunk(0, -1, 1);
		}
		if (overflowsBack)
			storedChunks[12] = StoreChunk(0, 0, -1);
		if (overflowsFront)
			storedChunks[14] = StoreChunk(0, 0, 1);
		if (overflowsUp)
		{
			if (overflowsBack)
				storedChunks[15] = StoreChunk(0, 1, -1);
			storedChunks[16] = StoreChunk(0, 1, 0);
			if (overflowsFront)
			{
				storedChunks[17] = StoreChunk(0, 1, 1);
			}
		}
		if (overflowsRight)
		{
			if (overflowsDown)
			{
				if (overflowsBack)
					storedChunks[18] = StoreChunk(1, -1, -1);
				storedChunks[19] = StoreChunk(1, -1, 0);
				if (overflowsFront)
					storedChunks[20] = StoreChunk(1, -1, 1);
			}
			if (overflowsBack)
				storedChunks[21] = StoreChunk(1, 0, -1);
			storedChunks[22] = StoreChunk(1, 0, 0);
			if (overflowsFront)
				storedChunks[23] = StoreChunk(1, 0, 1);
			if (overflowsUp)
			{
				if (overflowsBack)
					storedChunks[24] = StoreChunk(1, 1, -1);
				storedChunks[25] = StoreChunk(1, 1, 0);
				if (overflowsFront)
					storedChunks[26] = StoreChunk(1, 1, 1);
			}
		}
		return storedChunks;
	}

	private Chunk StoreChunk(int xOf, int yOf, int zOf)
	{
		int c1DIndex = ConvertChunk3DIndexTo1D(_c.CX + xOf, _c.CY + yOf, _c.CZ + zOf);
		if (!World.I.GetChunkFromDictionaryWithChunk1DIndex(c1DIndex, out Chunk c))
			World.I.WorldUpdater.GenerateChunkTerrainData(c1DIndex, _c.CX + xOf, _c.CY + yOf, _c.CZ + zOf, out c);
		return c;
	}

	private Chunk GetCurrentChunk(int bX, int bY, int bZ, Chunk[] storedChunks)
	{
		var posX = bX < 0 ? -1 : 0 + bX >= CHUNK_SIZE ? 1 : 0;
		var posY = bY < 0 ? -1 : 0 + bY >= CHUNK_SIZE ? 1 : 0;
		var posZ = bZ < 0 ? -1 : 0 + bZ >= CHUNK_SIZE ? 1 : 0;

		if (posX == 0 && posY == 0 && posZ == 0)
			return (_c);
		return (storedChunks[(1 + posX) * 9 + (1 + posY) * 3 + (1 + posZ)]);
	}

	private void BoundBlockPositionsToChunkSize(ref int bX, ref int bY, ref int bZ)
	{
		BoundBlockPositionToChunkSize(ref bX);
		BoundBlockPositionToChunkSize(ref bY);
		BoundBlockPositionToChunkSize(ref bZ);
	}

	private void BoundBlockPositionToChunkSize(ref int bPos)
	{
		if (bPos < 0)
			bPos = CHUNK_SIZE + bPos;
		else if (bPos >= CHUNK_SIZE)
			bPos -= CHUNK_SIZE;
	}
}
