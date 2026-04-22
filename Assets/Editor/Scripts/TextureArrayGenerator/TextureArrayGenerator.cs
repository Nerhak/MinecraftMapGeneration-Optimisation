using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class TextureArrayGenerator : EditorWindow
{
	private Texture2DArray _texture2DArray;
	private static int _blocksSizeInPixels = 16;

	private readonly List<Texture2D> _filteredTextures = new List<Texture2D>();

	//Variables
	private const string _menuItemName = "Legacy/Texture Array Generator";
	private const string _toolName = "Texture Array Generator";
	private const string _toolNameBarksPrefix = "TextureArrayGenerator: ";
	private const string _worldRawTexturesPath = "Textures/RawBlockTextures/";
	private const string _crackingRawTexturesPath = "Textures/Cracking/";
	private const string _worldTexture2DArraySavePath = "Assets/Game/Textures/Texture2DArray/WorldTexture2DArray.asset";
	private const string _crackingTexture2DArraySavePath = "Assets/Game/Textures/Texture2DArray/CrackingTexture2DArray.asset";

	[MenuItem(_menuItemName)]

	public static void ShowWindow()
	{
		GetWindow(typeof(TextureArrayGenerator));
	}

	private void OnGUI()
	{
		GUILayout.Label(_toolName, EditorStyles.boldLabel);

		if (GUILayout.Button("Generate World Texture Array"))
		{
			Debug.Log($"{_toolNameBarksPrefix}Generating World Texture Array.");
			ConvertRawTexturesIntoFilteredTextureList(_worldRawTexturesPath);
			GenerateTexture2DArray();
			PopulateTexture2DArray();
			SaveTexture2DArray(_worldTexture2DArraySavePath);
		}

		if (GUILayout.Button("Generate Cracking Texture Array"))
		{
			Debug.Log($"{_toolNameBarksPrefix}Generating Cracking Texture Array.");
			ConvertRawTexturesIntoFilteredTextureList(_crackingRawTexturesPath);
			GenerateTexture2DArray();
			PopulateTexture2DArray();
			SaveTexture2DArray(_crackingTexture2DArraySavePath);
		}
	}

	private void ConvertRawTexturesIntoFilteredTextureList(string rawTexturePath)
	{
		_filteredTextures.Clear();

		Object[] rawTextures = Resources.LoadAll(rawTexturePath, typeof(Texture2D));

		foreach (Object rawTexture in rawTextures)
		{
			Texture2D texture = (Texture2D)rawTexture;
			if (texture.width == _blocksSizeInPixels && texture.height == _blocksSizeInPixels)
				_filteredTextures.Add(texture);
			else
				Debug.Log($"{_toolNameBarksPrefix}{rawTexture.name} incorrect size. Texture not loaded.");
		}
		Debug.Log($"{_toolNameBarksPrefix}{_filteredTextures.Count} successfully loaded.");
	}

	private void GenerateTexture2DArray()
	{
		_texture2DArray = new Texture2DArray(_filteredTextures[0].width,
												_filteredTextures[0].height,
												_filteredTextures.Count,
												TextureFormat.RGBA32,
												true,
												false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Repeat
		};
	}

	private void PopulateTexture2DArray()
	{
		for (int i = 0; i < _filteredTextures.Count; i++)
		{
			_texture2DArray.SetPixels(_filteredTextures[i].GetPixels(0), i, 0);
		}
		_texture2DArray.Apply();
	}

	private void SaveTexture2DArray(string texture2DArraySavePath)
	{
		AssetDatabase.CreateAsset(_texture2DArray, texture2DArraySavePath);
	}
}