using System.IO;
using UnityEngine;
using static WorldSettings;

public class ChunkIOHandler : MonoBehaviour
{
	private Chunk _chunk;

	private string _chunkFileName;
	private string _chunkFilePath;

	public void Init(Chunk chunk)
	{
		_chunk = chunk;
		_chunkFileName = chunk.CID.ToString();
		_chunkFilePath = CHUNKS_DIRECTORY_PATH + _chunkFileName;
	}

	public bool LoadChunkBlockTypesFromFile()
	{
		if (ES3.FileExists(_chunkFilePath))
		{
			_chunk.Blocks = ES3.Load(_chunkFileName, _chunkFilePath, _chunk.Blocks);
			return _chunk.Blocks != null;
		}
		return false;
	}

	public void Activate()
	{
		_chunk.gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		_chunk.gameObject.SetActive(false);
	}

	public void SaveAndRemoveFromScene()
	{
		SaveChunkToFile();
		_chunk.Blocks = null;
		_chunk.BlockIsOpaque = null;
		_chunk.BWSHAWPXZ = null;
		_chunk.ChunkMeshGenerator.MVert = null;
		_chunk.ChunkMeshGenerator.MTri = null;
		_chunk.ChunkMeshGenerator.MUVs = null;
		_chunk.ChunkMeshGenerator.MCol = null;
		Destroy(_chunk.MeshFilter.mesh);
		Destroy(gameObject);
	}

	public void SaveChunkToFile()
	{
		if (!ES3.DirectoryExists(CHUNKS_DIRECTORY_PATH))
			Directory.CreateDirectory(CHUNKS_DIRECTORY_PATH);
		ES3.Save(_chunkFileName, _chunk.Blocks, _chunkFilePath);
	}
}
