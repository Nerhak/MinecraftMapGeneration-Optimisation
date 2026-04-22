using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Data/Building", order = 1)]
public class BuildingData : ScriptableObject
{
	[Header("Settings")]
	[SerializeField] int _xNegSize = default;
	[SerializeField] int _xPosSize = default;
	[SerializeField] int _yNegSize = default;
	[SerializeField] int _yPosSize = default;
	[SerializeField] int _zNegSize = default;
	[SerializeField] int _zPosSize = default;
	[SerializeField] List<Vector3Int> _blocksPosition = default;
	[SerializeField] List<BlockTypes> _blocksTypes = default;

	public int XNegSize => _xNegSize;
	public int XPosSize => _xPosSize;
	public int YNegSize => _yNegSize;
	public int YPosSize => _yPosSize;
	public int ZNegSize => _zNegSize;
	public int ZPosSize => _zPosSize;
	public List<Vector3Int> BlocksPosition => _blocksPosition;
	public List<BlockTypes> BlocksTypes => _blocksTypes;
}

/*	[Tree]
 *	XNegSize = -2;
 *	XPosSize = 2;
 *	YNegSize = 0;
 *	YPosSize = 5;
 *	ZNegSize = -2;
 *	ZPosSize = 2;
 *	
 *	[-2]	[3]		[-1]	=> Leaf			0
 *	[-2]	[3]		[0]		=> Leaf			1
 *	[-2]	[3]		[1]		=> Leaf			2
 *	[-1]	[2]		[-1]	=> Leaf			3
 *	[-1]	[2]		[0]		=> Leaf			4
 *	[-1]	[2]		[1]		=> Leaf			5
 *	[-1]	[3]		[-2]	=> Leaf			6
 *	[-1]	[3]		[-1]	=> Leaf			7
 *	[-1]	[3]		[0]		=> Leaf			8
 *	[-1]	[3]		[1]		=> Leaf			9
 *	[-1]	[3]		[2]		=> Leaf			10
 *	[-1]	[4]		[0]		=> Leaf			11
 *	[0]		[0]		[0]		=> TreeTrunk	12
 *	[0]		[1]		[0]		=> TreeTrunk	13
 *	[0]		[2]		[-1]	=> Leaf			14
 *	[0]		[2]		[0]		=> TreeTrunk	15
 *	[0]		[2]		[1]		=> Leaf			16
 *	[0]		[3]		[-2]	=> Leaf			17
 *	[0]		[3]		[-1]	=> Leaf			18
 *	[0]		[3]		[0]		=> TreeTrunk	19
 *	[0]		[3]		[1]		=> Leaf			20
 *	[0]		[3]		[2]		=> Leaf			21
 *	[0]		[4]		[-1]	=> Leaf			22
 *	[0]		[4]		[0]		=> TreeTrunk	23
 *	[0]		[4]		[1]		=> Leaf			24
 *	[0]		[5]		[0]		=> Leaf			25
 *	[1]		[2]		[-1]	=> Leaf			26
 *	[1]		[2]		[0]		=> Leaf			27
 *	[1]		[2]		[1]		=> Leaf			28
 *	[1]		[3]		[-2]	=> Leaf			29
 *	[1]		[3]		[-1]	=> Leaf			30
 *	[1]		[3]		[0]		=> Leaf			31
 *	[1]		[3]		[1]		=> Leaf			32
 *	[1]		[3]		[2]		=> Leaf			33
 *	[1]		[4]		[0]		=> Leaf			34
 *	[2]		[3]		[-1]	=> Leaf			35
 *	[2]		[3]		[0]		=> Leaf			36
 *	[2]		[3]		[1]		=> Leaf			37
 */
