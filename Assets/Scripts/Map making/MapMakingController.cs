using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapMakingController : MonoBehaviour {
	public static MapMakingController mmc;
	MouseControls mouse;
	float nextAvailable;
	float limitActionsBySeconds;
	EventSystem es;
	bool moving;
	AxialCoord moveOrigin;
	public GameObject spawnMenu;
	public GameObject menuRibbon;
	public GameObject testButton;
	public GameObject previewCamera;
	AxialCoord cachedMousePos;
	bool painting=false;
	int spawnIPOnClick = 0;
	int spawnMOnClick = 0;
	// Use this for initialization
	void Start () {
		mouse = GetComponent<MouseControls> ();
		limitActionsBySeconds = 0.5f;
		nextAvailable = 0.0f;
		es = GameObject.Find ("EventSystem").GetComponent<EventSystem> ();
		moving = false;
		MouseControls.lockToMap = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		#region MouseControls
		if (!es.IsPointerOverGameObject ()&& !ManageInputs.showMenu && MouseControls.screenRect.Contains(Input.mousePosition)) {
			if (Input.GetButtonDown("Primary Action"))//||  Input.GetMouseButton(0))
			{
				if(spawnMOnClick >0)
				{
					SpawnMegalopolisTile(spawnMOnClick);
					spawnMOnClick = 0;
				}
				else if(spawnIPOnClick >0)
				{
					SpawnInsulaPrimalisTile(spawnIPOnClick);
					spawnIPOnClick = 0;
				}
				else
				{
				if(moving)
					moving = false;
				else if(Input.GetButton("Modifier 1"))//|| Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))
				{
					Map.map.SaveForUndo();
					Map.map.ToggleEditing(mouse.mousePosition);
					cachedMousePos=mouse.mousePosition;
					painting=true;
				}
				else
				{
					painting = true;
					Map.map.SaveForUndo();
					Map.map.ToggleEditing (mouse.mousePosition);
					cachedMousePos=mouse.mousePosition;
				}
				}

			}
			else if (Input.GetButton("Primary Action"))//||  Input.GetMouseButton(0))
			{
				if(cachedMousePos!=mouse.mousePosition && painting)
				{
				if(Input.GetButton("Modifier 1"))//|| Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))
					Map.map.ToggleEditing(mouse.mousePosition);
				else if(!Input.GetButton("Modifier 1"))
				{
					Map.map.ToggleEditing (mouse.mousePosition);
				}
					cachedMousePos=mouse.mousePosition;
				}
			}
			else if (Input.GetButtonDown("Secondary Action"))//||Input.GetMouseButtonDown(1))
			{
				if(Input.GetButton("Modifier 1"))
				{
					ShowSpawnMenu();
				}
				else{
				menuRibbon.SetActive(true);
				ManageInputs.showMenu = true;
				}
				
			} else if (Input.GetButtonDown("Tertiary Action")) {	
			moving = !moving;
				if(moving)
					Map.map.SaveForUndo();
			moveOrigin = mouse.mousePosition;
			} 
			if(moving)
			{
				Map.map.MoveHexes(mouse.mousePosition- moveOrigin);
				moveOrigin=mouse.mousePosition;
			}

			if (Input.GetAxis ("Mouse ScrollWheel") > 0){
				if (Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))
					Map.map.IncreaseHexElevations();
			else
				Camera.allCameras[0].orthographicSize = Mathf.Max (Camera.allCameras[0].orthographicSize - 0.5f, 5);
			}
			else if(Input.GetAxis("Mouse ScrollWheel")<0)
			{
				if (Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))
					Map.map.DecreaseHexElevations();
			else
				Camera.allCameras[0].orthographicSize = Mathf.Min (Camera.allCameras[0].orthographicSize + 0.5f, 20);
			}
			
//			else if (Input.GetButton("Fire1")){
//				fCam.LookAround(-Input.GetAxis ("Mouse X"),-Input.GetAxis ("Mouse Y"));
//			}

		}
		else if(!es.IsPointerOverGameObject ()&& ManageInputs.showMenu && MouseControls.screenRect.Contains(Input.mousePosition))
		{
			if(Input.GetButton("Primary Action"))
			{
			CancelContext();
			CancelSpawn();
			}
		}

		if(Input.GetButtonUp("Primary Action"))
		{
			painting=false;
		}
		#endregion
	

		#region keyboardControls
		if ((!ManageInputs.showMenu)&&(Time.time > nextAvailable)) {
			if ((Input.GetButtonDown ("Confirm"))){
				FinalizeChanges();
			nextAvailable = Time.time + limitActionsBySeconds;
			}else if (Input.GetButtonDown ("Rotate/Flip")) {
				if(Input.GetButton("Modifier 1"))
					Flip(false);
				else
					Rotate(false);
				nextAvailable = Time.time + limitActionsBySeconds;
			} else if (Input.GetButtonDown ("Hide Cursor")) {
				if (Input.GetButton("Modifier 1")){
					mouse.ToggleInvisible();
					nextAvailable = Time.time + limitActionsBySeconds;
				}
			}else if (Input.GetButtonDown ("Delete Selection")) {
				if (Input.GetButton("Modifier 1"))
					Map.map.ClearClipBoard ();
				else
					Delete();
			nextAvailable = Time.time + limitActionsBySeconds;
			} else if (Input.GetButtonDown("Copy")) {	
				Copy(false);
			nextAvailable = Time.time + limitActionsBySeconds;
			} else if (Input.GetButtonDown("Paste")) {	
				Paste();
			nextAvailable = Time.time + limitActionsBySeconds;
			}else if (Input.GetButtonDown("Undo")) {
				UndoAction();
				nextAvailable = Time.time + limitActionsBySeconds;
			} else if (Input.GetButtonDown ("Select All")) {
				if (Input.GetButton("Modifier 1"))
					Map.map.CenterMap ();
				else
					Map.map.SelectAll ();
			nextAvailable = Time.time + limitActionsBySeconds;
		} else if (Input.GetKey (KeyCode.Alpha1)) {
				if (Input.GetButton("Modifier 1")) 
				SpawnMegalopolisTile(1);
				else if (Input.GetButton("Modifier 2"))
					SpawnInsulaPrimalisTile(1);
			else
				SetElevationsTo(1);
			nextAvailable = Time.time + limitActionsBySeconds;
		} else if (Input.GetKey (KeyCode.Alpha2)) {
				if (Input.GetButton("Modifier 1")) 
				SpawnMegalopolisTile(2);
				else if (Input.GetButton("Modifier 2"))
					SpawnInsulaPrimalisTile(2);
			else
				SetElevationsTo(2);
			nextAvailable = Time.time + limitActionsBySeconds;
		} else if (Input.GetKey (KeyCode.Alpha3)) {
				if (Input.GetButton("Modifier 1"))
					SpawnMegalopolisTile(3);
				else if (Input.GetButton("Modifier 2"))
					SpawnInsulaPrimalisTile(3);
			else
				SetElevationsTo(3);
			nextAvailable = Time.time + limitActionsBySeconds;
		} else if (Input.GetKey (KeyCode.Alpha4)) {
				if (Input.GetButton("Modifier 1"))
					SpawnMegalopolisTile(4);
				else if (Input.GetButton("Modifier 2"))
					SpawnInsulaPrimalisTile(4);
			else
				SetElevationsTo(4);
			nextAvailable = Time.time + limitActionsBySeconds;
		} else if (Input.GetKey (KeyCode.Alpha5)) {
				if (Input.GetButton("Modifier 2"))
					SpawnInsulaPrimalisTile(5);
				else if (Input.GetButton("Modifier 1"))
					SpawnMegalopolisTile(5);
			nextAvailable = Time.time + limitActionsBySeconds;
		} else if (Input.GetKey (KeyCode.Alpha6)) {
				if (Input.GetButton("Modifier 2"))
					SpawnInsulaPrimalisTile(6);
				else if (Input.GetButton("Modifier 1"))
					SpawnMegalopolisTile( 6);	
			nextAvailable = Time.time + limitActionsBySeconds;
		} else if (Input.GetKey (KeyCode.Alpha7)) {
				if (Input.GetButton("Modifier 2"))
					SpawnInsulaPrimalisTile(7);
				else if (Input.GetButton("Modifier 1"))
					SpawnMegalopolisTile( 7);
			nextAvailable = Time.time + limitActionsBySeconds;
		} else if (Input.GetKey (KeyCode.Alpha8)) {
				if (Input.GetButton("Modifier 2"))
					SpawnInsulaPrimalisTile(8);
				else if (Input.GetButton("Modifier 1"))
					SpawnMegalopolisTile(8);
			nextAvailable = Time.time + limitActionsBySeconds;
			}
		}
		#endregion

		//if (Map.map !=null && Map.map.hexModels.Count > 0 && !testButton.activeInHierarchy)
			//testButton.SetActive (true);
		//else if (testButton.activeInHierarchy && Map.map.hexModels.Count == 0)
			//testButton.SetActive (false);


	}
	#region contextFunctions
	public void UndoAction()
	{
		Map.map.UndoMapCommand ();
	}
	public void SetElevationsTo(int i)
	{
		Map.map.SaveForUndo ();
		Map.map.SetHexElevationsTo (i);
	}
	public void Rotate(bool fromCenter = false)
	{
		Map.map.SaveForUndo ();
		MapMath.RotateAboutPosition (fromCenter ? MapMath.FindCenterOfSelection (): mouse.mousePosition);
	}
	public void Flip(bool fromCenter = false)
	{
		Map.map.SaveForUndo ();
		MapMath.FlipAboutPosition (fromCenter ? MapMath.FindCenterOfSelection (): mouse.mousePosition);
	}
	public void Copy(bool fromCenter = false)
	{
		Map.map.CopyHexes(fromCenter ? MapMath.FindCenterOfSelection (): mouse.mousePosition);
	}
	public void Rotate()
	{
		Map.map.SaveForUndo ();
		MapMath.RotateAboutPosition (mouse.mousePosition);
	}
	public void Flip()
	{
		Map.map.SaveForUndo ();
		MapMath.FlipAboutPosition (mouse.mousePosition);
	}
	public void Copy()
	{
		Map.map.CopyHexes(mouse.mousePosition);
	}
	public void Paste()
	{
		Map.map.SaveForUndo ();
		Map.map.PasteHexes(mouse.mousePosition, true);
	}
	public void FinalizeChanges()
	{
		Map.map.FinalizeChanges ();
		moving = false;
	}
	public void Delete()
	{
		Map.map.SaveForUndo ();
		Map.map.DeleteHexes ();
	}
	public void UnlockForMovement(bool fromCenter = false)
	{
		Map.map.SaveForUndo ();
		moving = true;
		moveOrigin = fromCenter ? MapMath.FindCenterOfSelection (): mouse.mousePosition; //mouse.mousePosition;
	 
	}
	public void UnlockForMovement()
	{
		Map.map.SaveForUndo ();
		moving = true;
		moveOrigin = mouse.mousePosition;
		
	}
	public void SelectAll()
	{
		Map.map.SelectAll ();
	}
	public void CenterMap()
	{
		Map.map.CenterMap ();
	}
	public void SetGreenOutline()
	{
		Map.map.OutlinesForSelected (Outline.cover);
	}
	public void SetRedOutline()
	{
		Map.map.OutlinesForSelected (Outline.hazard);
	}
	public void SetBlueOutline()
	{
		Map.map.OutlinesForSelected (Outline.blue);
	}
	public void RemoveOutlines()
	{
		Map.map.OutlinesForSelected (Outline.none);
	}
	public void SetBlank()
	{
		Map.map.SaveForUndo ();
		Map.map.setBlank (false);
	}
	public void SetTransparent()
	{
		Map.map.SaveForUndo ();
		Map.map.setTransparent (false);
	}
	#endregion
	public void SetUIVisiblity(bool visible)
	{
		menuRibbon.SetActive (visible);
		testButton.SetActive (visible);
		mouse.ToggleInvisible (visible);
	}
	public void PreviewMegalopolisTile(int tileNumber)
	{
		Map.map.ClearPreview ();
		string tileHexName = "";
		AxialCoord[] tileCoords = MapMath.PositionsInRadiusFrom (AxialCoord.origin, 2).ToArray();
		foreach (AxialCoord a in tileCoords) {
			tileHexName = "M"+tileNumber.ToString()+"_"+a.ToString();
			
			Map.map.AddHexToPreview(tileHexName,a);
		}
		if (previewCamera != null)
			previewCamera.SetActive (true);
	}
	public void PreviewInsulaPrimalisTile(int tileNumber)
	{
		Map.map.ClearPreview ();
		string tileHexName = "";
		AxialCoord[] tileCoords = MapMath.PositionsInRadiusFrom (AxialCoord.origin, 2).ToArray();
		foreach (AxialCoord a in tileCoords) {
			tileHexName = "IP"+tileNumber.ToString()+"_"+a.ToString();
			
			Map.map.AddHexToPreview(tileHexName,a);
		}
		if (previewCamera != null)
			previewCamera.SetActive (true);
	}
	public void SpawnMegalopolisTileOnNextClick(int tileNumber)
	{
		spawnMOnClick = tileNumber;
	}
	public void SpawnIPTileOnNextClick(int tileNumber)
	{
		spawnIPOnClick = tileNumber;
	}
	public void SpawnMegalopolisTile(int tileNumber)
	{
		Map.map.SaveForUndo ();
		MapSpawnManager.mapSpawner.SpawnMegalopolisTileAtAxial (mouse.mousePosition, tileNumber);
		spawnMenu.SetActive (false);
		ManageInputs.showMenu = false;
		Map.map.ClearPreview ();
		Map.map.FinalizeChanges ();
	}
	public void SpawnInsulaPrimalisTile(int tileNumber)
	{
		Map.map.SaveForUndo ();
		MapSpawnManager.mapSpawner.SpawnIPTileAtAxial (mouse.mousePosition, tileNumber);
		spawnMenu.SetActive (false);
		ManageInputs.showMenu = false;
		Map.map.ClearPreview ();
		Map.map.FinalizeChanges ();
	}
	public void CancelSpawn()
	{
		spawnMenu.SetActive (false);
		ManageInputs.showMenu = false;
		Map.map.ClearPreview ();
	}
	public void CancelContext()
	{
		menuRibbon.SetActive (false);
		ManageInputs.showMenu = false;
	}
	public void ShowSpawnMenu()
	{
		spawnMenu.SetActive(true);
		ManageInputs.showMenu = true;
		
	}

	public void Quit()
	{
		Application.Quit ();
	}
	public void BeginTest()
	{
		SceneManager.LoadScene("TargetChooser");
	}
}
