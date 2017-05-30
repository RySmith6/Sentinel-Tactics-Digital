using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveButton : MonoBehaviour {

	public Text fileNameText;
	public MapMakingController mmc;

	// Use this for initialization
	public void SaveFilename()
	{
#if UNITY_STANDALONE_WIN
		MapSaveLoader.mapSaveLoader.SaveToFile (fileNameText.text + ".dat");
		StartCoroutine("SaveMapImage");
#endif
	}
	IEnumerator SaveMapImage()
	{
		yield return new WaitForSeconds (1);
		Application.CaptureScreenshot (fileNameText.text + ".png", 4);
		MapMakingController.mmc.SetUIVisiblity (true);
	}
}
