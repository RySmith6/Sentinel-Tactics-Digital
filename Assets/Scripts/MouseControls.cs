using UnityEngine;
//using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class MouseControls : MonoBehaviour {
	public static MouseControls mouse;
	public AxialCoord mousePosition;
	RaycastHit hit;
	EventSystem es;
	public bool active=false;
	public static Rect screenRect = new Rect (0,0,Screen.width, Screen.height);
	public static int Xmid = Screen.width / 2;
	public static int Ymid = Screen.height / 2;
	SpriteRenderer tokenRenderer;
	public static bool lockToMap= false;
	public AxialCoord onMapPosition;
	public Vector3 worldPointer;
	// Use this for initialization
	void Start () {
		es = GameObject.Find ("EventSystem").GetComponent<EventSystem> ();
		tokenRenderer = GetComponent<SpriteRenderer> ();
		mouse = this;
		worldPointer = new Vector3 ();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!es.IsPointerOverGameObject () && !ManageInputs.showMenu && screenRect.Contains (Input.mousePosition)) {
			Vector3 mousepos = Input.mousePosition;
			mousepos.z = Camera.allCameras [0].transform.position.z;
			worldPointer = Camera.allCameras [0].ScreenToWorldPoint (mousepos);
			mousePosition = AxialCoord.AxialFromWorld (worldPointer);
			if(onMapPosition!=mousePosition && (Map.map.HexModelExistsAt(mousePosition) || !lockToMap))
			{
			this.transform.position = AxialCoord.WorldPositionFromAxial (mousePosition);
				onMapPosition=mousePosition;
				active=true;
			}
			else if(active)
				active=false;

		}
		else if(active)
			active=false;

	}

	public void ToggleInvisible()
	{
		if (tokenRenderer != null) {
			tokenRenderer.enabled = ! tokenRenderer.enabled;
		}
	}
	public void ToggleInvisible(bool visible)
	{
		if (tokenRenderer != null) {
			tokenRenderer.enabled = visible;
		}
	}
	public void SetBlueMat()
	{
		tokenRenderer.color = Color.blue;
	}
	public void SetStandardMat()
	{
		tokenRenderer.color = Color.red;
	}
}
