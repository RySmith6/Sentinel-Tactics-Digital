using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CharacterDataController : MonoBehaviour {

	string characterDataFileSuffix = "data.json";
	List<CharacterDataObject> characters = new List<CharacterDataObject>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LoadCharacterData()
	{
		// Path.Combine combines strings into a file path
		// Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
		string filePath = Path.Combine(Application.streamingAssetsPath, characterDataFileSuffix);

		if(File.Exists(filePath))
		{
			// Read the json from the file into a string
			string dataAsJson = File.ReadAllText(filePath);
			Debug.Log (dataAsJson);
			// Pass the json to JsonUtility, and tell it to create a GameData object from it
			CharacterDataObject loadedData = JsonUtility.FromJson<CharacterDataObject>(dataAsJson);
			characters.Add (loadedData);
			// Retrieve the allRoundData property of loadedData
			//allRoundData = loadedData.allRoundData;
		}
		else
		{
			Debug.LogError("Cannot load game data!");
		}
	}
}
