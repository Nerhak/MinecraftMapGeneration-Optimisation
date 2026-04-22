using UnityEngine;
using System.Linq;
using System.Collections;
using Library.Coroutines;
using static WorldSettings;
using static Library.Legacy.IndexConverter;

public class WorldUpdater
{
	private Chunk _c;

	public Transform PlayerWP;
	private int _lastGenChunkX;
	private int _lastGenChunkY;
	private int _lastGenChunkZ;

	public IEnumerator UpdateWorldAroundPlayer()
	{
		while (true)
		{
			ConvertWorldPositionToChunk3DIndex(PlayerWP.position, out int cX, out int cY, out int cZ);
			if (cX != _lastGenChunkX || cY != _lastGenChunkY || cZ != _lastGenChunkZ)
			{
				//var exTime = Time.realtimeSinceStartup;
				//Debug.Log($"GenerateWorldAroundChunkIndex:{Time.realtimeSinceStartup - exTime}");
				
				yield return GenerateWorldAroundChunkIndex(cX, cY, cZ);
				DeactivateUnneededChunks();//Must evolve to deactivate chunks, not destroy them.
				UnmarkToKeepChunksForNextCycle();
				_lastGenChunkX = cX; _lastGenChunkY = cY; _lastGenChunkZ = cZ;
			}
			yield return Wait.ForEndOfFrame;
		}
	}

	private IEnumerator GenerateWorldAroundChunkIndex(int cX, int cY, int cZ)
	{
		BoundWorldGenerationAroundPlayerWithinWorldSizeLimit(cX, cY, cZ,
			out int startX, out int startY, out int startZ);

		int preCalcX, preCalcXY;
		for (int x = startX; x < cX + CHUNK_GENERATION_RADIUS && x < WORLD_SIZE; x++)
		{
			preCalcX = x * WORLD_SIZE_SQUARED;
			for (int y = startY; y < cY + CHUNK_GENERATION_RADIUS && y < WORLD_SIZE; y++)
			{
				preCalcXY = preCalcX + y * WORLD_SIZE;
				for (int z = startZ; z < cZ + CHUNK_GENERATION_RADIUS && z < WORLD_SIZE; z++)
				{
					if (ChunkNeedsToBeGenerated(preCalcXY + z, out Chunk c))
					{
						GenerateChunkTerrainData(preCalcXY + z, x, y, z, out c);
						GenerateChunkBuildingsData(c);
					}
					else
					{
						if (c.State == ChunkStates.DEFAULT)
							GenerateChunkBuildingsData(c);
					}
					SetChunkStateToGenerate(c);
					GenerateChunkMeshs(c);
					SetChunksStateToKeep(c);
					yield return Wait.ForEndOfFrame;
				}
			}
		}
		yield return 0;
	}

	private void BoundWorldGenerationAroundPlayerWithinWorldSizeLimit(int cX, int cY, int cZ,
		out int startX, out int startY, out int startZ)
	{
		startX = cX - CHUNK_GENERATION_RADIUS + 1 < -WORLD_SIZE ? -WORLD_SIZE : cX - CHUNK_GENERATION_RADIUS + 1;
		startY = cY - CHUNK_GENERATION_RADIUS + 1 < -WORLD_SIZE ? -WORLD_SIZE : cY - CHUNK_GENERATION_RADIUS + 1;
		startZ = cZ - CHUNK_GENERATION_RADIUS + 1 < -WORLD_SIZE ? -WORLD_SIZE : cZ - CHUNK_GENERATION_RADIUS + 1;
	}

	private bool ChunkNeedsToBeGenerated(int chunkId, out Chunk c)
	{
		if (World.I.GetChunkFromDictionaryWithChunk1DIndex(chunkId, out c))
		{
			c.ChunkIOHandler.Activate();
			return (false);
		}
		c = null;
		return (true);
	}
	
	public void GenerateChunkTerrainData(int chunkID, int cX, int cY, int cZ, out Chunk c)
	{
		var cGO = new GameObject(chunkID.ToString());
		c = cGO.AddComponent<Chunk>();
		c.Init(chunkID, cX, cY, cZ, World.I.BiomeTypeDataList.Default);
		cGO.transform.position = new Vector3Int(c.CWPX, c.CWPY, c.CWPZ);
		cGO.transform.parent = World.I.ChunksParent;

		c.ChunkTerrainGenerator.Generate();
		World.I.ChunksDictionary.Add(chunkID, c);
	}

	private void SetChunkStateToGenerate(Chunk c)
	{
		c.State = ChunkStates.GENERATE;
	}

	private void GenerateChunkBuildingsData(Chunk c)
	{
		c.ChunkBuildingsGenerator.Generate();
	}

	private void GenerateChunkMeshs(Chunk c)
	{
		if (c.State == ChunkStates.GENERATE)
			c.ChunkMeshGenerator.Generate();
	}

	private void SetChunksStateToKeep(Chunk chunk)
	{
		chunk.State = ChunkStates.KEEP;
	}

	private void DeactivateUnneededChunks()
	{
		var chunksToDeactivate = World.I.ChunksDictionary.
			Where(loadedChunk => loadedChunk.Value.State != ChunkStates.KEEP).ToArray();
		foreach (var chunkToDeactivate in chunksToDeactivate)
			chunkToDeactivate.Value.ChunkIOHandler.Deactivate();
	}

	private void SaveAndRemoveUnneededChunks()
	{
		var chunksToUnload = World.I.ChunksDictionary.
			Where(loadedChunk => loadedChunk.Value.State != ChunkStates.KEEP).ToArray();
		foreach (var chunkToUnload in chunksToUnload)
		{
			chunkToUnload.Value.ChunkIOHandler.SaveAndRemoveFromScene();
			World.I.ChunksDictionary.Remove(chunkToUnload.Key);
		}
	}

	private void UnmarkToKeepChunksForNextCycle()
	{
		var chunksToUnmark = World.I.ChunksDictionary.
			Where(loadedChunk => loadedChunk.Value.State == ChunkStates.KEEP).ToArray();
		foreach (var loadedChunk in chunksToUnmark)
			loadedChunk.Value.State = ChunkStates.GENERATED;
	}
}
