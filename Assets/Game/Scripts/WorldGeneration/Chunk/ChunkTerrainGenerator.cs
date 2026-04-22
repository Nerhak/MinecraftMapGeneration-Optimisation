using static WorldSettings;
using static Library.Legacy.PerlinNoiseMultiThread;
using static Library.Legacy.IndexConverter;
using static Library.Legacy.BlockTypesInfoGetter;
using static ChunkGenerationMethods;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using System.Diagnostics;

public class ChunkTerrainGenerator
{
	private readonly Chunk _c;

	public ChunkTerrainGenerator(Chunk chunk)
	{
		_c = chunk;
	}

	public void Generate()
	{
		if (_c.ChunkIOHandler.LoadChunkBlockTypesFromFile())
			GenerateFromFile();
		else
			GenerateFromScratch();
	}

	private void GenerateFromFile()
	{
		for (int i = 0; i < CHUNK_SIZE_CUBED; i++)
		{
			_c.BlockIsOpaque[i] = GetBlockIsOpaqueBoolFromBlockType(_c.Blocks[i]);
			_c.BlocksHP[i] = GetBlocksHPFromBlockType(_c.Blocks[i]);
		}
	}

	private void GenerateFromScratch()
	{
		NativeArray<int> bWSHAWPXZ = PopulateChunkBiomeWorldSurfaceHeightAtWorldPositionArray(_c);
		PopulateChunkTerrainData(bWSHAWPXZ);
		bWSHAWPXZ.Dispose();
	}

	private void PopulateChunkTerrainData(NativeArray<int> bWSHAWPXZ)
	{
		NativeArray<BlockTypes> blocks = new NativeArray<BlockTypes>(CHUNK_SIZE_CUBED, Allocator.TempJob);
		NativeArray<bool> blockIsOpaque = new NativeArray<bool>(CHUNK_SIZE_CUBED, Allocator.TempJob);
		NativeArray<sbyte> blocksHP = new NativeArray<sbyte>(CHUNK_SIZE_CUBED, Allocator.TempJob);

		var jobHandle = new PopulateChunkTerrainDataJob()
		{
			BWSHAWPXZ = bWSHAWPXZ,
			CWPX = _c.CWPX,
			CWPY = _c.CWPY,
			CWPZ = _c.CWPZ,
			CaveFreq = _c.CaveFreq,
			CaveAmp = _c.CaveAmp,
			CaveOct = _c.CaveOct,
			CavePers = _c.CavePers,
			CaveProb = _c.CaveProb,
			Blocks = blocks,
			BlockIsOpaque = blockIsOpaque,
			BlocksHP = blocksHP
		}.Schedule(CHUNK_SIZE_CUBED, 128);
		jobHandle.Complete();

		blocks.CopyTo(_c.Blocks);
		blockIsOpaque.CopyTo(_c.BlockIsOpaque);
		blocksHP.CopyTo(_c.BlocksHP);

		blocks.Dispose();
		blockIsOpaque.Dispose();
		blocksHP.Dispose();
	}

	[BurstCompile]
	public struct PopulateChunkTerrainDataJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<int> BWSHAWPXZ;
		[ReadOnly]
		public int CWPX, CWPY, CWPZ;
		[ReadOnly]
		public float CaveFreq, CaveAmp, CaveOct, CavePers, CaveProb;

		[WriteOnly]
		public NativeArray<BlockTypes> Blocks;
		[WriteOnly]
		public NativeArray<bool> BlockIsOpaque;
		[WriteOnly]
		public NativeArray<sbyte> BlocksHP;

		public void Execute(int index)
		{
			ConvertBlock1DIndexTo3D(index, out int bX, out int bY, out int bZ);
			Blocks[index] = SetBlock(index, GenerateBlockTypeData(bX, bY, bZ));
		}

		private BlockTypes GenerateBlockTypeData(int bX, int bY, int bZ)
		{
			ConvertBlock3DIndexToWorldPosition(bX, bY, bZ, CWPZ, CWPY, CWPZ, out int bWPX, out int bWPY, out int bWPZ);
			var bWSHAWPXZ = BWSHAWPXZ[ConvertBlock2DIndexTo1D(bX, bZ)];

			switch (bWPY)
			{
				case WORLD_NEGATIVE_Y_LIMIT:
					return (GenerateWorldNegativeYLimit());
				case var _ when (bWPY < bWSHAWPXZ):
					return (GenerateBelowSurface(bWPX, bWPY, bWPZ, bWSHAWPXZ));
				case var _ when (bWPY == bWSHAWPXZ):
					return (GenerateSurface());
				case var _ when (bWPY > bWSHAWPXZ):
					return (GenerateAboveSurface(bWPX, bWPY, bWPZ, bWSHAWPXZ));
			}
			return (GenerateNotGeneratedBlock());
		}

		public BlockTypes SetBlock(int b1DIndex, BlockTypes blockType)
		{
			BlockIsOpaque[b1DIndex] = GetBlockIsOpaqueBoolFromBlockType(blockType);
			BlocksHP[b1DIndex] = GetBlocksHPFromBlockType(blockType);
			return blockType;
		}

		private BlockTypes GenerateWorldNegativeYLimit()
		{
			return (BlockTypes.Bedrock);
		}

		private BlockTypes GenerateBelowSurface(int bWPX, int bWPY, int bWPZ, int bWSHAWPXZ)
		{
			if (IsCave(bWPX, bWPY, bWPZ))
				return BlockTypes.Air;
			switch (bWPY)
			{
				case var _ when bWPY >= bWSHAWPXZ - 5: // Magic number ? Refine.
					return BlockTypes.Dirt;
			}
			return BlockTypes.Stone;
		}

		private bool IsCave(int bWPX, int bWPY, int bWPZ)
		{
			return (FractalBrownianMotion3D(bWPX, bWPY, bWPZ, CaveFreq, CaveAmp, CaveOct, CavePers) < CaveProb);
		}

		private BlockTypes GenerateSurface()
		{
			return (BlockTypes.DirtWithGrass);
		}

		private BlockTypes GenerateAboveSurface(int bWPX, int bWPY, int bWPZ, int bWSHAWPXZ)
		{
			return (BlockTypes.Air);
		}

		private BlockTypes GenerateNotGeneratedBlock()
		{
			return (BlockTypes.Air);
		}
	}
}
