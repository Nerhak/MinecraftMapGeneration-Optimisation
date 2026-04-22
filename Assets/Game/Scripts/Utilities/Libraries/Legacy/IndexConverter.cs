using Unity.Mathematics;
using UnityEngine;
using static WorldSettings;

namespace Library.Legacy
{
	public static class IndexConverter
	{
		public static void ConvertWorldPositionToChunk3DIndex(Vector3 wP, out int cX, out int cY, out int cZ)
		{
			cX = Mathf.FloorToInt(wP.x / CHUNK_SIZE);
			cY = Mathf.FloorToInt(wP.y / CHUNK_SIZE);
			cZ = Mathf.FloorToInt(wP.z / CHUNK_SIZE);
		}

		public static int ConvertChunk3DIndexTo1D(int x, int y, int z)
		{
			return x * WORLD_SIZE_SQUARED + y * WORLD_SIZE + z;
		}

		public static void ConvertBlockWorldPositionToFlooredToIntWorldPosition(Vector3 wP,
			out int bWPX, out int bWPY, out int bWPZ)
		{
			bWPX = Mathf.FloorToInt(wP.x);
			bWPY = Mathf.FloorToInt(wP.y);
			bWPZ = Mathf.FloorToInt(wP.z);
		}

		public static Vector3Int ConvertBlockWorldPositionToFlooredToIntVector3IntWorldPosition(Vector3 wP)
		{
			return new Vector3Int(Mathf.FloorToInt(wP.x), Mathf.FloorToInt(wP.y), Mathf.FloorToInt(wP.z));
		}

		public static void ConvertBlockWorldPositionTo3D(Vector3 wP,
			out int bX, out int bY, out int bZ)
		{
			bX = Mathf.FloorToInt(wP.x);
			bY = Mathf.FloorToInt(wP.y);
			bZ = Mathf.FloorToInt(wP.z);

			bX = bX < 0 ? ConvertNegativeBlockWorldPositionAxis(bX) : ConvertPositiveBlockWorldPositionAxis(bX);
			bY = bY < 0 ? ConvertNegativeBlockWorldPositionAxis(bY) : ConvertPositiveBlockWorldPositionAxis(bY);
			bZ = bZ < 0 ? ConvertNegativeBlockWorldPositionAxis(bZ) : ConvertPositiveBlockWorldPositionAxis(bZ);
		}

		public static int ConvertBlockWorldPositionTo1D(Vector3 wP)
		{
			ConvertBlockWorldPositionTo3D(wP, out int bX, out int bY, out int bZ);
			return ConvertBlock3DIndexTo1D(bX, bY, bZ);
		}
		
		private static int ConvertPositiveBlockWorldPositionAxis(int value)
		{
			return value -= (value / CHUNK_SIZE) * CHUNK_SIZE;
		}

		private static int ConvertNegativeBlockWorldPositionAxis(int value)
		{
			var result = CHUNK_SIZE + value + CHUNK_SIZE * (-value / CHUNK_SIZE);
			return (result != 32 ? result : 0);
		}


		public static void ConvertBlock3DIndexToWorldPosition(int bX, int bY, int bZ, int cWPX, int cWPY, int cWPZ,
			out int bWPX, out int bWPY, out int bWPZ)
		{
			bWPX = bX + cWPX;
			bWPY = bY + cWPY;
			bWPZ = bZ + cWPZ;
		}

		public static void ConvertBlockXZ3DIndexToWorldPosition(int bX, int bZ, int cWPX, int cWPY, int cWPZ,
			out int bWPX, out int bWPZ)
		{
			bWPX = bX + cWPX;
			bWPZ = bZ + cWPZ;
		}

		public static int ConvertBlock3DIndexTo1D(int x, int y, int z)
		{
			return Mathf.Abs(x * CHUNK_SIZE_SQUARED + (y * CHUNK_SIZE) + z);
		}

		public static int ConvertBlock3DIndexTo1DMultiThread(int x, int y, int z, int ChunkSize, int ChunkSizeSquared)
		{
			return math.abs(x * ChunkSizeSquared + (y * ChunkSize) + z);
		}

		public static int ConvertBlock2DIndexTo1D(int bX, int bZ)
		{
			return bZ + bX * CHUNK_SIZE;
		}

		public static void ConvertBlock1DIndexTo3D(int index, out int bX, out int bY, out int bZ)
		{
			bX = index / (CHUNK_SIZE * CHUNK_SIZE);
			bY = index / CHUNK_SIZE % CHUNK_SIZE;
			bZ = index % CHUNK_SIZE;
		}

		public static int ConvertBlockIndexToNeighborChunkBlockIndex(int index)
		{
			if (index < 0)
				return (CHUNK_SIZE + index);
			if (index >= CHUNK_SIZE)
				return (index - CHUNK_SIZE);
			return (index);
		}

		public static Vector3 ConvertHitToInnerBlockWorldPosition(RaycastHit hit)
		{
			return hit.point - (hit.normal * 0.1f);
		}

		public static Vector3 ConvertHitToOuterBlockWorldPosition(RaycastHit hit)
		{
			return hit.point + (hit.normal * 0.1f);
		}

	}
}