using UnityEngine;
using System.Collections.Generic;
using static WorldSettings;
using static Library.Legacy.IndexConverter;
using UnityEngine.PlayerLoop;

public class World : MonoSingleton<World>
{
	#region Inspector
	[Header("Dev")]
	[SerializeField] private GameObject _devControllerPrefab = default;
	[SerializeField] private bool _deleteWorldSaveAtLaunch = default;
	[SerializeField] private bool _saveWorldAtApplicationQuit = default;
	[Range(0f, 1f)]
	[SerializeField] private float _globalLightLevel = 1f;

	[Header("Injections")]
	[SerializeField] private BlockTypeDataList _blockTypeDataList = default;
	[SerializeField] private BiomeTypeDataList _biomeTypeDataList = default;
	[SerializeField] private BuildingDataList _buildingDataList = default;
	[SerializeField] private Material _worldMaterial = default;
	[SerializeField] private PhysicMaterial _chunkPhysicMaterial = default;
	[SerializeField] private GameObject _crackingOverlay = default;
	[SerializeField] private Transform _chunksParent = default;
	#endregion

	#region Exposed
	public BlockTypeDataList BlockTypeDataList => _blockTypeDataList;
	public BiomeTypeDataList BiomeTypeDataList => _biomeTypeDataList;
	public BuildingDataList BldDatLst => _buildingDataList;
	public Material WorldMaterial => _worldMaterial;
	public PhysicMaterial ChunkPhysicMaterial => _chunkPhysicMaterial;
	public GameObject CrackingOverlay => _crackingOverlay;
	public Transform ChunksParent => _chunksParent;
	public Dictionary<int, Chunk> ChunksDictionary { get; private set; }
	public Dictionary<Vector3Int, Crack> CrackDictionary { get; private set; }
	#endregion

	public WorldUpdater WorldUpdater;

	#region Awake
	private void Awake()
	{
		ChunksDictionary = new Dictionary<int, Chunk>();
		CrackDictionary = new Dictionary<Vector3Int, Crack>();
		WorldUpdater = new WorldUpdater();

		//Dev
		if (_deleteWorldSaveAtLaunch)
		{
			if (ES3.DirectoryExists(WORLD_SAVING_DIRECTORY))
				ES3.DeleteDirectory(WORLD_SAVING_DIRECTORY);
		}
		SpawnDevController();

		SetShadersMinMaxLightLevel();

		StartCoroutine(WorldUpdater.UpdateWorldAroundPlayer());
	}

	private void SpawnDevController()
	{
		var devControllerGO = Instantiate(_devControllerPrefab, new Vector3(0f,	140f, 0f), Quaternion.identity);
		devControllerGO.name = _devControllerPrefab.name;
		devControllerGO.GetComponent<Rigidbody>().useGravity = false;
		//Adjust Later
		WorldUpdater.PlayerWP = devControllerGO.transform;
	}

	private void SetShadersMinMaxLightLevel()
	{
		Shader.SetGlobalFloat("MinGlobalLightLevel", MIN_GLOBAL_LIGHT_LEVEL);
		Shader.SetGlobalFloat("MaxGlobalLightLevel", MAX_GLOBAL_LIGHT_LEVEL);
	}
	#endregion

	private void Update()
	{
		Shader.SetGlobalFloat("GlobalLightLevel", _globalLightLevel);
	}

	#region Public Methods
	public bool GetChunkFromDictionaryWithChunk1DIndex(int cID, out Chunk c)
	{
		return ChunksDictionary.TryGetValue(cID, out c);
	}

	public bool GetChunkFromDictionaryWithChunkIndex(int cX, int cY, int cZ, out Chunk c)
	{
		 return ChunksDictionary.TryGetValue(ConvertChunk3DIndexTo1D(cX, cY, cZ), out c);
	}

	public bool GetChunkFromDictionaryWithWorldPosition(Vector3 wP, out Chunk c)
	{
		ConvertWorldPositionToChunk3DIndex(wP, out int cX, out int cY, out int cZ);
		return GetChunkFromDictionaryWithChunkIndex(cX, cY, cZ, out c);
	}

	public bool GetCrackFromDictionaryWithWorldPosition(Vector3Int wP, out Crack crack)
	{
		return CrackDictionary.TryGetValue(wP, out crack);
	}
	#endregion

	private void OnApplicationQuit()
	{
		if (_saveWorldAtApplicationQuit)
			SaveChunks();
	}

	private void SaveChunks()
	{
		foreach (KeyValuePair<int, Chunk> chunk in ChunksDictionary)
			chunk.Value.ChunkIOHandler.SaveChunkToFile();
	}
}