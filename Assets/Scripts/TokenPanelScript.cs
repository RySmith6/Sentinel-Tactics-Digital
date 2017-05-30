using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TokenPanelScript : MonoBehaviour {
	public Text LOSText;
	public Text positionText;
	public Text elevationText;
	public Text healthText;


	GameObject statPanel;
	// Use this for initialization
	void Awake () {
		//panelText = GetComponentInChildren<Text> ();

	}
	void Start()
	{

	}
	// Update is called once per frame
	void Update () {
	
	}
	public void UpdatePosition(AxialCoord a)
	{
		positionText.text = "Position: (" + a.ToString () + ")";
	}
	public void UpdateElevation(int elevation)
	{
		elevationText.text = "Elevation: " + elevation.ToString ();
	}
	public void UpdateHealth(int health)
	{
		healthText.text = "Health: " + health.ToString ();
	}
	public void UpdateLOS(List<string> targetsInLOS)
	{
		LOSText.text = "LOS to: " + string.Join (", ", targetsInLOS.ToArray());
	}

}
