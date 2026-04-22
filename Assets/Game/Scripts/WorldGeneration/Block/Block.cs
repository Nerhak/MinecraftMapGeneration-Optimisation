using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
public class Block
{
	public BlockTypeData BlockTypeData;
	//public BlockTypes BlockType { get; private set; }
	//public bool IsOpaque { get; private set; }
	//public bool IsSolid { get; private set; }
	//public bool IsWalkable { get; private set; }

	public Block(BlockTypeData blockTypeData)
	{
		BlockTypeData = blockTypeData;
	}
}
