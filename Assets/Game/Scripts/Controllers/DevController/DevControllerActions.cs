using Unity.Mathematics;
using UnityEngine;
using static Library.Legacy.IndexConverter;
using static Library.Legacy.BlockTypesInfoGetter;

namespace Legacy
{
	public class DevControllerActions : MonoBehaviour
	{
		[Header("Injections")]
		[SerializeField] private Transform _camera = default;

		[Header("Settings")]
		[SerializeField] private float _blockInteractionRange = default;

		private bool GetBlockAtLookPosition(out RaycastHit hit)
		{
			if (Physics.Raycast(_camera.position, _camera.forward, out hit, _blockInteractionRange))
				return (true);
			return (false);
		}

		private void ChangeBlockAtWorldPositionTo(Vector3 wP, BlockTypes blockType)
		{
			if (World.I.GetChunkFromDictionaryWithWorldPosition(wP, out Chunk c))
			{
				c.ChunkMeshModifier.ChangeBlock(ConvertBlockWorldPositionTo1D(wP), blockType);
				c.ChunkMeshGenerator.Generate();
			}
		}

		public void PlaceBlock()
		{
			if (GetBlockAtLookPosition(out RaycastHit hit))
				ChangeBlockAtWorldPositionTo(ConvertHitToOuterBlockWorldPosition(hit), BlockTypes.Glass);
		}

		public void DestroyBlock()
		{
			if (GetBlockAtLookPosition(out RaycastHit hit))
			{
				Vector3 wP = ConvertHitToInnerBlockWorldPosition(hit);
				ChangeBlockAtWorldPositionTo(wP, BlockTypes.Air);
				DeleteCrack(wP);
			}
		}

		public void DamageBlock()
		{
			if (GetBlockAtLookPosition(out RaycastHit hit))
			{
				Vector3 wP = ConvertHitToInnerBlockWorldPosition(hit);
				if (World.I.GetChunkFromDictionaryWithWorldPosition(wP, out Chunk c))
				{
					int b1DIndex = ConvertBlockWorldPositionTo1D(wP);
					if (BlockIsDamageable(c.BlocksHP[b1DIndex]))
					{
						c.BlocksHP[b1DIndex]--;
						Vector3Int crackID = ConvertBlockWorldPositionToFlooredToIntVector3IntWorldPosition(wP);
						if (c.BlocksHP[b1DIndex] > 0)
						{
							if (World.I.GetCrackFromDictionaryWithWorldPosition(crackID, out Crack crack))
								crack.TakeDamage();
							else
								SpawnCrack(wP, crackID, b1DIndex, c, GetBlocksHPFromBlockType(c.Blocks[b1DIndex]));
						}
						else
						{
							ChangeBlockAtWorldPositionTo(wP, BlockTypes.Air);
							DeleteCrack(wP);
						}
					}
				}
			}
		}

		private bool BlockIsDamageable(sbyte bHP)
		{
			return bHP > 0;
		}

		private void SpawnCrack(Vector3 wP, Vector3Int crackID, int b1DIndex, Chunk c, sbyte bMaxHP)
		{
			ConvertBlockWorldPositionToFlooredToIntWorldPosition(wP, out int bWPX, out int bWPY, out int bWPZ);
			GameObject crackGO = Instantiate(World.I.CrackingOverlay, new Vector3(bWPX + 0.5f, bWPY + 0.5f, bWPZ + 0.5f), quaternion.identity);
			var crackScript = crackGO.AddComponent<Crack>();
			crackScript.Init(crackID, b1DIndex, c, bMaxHP);
		}

		private void DeleteCrack(Vector3 wP)
		{
			Vector3Int crackID = ConvertBlockWorldPositionToFlooredToIntVector3IntWorldPosition(wP);
			if (World.I.GetCrackFromDictionaryWithWorldPosition(crackID, out Crack crack))
			{
				World.I.CrackDictionary.Remove(crackID);
				Destroy(crack.gameObject);
			}
		}
	}
}