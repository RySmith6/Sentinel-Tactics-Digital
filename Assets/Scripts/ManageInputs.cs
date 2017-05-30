using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ManageInputs : MonoBehaviour {
	public static ManageInputs inputManager;

	[SerializeField] GameObject saveloadPanel;
	[SerializeField] GameObject controlPanel;

	public static bool showMenu;
	//public static void
	// Use this for initialization
	void Start () {

		if (inputManager == null) {
			DontDestroyOnLoad(gameObject);
			inputManager=this;
		} else if(inputManager!=this) {
			Destroy(gameObject);
		}
		showMenu = false;
		//saveloadPanel = GameObject.FindWithTag ("SaveLoad");

		
	}
	
	// Update is called once per frame
	void Update () {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		if (Input.GetButtonDown("Menu"))
		{
			showMenu = !showMenu;
			if(controlPanel.activeSelf)
				controlPanel.SetActive (false);
			saveloadPanel.SetActive(showMenu);
		}


	}
	public void CloseMenu()
	{
		showMenu = false;
		//saveloadPanel.SetActive (false);
	}
	public void OpenMenu()
	{
		showMenu = true;
	}
	public void ShowControls()
	{
		saveloadPanel.SetActive (false);
		showMenu = true;
		controlPanel.SetActive (true);
	}


}
