using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CharacterDataEditor : EditorWindow {

	private string CharacterDataProjectPath = "/StreamingAssets/";
	private string CharacterFileNameSuffix = "data.json";
	public string characterName = "Legacy";
	public CharacterDataObject character = new CharacterDataObject();
	Vector2 scrollView= Vector2.zero;

	[MenuItem("Window/Character Data Editor")]
	static void Init()
	{
		CharacterDataEditor window = (CharacterDataEditor)EditorWindow.GetWindow (typeof(CharacterDataEditor));
		window.Show ();
	}

	void OnGUI()
	{
		scrollView = GUILayout.BeginScrollView (scrollView, true, true);
		if (character != null) 
		{
			SerializedObject serializedObject = new SerializedObject (this);
			SerializedProperty serializedProperty = serializedObject.FindProperty ("character");

			EditorGUILayout.PropertyField (serializedProperty, true);

			serializedObject.ApplyModifiedProperties ();

			if (GUILayout.Button ("Save data")) {
				SaveGameData();
			}
		}
		if (GUILayout.Button ("Load Data")) {
			LoadGameData ();
		}
		GUILayout.EndScrollView ();
	}

	private void LoadGameData(){
		string filePath = Application.dataPath + CharacterDataProjectPath + (character.characterName ?? "Legacy") + CharacterFileNameSuffix;

		if (File.Exists (filePath)) {
			// Read the json from the file into a string
			string dataAsJson = File.ReadAllText(filePath); 
			// Pass the json to JsonUtility, and tell it to create a GameData object from it
			character = JsonUtility.FromJson<CharacterDataObject>(dataAsJson);

		}
		else
		{
			Debug.LogError("Cannot load game data!");
		}
	}

	private void SaveGameData(){
		string dataAsJson = JsonUtility.ToJson (character);
		string filePath = Application.dataPath + CharacterDataProjectPath + (character.characterName ?? "Legacy") + CharacterFileNameSuffix;	
		File.WriteAllText (filePath, dataAsJson);
	}

}
