using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static WorldSettings;

public static class ChunkGenerationMethods
{
	public static NativeArray<int> PopulateChunkBiomeWorldSurfaceHeightAtWorldPositionArray(Chunk c)
	{
		NativeArray<int> bWSHAWPXZ = new NativeArray<int>(CHUNK_SIZE_SQUARED, Allocator.TempJob);

		var jobHandle = new PopulateChunkBiomeWorldHeightAtWorldPositionArrayJob()
		{
			BWSHAWPXZ = bWSHAWPXZ,
			ChunkSize = CHUNK_SIZE,
			CWPX = c.CWPX,
			CWPZ = c.CWPZ,
			SHMin = c.SHMin,
			SHMax = c.SHMax,
			SFreq = c.SFreq,
			SAmp = c.SAmp,
			SOct = c.SOct,
			SPers = c.SPers
		}.Schedule(CHUNK_SIZE_SQUARED, 128);
		jobHandle.Complete();
		return bWSHAWPXZ;
	}
}
