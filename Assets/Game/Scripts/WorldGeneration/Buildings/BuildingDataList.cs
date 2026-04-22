using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDataList", menuName = "Data/BuildingDataList", order = 1)]
public class BuildingDataList : ScriptableObject
{
	[Header("Injections")]
	[SerializeField] private BuildingData _tree = default;

	public BuildingData Tree => _tree;
}
