using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class Map :MonoBehaviour {
	public static Map map;

	public delegate void HexUnderEdit(Hex h);
	public static event HexUnderEdit OnEditing;
	public delegate void HexClearedFromEdit(Hex h);
	public static event HexClearedFromEdit OnNoLongerEditing;
	public delegate void NetworkAddHex(AxialCoord a);
	public static event NetworkAddHex OnAfterAddHex;
	public delegate void SceneHexAddRequest(AxialCoord a, int elevation, int textureIndex);
	public static event SceneHexAddRequest OnBeforeAddHex;
	public delegate void SceneHexSetRequest(AxialCoord a, string spriteName);
	public static event SceneHexSetRequest OnBeforeSetHex;
	//public delegate void SceneHexSetReturn(AxialCoord a, PhotonPlayer p);
	//public static event SceneHexSetReturn OnAfterSetHex;


	public GameObject MapHexObject;
	public GameObject ClipBoardCamera;
	public GameObject PreviewCamera;
	public List<Hex> hexModels;
	public List<Vertex> verteces;
	public List<Hex> editableHexes;
	public List<Hex> hexesUnderEdit;
	public Dictionary<AxialCoord, Hex> clipboard;
	public List<Hex> hexClipBoard;
	public List<Hex> spawnPreview;
	List<SaveableHex> saveHexes;
	List<AxialCoord> findBuffer;
	List<AxialCoord> editBuffer;
	Queue<AxialCoord> editQueue;
	bool parsingEditBuffer;
	bool parsingFindBuffer;
	string filePath;
	bool loadComplete;
	List<List<SaveableHex>> UndoLists;
	public bool editable;
	public int hexCount;
	void Awake()
	{
		if (map == null) {
			DontDestroyOnLoad(gameObject);
			map=this;
			verteces = new List<Vertex> ();
			saveHexes = new List<SaveableHex> ();
			hexModels = new List<Hex> ();
			editableHexes = new List<Hex> ();
			hexesUnderEdit = new List<Hex> ();
			//MapSpawnManager.mapSpawner.SpawnRandomHelicalAtAxial (AxialCoord.origin, 3);
			editable = true;
			findBuffer = new List<AxialCoord>();
			editBuffer = new List<AxialCoord>();
			editQueue = new Queue<AxialCoord>();
			clipboard = new Dictionary<AxialCoord, Hex>();
			UndoLists = new List<List<SaveableHex>>(5);
		} else if(map!=this) {
			Destroy(gameObject);
		}
		filePath = Path.Combine (Application.streamingAssetsPath, "PAXEast2015.dat");
		loadComplete = false;
	}
	void Start()
	{

	}
	void Update()
	{

	}
	#region add functions
	public Hex AddHex(AxialCoord a, int elevation = 1, int textureIndex=-1, string textureString ="", int rotations = 0, Outline o = Outline.none)
	{
			GameObject g = 
				Instantiate (MapHexObject,
		                         AxialCoord.FlatWorldPositionFromAxial (a),
			            Quaternion.identity) as GameObject;
		Hex newHex = g.GetComponent<Hex> ();
			newHex.SetPosition (a);
			newHex.SetElevation (elevation);
			newHex.SetNamedFromHextures(textureString);
			newHex.SetRotations(rotations);
			hexModels.Add (newHex);
			AddNecessaryVerteces (newHex);
		newHex.HasOutline = o;
			if (OnAfterAddHex != null)
				OnAfterAddHex (a);
		hexCount++;
			return newHex;
		//} else {
		//	if(OnBeforeAddHex!=null)
		//		OnBeforeAddHex(a, elevation,textureIndex);
		//	return null;
		//}
	}
	void AddNecessaryVerteces(Hex h)
	{
		Vector3 center = h.GetWorldPosition(); 
		for (int i =0; i<6; i++) {
			int angle_deg = (60 * i);
			float angle_rad = Mathf.PI / 180 * angle_deg;
			Vector3 vPos = new Vector3 (center.x + 1 * Mathf.Cos (angle_rad),
			                            center.y + 1 * Mathf.Sin (angle_rad));
			Vertex v = VertexExistsAt(vPos);
			if(v==null){
				v= new Vertex(vPos);
				verteces.Add(v);
			}
			v.AddNewNeighbor(h);

		}
	}
	
	public void AddVertex (Vertex v)
	{
		verteces.Add (v);
	}
	public void ToggleOutline(AxialCoord a)
	{
		Hex h = HexModelExistsAt (a);
		if (h == null)
			h = AddHex (a);
		Outline o = h.ToggleOutline ();
	}
	#endregion
	#region editing Functions
	public Hex EditHex(AxialCoord position, bool fromBuffer = false, int rotations=0, bool createIfNotExist = true)
	{
		Hex hexToEdit;
		//Does the hex exist?
		hexToEdit = HexModelExistsAt (position);
		if (hexToEdit == null && createIfNotExist) {

			if(!fromBuffer)
			hexToEdit = AddHex (position, 1,-1,"",rotations);
			if (hexToEdit == null){
				if(!fromBuffer)
				editQueue.Enqueue(position);
			return null;
			}
			else{
			//hexToEdit.ToggleEditState();
				if(!hexToEdit.IsUnderEdit()){
					hexesUnderEdit.Add(hexToEdit);
				hexToEdit.SetUnderEdit(true);
				}
			if(OnEditing!=null)
			OnEditing(hexToEdit);
			return hexToEdit;
			}
		} else if(!hexToEdit.IsUnderEdit())
			{
				hexesUnderEdit.Add(hexToEdit);
					hexToEdit.SetUnderEdit(true);
			if(OnEditing!=null)
				OnEditing(hexToEdit);
				return hexToEdit;
		}
		return null;
	}
	public void SetOrCreateHex(AxialCoord a, string path, int rotation = 0)
	{
		Hex h = EditHex (a, false,rotation);
		if(h==null)
			h = HexModelExistsAt(a);
		h.SetNamedFromHextures(path);
	}

	public void OutlinesForSelected(Outline o)
	{
		foreach (Hex h in hexesUnderEdit) {
			h.HasOutline = o;
		}
	}
	public void ToggleEditing(AxialCoord a)
	{
		if (!ReleaseFromEdit(a)) {
			EditHex(a);
		}
	}
	public bool ReleaseFromEdit(AxialCoord a)
	{
		if (hexesUnderEdit.Exists (h => h.position == a)) {
			Hex toggleHex = hexesUnderEdit.Find (h => h.position == a);
			toggleHex.SetUnderEdit(false);
			DeleteOthersAt(toggleHex);
			if(OnNoLongerEditing!=null)
			OnNoLongerEditing(toggleHex);
			hexesUnderEdit.Remove(toggleHex);
			return true;
		} 
		return false;
	}
	public void MoveHexes(AxialCoord offset)
	{
		foreach (Hex h in hexesUnderEdit) {
			h.SetPosition (h.GetPosition ()+offset);
		}
	}
	public void SetHexElevationsTo(int elevation)
	{
		foreach (Hex h in hexesUnderEdit) {
			h.SetElevation(elevation);
		}
	}
	public void IncreaseHexElevations()
	{
		foreach (Hex h in hexesUnderEdit) {
			if(h.GetElevation()<4)
			h.SetElevation(h.GetElevation()+1);
		}
	}
	public void DecreaseHexElevations()
	{
		foreach (Hex h in hexesUnderEdit) {
			if(h.GetElevation()>1)
				h.SetElevation(h.GetElevation()-1);
		}
	}
	public void FinalizeChanges()//List<Hex> doneEditing)
	{
		List<Hex> templist = new List<Hex> ();
		foreach (Hex h in hexesUnderEdit) {
			if(!templist.Contains(h)){
			h.SetUnderEdit(false);
			templist.AddRange(DeleteOthersAt(h));
			if(OnNoLongerEditing != null)
			OnNoLongerEditing(h);
			}
		}
		hexesUnderEdit.Clear ();
		DeleteHexes (templist);
	}
	List<Hex> DeleteOthersAt(Hex priorityHex)
	{
		List<Hex> tempList = new List<Hex> ();
		AxialCoord a = priorityHex.position;
		List<Hex> allAtPosition = hexModels.FindAll (h => h.position == a);
		foreach (Hex h in allAtPosition) {
			if(h!= priorityHex)
				tempList.Add (h);
		}
		return tempList;
	}
	public void SelectAll()
	{
		hexesUnderEdit.Clear ();
		hexesUnderEdit.AddRange (hexModels);
		foreach (Hex h in hexModels) {
			if(!h.IsUnderEdit()){
				if(OnEditing!=null)
					OnEditing(h);
				h.SetUnderEdit(true);
			}
		}
	}
	public void CenterMap()
	{
		int maxQ = 0;
		int maxR = 0;
		int minQ = 0;
		int minR = 0;
		foreach (Hex h in hexModels) {
			maxQ = Mathf.Max(maxQ, h.position.q);
			maxR = Mathf.Max(maxR, h.position.r);
			minQ = Mathf.Min(minQ, h.position.q);
			minR = Mathf.Min(minR, h.position.r);
		}
		int delQ = ((maxQ + minQ) / (-2));
		int delR = ((maxR + minR) / (-2));
		AxialCoord offset = new AxialCoord (delQ, delR);
		foreach (Hex h in hexModels) {
			h.SetPosition(h.position+offset);
		}

	}
	public void DeleteHexes(List<Hex> toBeDeleted)
	{
		foreach (Hex g in toBeDeleted) {
			DeleteHex(g);
		}
	}
	public void DeleteHexes()
	{
		List<Hex> toBeDeleted = new List<Hex> ();
		toBeDeleted.AddRange (hexesUnderEdit);
		foreach (Hex g in toBeDeleted) {
			DeleteHex(g);
		}
	}
	public void CopyHexes(AxialCoord pos)
	{
		//clipboard.Clear ();
		ClearClipBoard ();
		foreach (Hex h in hexesUnderEdit) {
			AxialCoord offset = h.position - pos;
			//clipboard.Add(offset, h);
			CopyHexToClipboard(h,offset);
		}
		if (ClipBoardCamera != null && !ClipBoardCamera.activeSelf)
			ClipBoardCamera.SetActive (true);

	}
	public void CopyHexToClipboard(Hex h, AxialCoord offset)
	{
		GameObject g = //PhotonNetwork.InstantiateSceneObject ("MapHexObject",
			Instantiate (MapHexObject,
			             AxialCoord.SunkenWorldPositionFromAxial (offset),
			             Quaternion.identity) as GameObject;//, 0);//, null);
		Hex newHex = g.GetComponent<Hex> ();
		newHex.SetPosition (offset, true);
		newHex.SetElevation (h.GetElevation());
		newHex.SetNamedFromHextures(h.spriteName);
		newHex.SetRotations(h.rotations);
//		newHex.hasOutline = h.hasOutline;
		hexClipBoard.Add (newHex);
	}
	public void AddHexToPreview(string hextureName, AxialCoord offset)
	{
		GameObject g = //PhotonNetwork.InstantiateSceneObject ("MapHexObject",
			Instantiate (MapHexObject,
			             AxialCoord.PreviewWorldPositionFromAxial (offset),
			             Quaternion.LookRotation(Vector3.back)) as GameObject;//, 0);//, null);
		Hex newHex = g.GetComponent<Hex> ();
		newHex.SetPosition (offset, false, true);
		newHex.SetNamedFromHextures(hextureName);
		//newHex.FlipFace ();
			             spawnPreview.Add (newHex);
	}
	public void ClearPreview()
	{
		if (spawnPreview.Count > 0) {
			foreach (Hex h in spawnPreview) {
				GameObject.Destroy (h.gameObject);
			}
			spawnPreview.Clear ();
			if (PreviewCamera != null && PreviewCamera.activeSelf)
				PreviewCamera.SetActive (false);
		}
	}
	public void ClearClipBoard()
	{
		foreach (Hex h in hexClipBoard) {
			GameObject.Destroy(h.gameObject);
		}
		hexClipBoard.Clear ();
		if (ClipBoardCamera != null && ClipBoardCamera.activeSelf)
			ClipBoardCamera.SetActive (false);
	}
	public void PasteHexes(AxialCoord pos)
	{
		foreach (AxialCoord a in clipboard.Keys) {
			SetOrCreateHex(a+pos, clipboard[a].spriteName, clipboard[a].rotations);
		}

	}
	public void PasteHexes(AxialCoord pos, bool fromClipboard)
	{
		if (fromClipboard) {
			foreach (Hex h in hexClipBoard) {
				SetOrCreateHex (h.position + pos, h.spriteName, h.rotations);
			}
		} else
			PasteHexes (pos);
	}
	public void DeleteHex(Hex toBeDeleted)
	{
		hexModels.Remove(toBeDeleted);
		hexesUnderEdit.Remove (toBeDeleted);
		if(OnNoLongerEditing!=null)
		OnNoLongerEditing(toBeDeleted);
		GameObject.Destroy(toBeDeleted.gameObject);
	}
	public void WipeMapHexes()
	{
		foreach (Hex g in hexModels) {
			GameObject.Destroy(g.gameObject);
		}
		verteces.Clear ();
		hexModels.Clear ();
	}
	public bool IsHexModelUnderEditing(Hex h)
	{
		for(int x=0; x<hexesUnderEdit.Count;x++)  
		if (hexesUnderEdit[x] == h ) {
					return true;;
		}
				return false;
	}
	public Hex HexModelExistsAt(AxialCoord position)
	{
		for(int x=0; x<hexModels.Count;x++)  
		if (hexModels[x].GetPosition() == position ) {
			return hexModels[x];
		}
		return null;
	}
	public Vertex VertexExistsAt(Vector3 worldPosition)
	{
		for (int i =0; i<verteces.Count; i++) {
			if(verteces[i].location == worldPosition){
				return verteces[i];
			}
		}
		return null;//verteces.Find(v => v.location == worldPosition);
	}
	public void SetAllToBeLit(bool SetAllToBeLit){
		foreach (Hex h in hexModels) {
			h.SetUnderEdit(SetAllToBeLit);
		}
	}

	#endregion
	public int GetElevationForPosition(AxialCoord a)
	{
		Hex h = HexModelExistsAt (a);
		if (h != null)
			return h.GetElevation ();
		return 0;
	}
	public void SaveForUndo()
	{
		List<SaveableHex> tempList = new List<SaveableHex> ();
		foreach (Hex h in hexModels) {
			tempList.Add(new SaveableHex(h.GetPosition(), h.spriteName, h.rotations, h.HasOutline));
		}
		if (UndoLists.Count == 5)
			UndoLists.RemoveAt (0);
		UndoLists.Add (tempList);
	}
	public void UndoMapCommand()
	{
		if (UndoLists.Count > 0) {
			WipeMapHexes ();
			if(UndoLists[UndoLists.Count-1].Count>0){
			foreach (SaveableHex h in UndoLists[UndoLists.Count-1]) {
				AddHex (h.position, 0, 0, h.textureString, h.rotations, h.outline);
			}
			}
			UndoLists.RemoveAt(UndoLists.Count-1);
		}

	}
	public AxialCoord RandomHex()
	{
		int randomHex = UnityEngine.Random.Range (0, hexModels.Count);
		return hexModels [randomHex].GetPosition();
	}
	public void setTransparent(bool fullMap)
	{
		if (fullMap) {
			foreach (Hex h in hexModels) {
				h.SetTransparent ();
			}
		} else {
			foreach (Hex h in hexesUnderEdit) {
				h.SetTransparent ();
			}
		}

	}
	public void setBlank(bool fullMap)
	{
		if (fullMap) {
			foreach (Hex h in hexModels) {
				h.SetBlank ();
			}
		} else {
			foreach (Hex h in hexesUnderEdit) {
				h.SetBlank ();
			}
		}
		
	}
	public float TraversalHeight(AxialCoord position)
	{
		for(int x=0; x<hexModels.Count;x++)  
		if (hexModels[x].GetPosition() == position ) {
			return (hexModels[x].GetElevation())*0.75f;
			
		}
		return 0;
	}
	public float TraversalHeight(Hex h)
	{
		return h.GetElevation() * 0.75f;
	}

}
[Serializable]
public class SaveableHex{
	public AxialCoord position;
	public string textureString;
	public int rotations;
	public bool flipped;
	public Outline outline;
	public SaveableHex(AxialCoord p, string t, int r, Outline o= Outline.none, bool isFlipped = false)
	{
		position = p;
		textureString =  t;
		rotations = r;
		outline = o;
		flipped = isFlipped;
	}
	public override string ToString ()
	{
		return string.Format ("{0}; {1}", position.ToString(), outline.ToString());
	}
}
[Serializable]
public class SaveableHex2{
	public AxialCoord position;
	public string textureString;
	public int rotations;
	public bool flipped;
	public Outline outline;
	public SaveableHex2(AxialCoord p, string t, int r, Outline o= Outline.none, bool isFlipped = false, bool c=false, bool h=false)
	{
		position = p;
		textureString =  t;
		rotations = r;
		outline = o;
		flipped = isFlipped;
	}
	public override string ToString ()
	{
		return string.Format ("{0}; {1}", position.ToString(), outline.ToString());
	}
}
