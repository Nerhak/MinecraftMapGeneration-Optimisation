using static Library.Legacy.PerlinNoiseMultiThread;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

[BurstCompile]
public struct PopulateChunkBiomeWorldHeightAtWorldPositionArrayJob : IJobParallelFor
{
	[ReadOnly]
	public int CWPX, CWPY, CWPZ, ChunkSize;
	[ReadOnly]
	public float SHMin, SHMax, SFreq, SAmp, SOct, SPers;

	[WriteOnly]
	public NativeArray<int> BWSHAWPXZ;

	private int _x, _bWPX, _bWPZ;
	public void Execute(int index)
	{
		_x = index / ChunkSize;
		_bWPX = _x + CWPX;
		_bWPZ = index - _x * ChunkSize + CWPZ;
		BWSHAWPXZ[index] = GenerateBiomeWorldSurfaceHeightAtWorldPositionXZ(_bWPX, _bWPZ, SHMin, SHMax, SFreq, SAmp, SOct, SPers);
	}
}
