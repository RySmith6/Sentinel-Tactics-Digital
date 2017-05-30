using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoadManager : MonoBehaviour
{
	public Transform contentPanel;
	public GameObject loadButton;
	// Use this for initialization
	void Start ()
	{
		#if UNITY_STANDALONE_WIN

		PopulateList ();
#endif
	}

	void OnEnable()
	{
		//PopulateList ();
	}
	
	void PopulateList()
	{
		#if UNITY_STANDALONE_WIN
		DirectoryInfo di = new DirectoryInfo (Application.persistentDataPath); 
		FileInfo[] files = di.GetFiles("*.dat");
		foreach (FileInfo f in files) {
			GameObject newButton = Instantiate (loadButton) as GameObject;
			LoadButton button = newButton.GetComponent<LoadButton>();
			button.filename.text = f.Name.Remove(f.Name.LastIndexOf("."));
			//button.button.onClick +=
			newButton.transform.SetParent(contentPanel);
		}
#endif
	}

	void PopulateList(bool totalList)
	{
		PopulateList ();
		if (totalList) {
			DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/StreamingAssets");
			FileInfo[] files = di.GetFiles("*.dat");
			foreach (FileInfo f in files) {
				GameObject newButton = Instantiate (loadButton) as GameObject;
				LoadButton button = newButton.GetComponent<LoadButton>();
				button.filename.text = f.Name.Remove(f.Name.LastIndexOf("."));
				//button.button.onClick +=
				newButton.transform.SetParent(contentPanel);
			}
		}
	}
	void PopulateMenuList(bool totalList)
	{
		DirectoryInfo di = new DirectoryInfo (Application.persistentDataPath); 
		FileInfo[] files = di.GetFiles("*.dat");
		foreach (FileInfo f in files) {
			GameObject newButton = Instantiate (loadButton) as GameObject;
			LoadButton button = newButton.GetComponent<LoadButton>();
			button.filename.text = f.Name.Remove(f.Name.LastIndexOf("."));
			//button.button.onClick +=
			newButton.transform.SetParent(contentPanel);
		}
		if (totalList) {
			di = new DirectoryInfo(Application.dataPath + "/StreamingAssets");
			files = di.GetFiles("*.dat");
			foreach (FileInfo f in files) {
				GameObject newButton = Instantiate (loadButton) as GameObject;
				LoadButton button = newButton.GetComponent<LoadButton>();
				button.filename.text = f.Name.Remove(f.Name.LastIndexOf("."));
				//button.button.onClick +=
				newButton.transform.SetParent(contentPanel);
			}
		}
	}
	public void LoadFileName(GameObject item)
	{

	}
}

