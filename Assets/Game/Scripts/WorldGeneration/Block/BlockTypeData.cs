using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockTypeData", menuName = "Data/BlockTypeData", order = 1)]
public class BlockTypeData : ScriptableObject
{
	//Constructor
	[SerializeField] private BlockTypes _blockType = default; //_blockType
	[SerializeField] private bool _isOpaque = default;
	[SerializeField] private bool _isSolid = default;
	[SerializeField] private bool _isWalkable = default;
	//	TextureArrayIDs
	[SerializeField] private uint _tALFLeft = default;//_textureArrayLayerFaceLeft
	[SerializeField] private uint _tALFRight = default;//_textureArrayLayerFaceRight
	[SerializeField] private uint _tALFDown = default;//_textureArrayLayerFaceDown
	[SerializeField] private uint _tALFUp = default;//_textureArrayLayerFaceUp
	[SerializeField] private uint _tALFBack = default;//_textureArrayLayerFaceBack
	[SerializeField] private uint _tALFFront = default;//_textureArrayLayerFaceFront

	//Exposers
	public BlockTypes BlockType => _blockType;
	public bool IsOpaque => _isOpaque;
	public bool IsSolid => _isSolid;
	public bool IsWalkable => _isWalkable;
	//	TextureArrayIDs
	public uint TALFLeft => _tALFLeft;
	public uint TALFRight => _tALFRight;
	public uint TALFDown => _tALFDown;
	public uint TALFUp => _tALFUp;
	public uint TALFBack => _tALFBack;
	public uint TALFFront => _tALFFront;
}
