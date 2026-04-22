using UnityEngine;

[CreateAssetMenu(fileName = "BiomeTypeData", menuName = "Data/BiomeTypeData", order = 1)]
public class BiomeTypeData : ScriptableObject
{
	//Constructor
	[SerializeField] private BiomeTypes _biomeType = default;
	//	Surface Perlin Noise Values
	[SerializeField] private float _surfaceHeightMin = 100f;
	[SerializeField] private float _surfaceHeightMax = 150f;
	[SerializeField] private float _surfaceFrequency = 0.1f;
	[SerializeField] private float _surfaceAmplitude = 1f;
	[SerializeField] private int _surfaceOctaves = 1;
	[SerializeField] private float _surfacePersistence = 0.5f;
	//	Caves Perlin Noise Values
	[SerializeField] private float _caveFrequency = 0.1f;
	[SerializeField] private float _caveAmplitude = 1f;
	[SerializeField] private int _caveOctaves = 1;
	[SerializeField] private float _cavePersistence = 0.5f;
	[SerializeField] private float _caveProbability = 0.45f;

	//Exposers
	public BiomeTypes BiomeType => _biomeType;
	//	Surface Perlin Noise Values
	public float SurfaceHeightMin => _surfaceHeightMin;
	public float SurfaceHeightMax => _surfaceHeightMax;
	public float SurfaceFrequency => _surfaceFrequency;
	public float SurfaceAmplitude => _surfaceAmplitude;
	public int SurfaceOctaves => _surfaceOctaves;
	public float SurfacePersistence => _surfacePersistence;
	//	Caves Perlin Noise Values
	public float CaveFrequency => _caveFrequency;
	public float CaveAmplitude => _caveAmplitude;
	public int CaveOctaves => _caveOctaves;
	public float CavePersistence => _cavePersistence;
	public float CaveProbability => _caveProbability;
}
