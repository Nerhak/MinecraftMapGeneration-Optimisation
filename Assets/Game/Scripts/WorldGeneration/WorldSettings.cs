using UnityEditor;
using UnityEngine;

public static class WorldSettings
{
	//World
	public const byte WORLD_NEGATIVE_Y_LIMIT = 0;//Must be changed to -10k
	//World size in chunk quantity
	public const int WORLD_SIZE = 312; //Because of Unity floating precision limit
	public const int WORLD_SIZE_SQUARED = 97344;
	//Chunk size in block quantity
	public const int CHUNK_SIZE = 32;
	public const int CHUNK_SIZE_SQUARED = 1024;
	public const int CHUNK_SIZE_CUBED = 32768;
	public const int CHUNK_SIZE_MINUS_ONE = 31;
	//Chunk Generation States
	public const byte GENERATE = 0;
	public const byte GENERATED = 1;
	public const byte KEEP = 2;
	//World initial generation
	public const int INITIAL_WORLD_DIMENSIONS_IN_CHUNKS = 5;
	//World Axis
	public const int X_AXIS = 0;
	public const int Y_AXIS = 1;
	public const int Z_AXIS = 2;
	//
	public const bool NEGATIVE_FACE = true;
	public const bool POSITIVE_FACE = false;
	//
	public const byte CHUNK_GENERATION_RADIUS = 3;
	//
	public static string WORLD_SAVING_DIRECTORY = Application.persistentDataPath + "/SaveData/";
	public static string CHUNKS_DIRECTORY_PATH = Application.persistentDataPath + "/SaveData/Chunks/";
	//
	public const ushort MESH_VERTICES_QUANTITY_LIMIT = 65535;
	//
	public const float MIN_GLOBAL_LIGHT_LEVEL = 0.01f;
	public const float MAX_GLOBAL_LIGHT_LEVEL = 1f;

	//
}

//(x + y * ChunkSizeX + z * ChunkSizeX * ChunkSizeY)
//x = arrayIndex % ChunkSizeX;
//y = arrayIndex / ChunkSizeX % ChunkSizeY;
//z = arrayIndex / ChunkSizeX * ChunkSizeY;