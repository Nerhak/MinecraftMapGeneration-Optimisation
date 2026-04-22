using UnityEngine;

[CreateAssetMenu(fileName = "BlockTypeDataList", menuName = "Data/BlockTypeDataList", order = 1)]
public class BlockTypeDataList : ScriptableObject
{
	[Header("Injections")]
	[SerializeField] private BlockTypeData _air = default;
	[SerializeField] private BlockTypeData _dirt = default;
	[SerializeField] private BlockTypeData _dirtWithGrass = default;
	[SerializeField] private BlockTypeData _bedrock = default;
	[SerializeField] private BlockTypeData _stone = default;
	[SerializeField] private BlockTypeData _treeTrunk = default;
	[SerializeField] private BlockTypeData _treeLeaf = default;
	[SerializeField] private BlockTypeData _glass = default;

	//Exposed
	public BlockTypeData Air => _air;
	public BlockTypeData Dirt => _dirt;
	public BlockTypeData DirtWithGrass => _dirtWithGrass;
	public BlockTypeData Bedrock => _bedrock;
	public BlockTypeData Stone => _stone;
	public BlockTypeData TreeTrunk => _treeTrunk;
	public BlockTypeData TreeLeaf => _treeLeaf;
	public BlockTypeData Glass => _glass;
}
