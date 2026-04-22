#if false
using UnityEngine;
using static WorldSettings;
using Unity.Collections;
using Unity.Jobs;

public partial class ChunkMeshGenerator2
{
	private readonly Chunk _c;

	public ChunkMeshGenerator2(Chunk chunk)
	{
		_c = chunk;
	}

	public void Generate()
	{
		var blocks = CreateBlocksPopulatedNativeArray();
		var blockIsOpaque = CreateBlockIsOpaquePopulatedNativeArray();
		NativeList<Vector3> meshVertices = new NativeList<Vector3>(Allocator.Persistent);
		NativeList<int> meshTriangles = new NativeList<int>(Allocator.Persistent);
		NativeList<Vector3> meshUVs = new NativeList<Vector3>(Allocator.Persistent);
		NativeList<Color32> meshColors = new NativeList<Color32>(Allocator.Persistent);

		var GenerateChunkMeshSliceNegativeXJobHandle = new GenerateChunkMeshSliceXJob()
		{
			Blocks = blocks,
			BlockIsOpaque = blockIsOpaque,
			IsNegFace = true,
			MeshSliceVertices = meshVertices,
			MeshSliceTriangles = meshTriangles,
			MeshSliceUVs = meshUVs,
			MeshSliceColors = meshColors
		}.Schedule();

		var GenerateChunkMeshSlicePositiveXJobHandle = new GenerateChunkMeshSliceXJob()
		{
			Blocks = blocks,
			BlockIsOpaque = blockIsOpaque,
			IsNegFace = false,
			MeshSliceVertices = meshVertices,
			MeshSliceTriangles = meshTriangles,
			MeshSliceUVs = meshUVs,
			MeshSliceColors = meshColors
		}.Schedule(GenerateChunkMeshSliceNegativeXJobHandle);

		var GenerateChunkMeshSliceNegativeYJobHandle = new GenerateChunkMeshSliceYJob()
		{
			Blocks = blocks,
			BlockIsOpaque = blockIsOpaque,
			IsNegFace = true,
			MeshSliceYVertices = meshVertices,
			MeshSliceYTriangles = meshTriangles,
			MeshSliceYUVs = meshUVs,
			MeshSliceYColors = meshColors
		}.Schedule(GenerateChunkMeshSlicePositiveXJobHandle);

		var GenerateChunkMeshSlicePositiveYJobHandle = new GenerateChunkMeshSliceYJob()
		{
			Blocks = blocks,
			BlockIsOpaque = blockIsOpaque,
			IsNegFace = false,
			MeshSliceYVertices = meshVertices,
			MeshSliceYTriangles = meshTriangles,
			MeshSliceYUVs = meshUVs,
			MeshSliceYColors = meshColors
		}.Schedule(GenerateChunkMeshSliceNegativeYJobHandle);

		var GenerateChunkMeshSliceNegativeZJobHandle = new GenerateChunkMeshSliceZJob()
		{
			Blocks = blocks,
			BlockIsOpaque = blockIsOpaque,
			IsNegFace = true,
			MeshSliceVertices = meshVertices,
			MeshSliceTriangles = meshTriangles,
			MeshSliceUVs = meshUVs,
			MeshSliceColors = meshColors
		}.Schedule(GenerateChunkMeshSlicePositiveYJobHandle);

		var GenerateChunkMeshSlicePositiveZJobHandle = new GenerateChunkMeshSliceZJob()
		{
			Blocks = blocks,
			BlockIsOpaque = blockIsOpaque,
			IsNegFace = false,
			MeshSliceVertices = meshVertices,
			MeshSliceTriangles = meshTriangles,
			MeshSliceUVs = meshUVs,
			MeshSliceColors = meshColors
		}.Schedule(GenerateChunkMeshSliceNegativeZJobHandle);

		GenerateChunkMeshSliceNegativeXJobHandle.Complete();
		GenerateChunkMeshSlicePositiveXJobHandle.Complete();
		GenerateChunkMeshSliceNegativeYJobHandle.Complete();
		GenerateChunkMeshSlicePositiveYJobHandle.Complete();
		GenerateChunkMeshSliceNegativeZJobHandle.Complete();
		GenerateChunkMeshSlicePositiveZJobHandle.Complete();

		SetChunkMeshData(meshVertices.ToArray(), meshTriangles.ToArray(), meshUVs.ToArray(), meshColors.ToArray());

		blocks.Dispose();
		blockIsOpaque.Dispose();
		meshVertices.Dispose();
		meshTriangles.Dispose();
		meshUVs.Dispose();
		meshColors.Dispose();
	}

	private NativeArray<BlockTypes> CreateBlocksPopulatedNativeArray()
	{
		NativeArray<BlockTypes> blocks = new NativeArray<BlockTypes>(CHUNK_SIZE_CUBED, Allocator.Persistent);
		blocks.CopyFrom(_c.Blocks);
		return (blocks);
	}

	private NativeArray<bool> CreateBlockIsOpaquePopulatedNativeArray()
	{
		NativeArray<bool> blockIsOpaque = new NativeArray<bool>(CHUNK_SIZE_CUBED, Allocator.Persistent);
		blockIsOpaque.CopyFrom(_c.BlockIsOpaque);
		return (blockIsOpaque);
	}

	private void SetChunkMeshData(Vector3[] meshVertices, int[] meshTriangles, Vector3[] meshUVs, Color32[] meshColors)
	{
		//Mesh Filter & Mesh
		Mesh mesh;
		(mesh = _c.MeshFilter.mesh).Clear();
		if (meshVertices.Length > MESH_VERTICES_QUANTITY_LIMIT)
			mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = meshVertices;
		mesh.triangles = meshTriangles;
		mesh.SetUVs(0, meshUVs);
		mesh.colors32 = meshColors;
		mesh.RecalculateNormals();
		//Mesh Renderer
		_c.MeshRenderer.material = World.I.WorldMaterial;
		//Mesh Collider
		_c.MeshCollider.sharedMesh = mesh;
		_c.MeshCollider.material = World.I.ChunkPhysicMaterial;
	}

}
#endif