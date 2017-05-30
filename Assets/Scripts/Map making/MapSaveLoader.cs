using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MapSaveLoader : MonoBehaviour
{
	public static MapSaveLoader mapSaveLoader;
	static List<Hex> hexModels;
	static List<SaveableHex> saveHexes;
	static Map gameMap;
	// Use this for initialization
	void Awake ()
	{
		if (mapSaveLoader == null) {
			DontDestroyOnLoad (gameObject);
			mapSaveLoader = this;

			saveHexes = new List<SaveableHex> ();
			gameMap = Map.map;
		}else if(mapSaveLoader!=this) {
			Destroy(gameObject);
		}
	}
	void Start()
	{
		//hexModels = Map.map.hexModels;
	}
	public void SaveToFile (string filename)//List<Hex> toBeSaved)
	{
		saveHexes.Clear ();
		foreach (Hex h in Map.map.hexModels) {
			saveHexes.Add(new SaveableHex(h.GetPosition(), h.spriteName, h.rotations, h.HasOutline, h.flipped));
		}
		//System.IO.File.re
		FileStream file = File.Open (Path.Combine( Application.persistentDataPath, filename)  , FileMode.OpenOrCreate);
		BinaryFormatter bf = new BinaryFormatter ();
		bf.Serialize (file, saveHexes);
		file.Close ();
		StartCoroutine ("SaveMapImage", filename);
	}
	IEnumerator SaveMapImage(string filename)
	{
		// We should only read the screen buffer after rendering is complete
		yield return new WaitForEndOfFrame();
		
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		
		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		
		// Encode texture into PNG
		byte[] bytes = tex.EncodeToJPG ();
		Destroy(tex);

		#if UNITY_STANDALONE_WIN

		// For testing purposes, also write to a file in the project folder
		File.WriteAllBytes(Application.persistentDataPath + "/"+filename +".jpg", bytes);
		Application.CaptureScreenshot (Application.persistentDataPath + "/" + filename + ".png", 4);
#endif
	}
	public void LoadFromFile(string fileName, bool coroutine)
	{
		if (coroutine)
			StartCoroutine ("LoadFromFileCoroutine", fileName);
		else
			LoadFromFile (fileName);
	}
	public void LoadFromFile(string fileName)
	{
		if (gameMap == null)
			gameMap = Map.map;
		gameMap.WipeMapHexes ();
		saveHexes.Clear ();
		FileStream file = File.Open (Path.Combine( Application.persistentDataPath , fileName), FileMode.Open);
		if (file != null) {
			BinaryFormatter bf = new BinaryFormatter ();
			saveHexes = (List<SaveableHex>)bf.Deserialize (file);
			foreach (SaveableHex h in saveHexes) {
				gameMap.AddHex (h.position, 0, 0, h.textureString, h.rotations, h.outline); //, h.elevation, h.textureIndex);
			}
		}
	}
	IEnumerator LoadFromFileCoroutine(string fileName)
	{
		yield return new WaitForEndOfFrame ();
		if (gameMap == null)
			gameMap = Map.map;
		gameMap.WipeMapHexes ();
		saveHexes.Clear ();
		
		FileStream file = File.Open (Path.Combine( Application.persistentDataPath , fileName), FileMode.Open);
		if (file != null) {
			BinaryFormatter bf = new BinaryFormatter ();
			saveHexes = (List<SaveableHex>)bf.Deserialize (file);
			foreach (SaveableHex h in saveHexes) {
				gameMap.AddHex (h.position, 0, 0, h.textureString, h.rotations, h.outline); //, h.elevation, h.textureIndex);
			}
		}
	}
	public void LoadPresetFromFile(string s)
	{
		if (gameMap == null)
			gameMap = Map.map;
		gameMap.WipeMapHexes ();
		saveHexes.Clear ();
		string filePath = Path.Combine (Application.streamingAssetsPath , s);
		BinaryFormatter bf = new BinaryFormatter ();
		Debug.Log (filePath);
		if (filePath.Contains ("://")) {
			StartCoroutine("LoadPresetFromNetwork",s);
		} else {
			saveHexes = (List<SaveableHex>)bf.Deserialize (File.Open(filePath, FileMode.Open));
		}
		foreach (SaveableHex h in saveHexes) {
			gameMap.AddHex (h.position, 0, 0, h.textureString.Replace("M","").Replace("; ","_").Replace("Tile","M").Replace("IPM","IP"), h.rotations, h.outline);
		}
	}
	IEnumerator LoadPresetFromNetwork(string s)
	{
		if (gameMap == null)
			gameMap = Map.map;
		gameMap.WipeMapHexes ();
		saveHexes.Clear ();
		string filePath = "https://dl.dropboxusercontent.com/u/22300314/StreamingAssets/" + s;
		BinaryFormatter bf = new BinaryFormatter ();
		WWW www = new WWW (filePath);
		yield return www;
		MemoryStream m;
		m = new MemoryStream (www.bytes);
		saveHexes = (List<SaveableHex>)bf.Deserialize(m);
		foreach (SaveableHex h in saveHexes) {
			gameMap.AddHex (h.position, 0, 0, h.textureString, h.rotations, h.outline);
		}
	}
}

