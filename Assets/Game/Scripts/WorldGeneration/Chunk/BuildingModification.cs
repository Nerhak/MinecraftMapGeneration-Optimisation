using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BuildingModification
{
	public int B1DIndex;
	public BlockTypes BlockType;
	public Chunk C;

	public BuildingModification(int b1DIndex, BlockTypes blockType, Chunk c)
	{
		B1DIndex = b1DIndex;
		BlockType = blockType;
		C = c;
	}
}