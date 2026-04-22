using UnityEngine;
using static WorldSettings;

public class Chunk : MonoBehaviour
{
	public ChunkIOHandler ChunkIOHandler { get; private set; }
	public ChunkTerrainGenerator ChunkTerrainGenerator { get; private set; }
	public ChunkBuildingsGenerator ChunkBuildingsGenerator { get; private set; }
	public ChunkMeshGenerator ChunkMeshGenerator { get; private set; }
	public ChunkMeshModifier ChunkMeshModifier { get; private set; }
	public MeshFilter MeshFilter;
	public MeshRenderer MeshRenderer;
	public MeshCollider MeshCollider;

	//Chunk Positions\\
	public int CID { get; private set; }//					ChunkID
	public int CX { get; private set; }//					ChunkIndexX
	public int CY { get; private set; }//					ChunkIndexY
	public int CZ { get; private set; }//					ChunkIndexZ
	public int CWPX { get; private set; }//					ChunkWorldPositionX
	public int CWPY { get; private set; }//					ChunkWorldPositionY
	public int CWPZ { get; private set; }//					ChunkWorldPositionZ

	//BlocksData
	public BlockTypes[] Blocks;
	public bool[] BlockIsOpaque;
	public sbyte[] BlocksHP;
	public int[] BWSHAWPXZ;//									BiomeWorldSurfaceHeightAtWorldPositionXZ

	public ChunkStates State { get; set; }

	//Biome settings\\
	public BiomeTypeData BioTDat { get; private set; }//		BiomeTypeData
	public float SHMin = default;//								SurfaceHeightMin
	public float SHMax = default;//								SurfaceHeightMax
	public float SFreq = default;//								SurfaceFrequency
	public float SAmp = default;//								SurfaceAmplitude
	public float SOct = default;//								SurfaceOctaves
	public float SPers = default;//								SurfacePersistence
	public float CaveFreq = default;//							CaveFrequency
	public float CaveAmp = default;//							CaveAmplitude
	public float CaveOct = default;//							CaveOctaves
	public float CavePers = default;//							CavePersistence
	public float CaveProb = default;//							CaveProbability
	/*
	 * => Quick to access constant settings would be faster
	 * => Could be at the very least a static singleton to load at awake then serve as ref
	*/
	

	public void Init(int cID, int cX, int cY, int cZ, BiomeTypeData bioTDat)
	{
		CID = cID;
		CX = cX;
		CY = cY;
		CZ = cZ;
		CWPX = CX * CHUNK_SIZE;
		CWPY = CY * CHUNK_SIZE;
		CWPZ = CZ * CHUNK_SIZE;

		BioTDat = bioTDat;
		StoreBiomeTypeDataLocally();

		Blocks = new BlockTypes[CHUNK_SIZE_CUBED];
		BlockIsOpaque = new bool[CHUNK_SIZE_CUBED];
		BlocksHP = new sbyte[CHUNK_SIZE_CUBED];
		BWSHAWPXZ = new int[CHUNK_SIZE_SQUARED];

		MeshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer = gameObject.AddComponent<MeshRenderer>();
		MeshCollider = gameObject.AddComponent<MeshCollider>();

		ChunkIOHandler = gameObject.AddComponent<ChunkIOHandler>();
		ChunkIOHandler.Init(this);
		ChunkTerrainGenerator = new ChunkTerrainGenerator(this);
		ChunkBuildingsGenerator = new ChunkBuildingsGenerator(this);
		ChunkMeshGenerator = new ChunkMeshGenerator(this);
		ChunkMeshModifier = new ChunkMeshModifier(this);
		State = ChunkStates.DEFAULT;
	}

	private void StoreBiomeTypeDataLocally()
	{
		SHMin = BioTDat.SurfaceHeightMin;
		SHMax = BioTDat.SurfaceHeightMax;
		SFreq = BioTDat.SurfaceFrequency;
		SAmp = BioTDat.SurfaceAmplitude;
		SOct = BioTDat.SurfaceOctaves;
		SPers = BioTDat.SurfacePersistence;
		CaveFreq = BioTDat.CaveFrequency;
		CaveAmp = BioTDat.CaveAmplitude;
		CaveOct = BioTDat.CaveOctaves;
		CavePers = BioTDat.CavePersistence;
		CaveProb = BioTDat.CaveProbability;
	}
}
