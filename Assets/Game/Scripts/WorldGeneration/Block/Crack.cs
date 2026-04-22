using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Library.Coroutines;
using static Library.Legacy.BlockTypesInfoGetter;

public class Crack : MonoBehaviour
{
	private Chunk _c;
	private Vector3Int _wP;
	private int _b1DIndex;
	private Coroutine _regenCoroutine;
	private Mesh _mesh;
	private byte _damage;

	private void Awake()
	{
		_mesh = GetComponent<MeshFilter>().mesh;
	}

	public void Init(Vector3Int wP, int b1DIndex, Chunk c, sbyte blockMaxHP)
	{
		_wP = wP;
		_b1DIndex = b1DIndex;
		_c = c;
		World.I.CrackDictionary.Add(wP, this);
		_regenCoroutine = StartCoroutine(Regenerate());
		_damage = (byte)(9 - blockMaxHP);
		UpdateGraphicalDamage();
	}

	public void TakeDamage()
	{
		_damage++;
		UpdateGraphicalDamage();
		ResetRegenTimer();
	}

	private void UpdateGraphicalDamage()
	{
		Vector3[] vertices = _mesh.vertices;
		List<Vector3> uvs = new List<Vector3>();

		for (int i = 0; i < vertices.Length; i++)
			uvs.Add(new Vector3(_mesh.uv[i].x, _mesh.uv[i].y, _damage));
		_mesh.SetUVs(0, uvs.ToArray());
	}

	private void ResetRegenTimer()
	{
		StopCoroutine(_regenCoroutine);
		_regenCoroutine = StartCoroutine(Regenerate());
	}

	private IEnumerator Regenerate()
	{
		yield return Wait.ForFourSeconds;
		World.I.CrackDictionary.Remove(_wP);
		_c.BlocksHP[_b1DIndex] = GetBlocksHPFromBlockType(_c.Blocks[_b1DIndex]);
		Destroy(gameObject);
	}
}
