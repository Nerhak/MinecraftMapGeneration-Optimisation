using UnityEngine;

[CreateAssetMenu(fileName = "BiomeTypeDataList", menuName = "Data/BiomeTypeDataList", order = 1)]
public class BiomeTypeDataList : ScriptableObject
{
	//Constructors
	[SerializeField] private BiomeTypeData _default = default;
	
	//Exposers
	public BiomeTypeData Default => _default;
}
