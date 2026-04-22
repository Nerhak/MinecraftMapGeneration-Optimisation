using UnityEngine;
using static Library.Legacy.IndexConverter;
using static WorldSettings;
using static Library.Legacy.BlockTypesInfoGetter;

public class ChunkMeshModifier
{
	private Chunk _c;

	public ChunkMeshModifier(Chunk c)
	{
		_c = c;
	}

	public void ChangeBlock(int b1DIndex, BlockTypes blockType)
	{
		_c.Blocks[b1DIndex] = blockType;
		_c.BlockIsOpaque[b1DIndex] = GetBlockIsOpaqueBoolFromBlockType(blockType);
		_c.BlocksHP[b1DIndex] = GetBlocksHPFromBlockType(blockType);
	}
}
