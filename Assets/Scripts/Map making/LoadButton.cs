using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadButton : MonoBehaviour
{
	public Button button;
	public Text filename;

	public void LoadFile()
	{
		MapSaveLoader.mapSaveLoader.LoadFromFile (filename.text + ".dat", true);
	}
	public void LoadMapName()
	{
		GameSettingsObject.gso.SetMapName (filename.text);
		//GameSettingsObject.gso.mapName = filename.text;
	}
}

