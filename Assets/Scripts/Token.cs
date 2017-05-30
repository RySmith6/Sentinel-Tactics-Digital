using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//namespace LoreForge.Tactics.Controllers
//{
	public class Token: MonoBehaviour
	{
	public delegate void BeforeMove();
	public event BeforeMove OnBeforeMove;
	public delegate void AfterMove();
	public event AfterMove OnAfterMove;
	public delegate void EditPosition(AxialCoord a);
	public event EditPosition OnTakeResponsibility;
	public int turnOrder;
	public string CharacterName;
	public AxialCoord position;
	public Texture[] characterTextures;
	Camera followCamera;
	Transform fcTransform;
	Canvas statPanelCanvas;
	Vector3 followCameraHeight;// = {0, 0, 1000};
	float followCameraSmooth = 1.5f;
	Vector3 trailPosition;
	TokenPanelScript panelScript;
	List<Hex> editResponsibility;

	public bool flatTop { get; set; }
		void Awake()
	{
		Object[] objs =  Resources.LoadAll ("Textures/CharacterTokens");
		characterTextures = new Texture[objs.Length];
		for(int i = 0; i<objs.Length; i++)
		{
			if(objs[i] is Texture)
			{
				characterTextures[i] =(Texture) objs[i];
			}
		}
		panelScript = GetComponentInChildren<TokenPanelScript> ();
		flatTop = true;
		editResponsibility = new List<Hex> ();
	}
	void Start()
	{
		statPanelCanvas = GetComponentInChildren<Canvas> ();
	}

	void Update()
	{
		//fcTransform.position = trailPosition = Vector3.Lerp (fcTransform.position, followCameraHeight+transform.position, Time.deltaTime * followCameraSmooth);
	}

	public void SetPosition(AxialCoord a)
	{
		position = a;
		Vector3 newPosition = AxialCoord.WorldPositionFromAxial (position);
		Vector3 deltaVectors = transform.position - newPosition;
		trailPosition.x += deltaVectors.x;
		trailPosition.y += deltaVectors.y;
		transform.position = newPosition;
	}
	public void ToggleInvisible()
	{
		Renderer tokenRenderer = GetComponent<Renderer> ();
		if (tokenRenderer != null) {
			tokenRenderer.enabled = ! tokenRenderer.enabled;
		}
	}
	public void SetCharacterName(Characters c)
	{
		if (characterTextures.Length > (int)c ) {
			SpriteRenderer tokenRenderer = GetComponent<SpriteRenderer> ();
			if (tokenRenderer != null && TokenLoader.CharacterTokens.ContainsKey(c.ToString())) {
				tokenRenderer.sprite = TokenLoader.CharacterTokens[c.ToString()];
			}
		}
		CharacterName = c.ToString ();
	}
	public void SetCharacterName(string s)
	{
		if (System.Enum.Parse (typeof(Characters), s) != null) {
			SpriteRenderer tokenRenderer = GetComponent<SpriteRenderer> ();
			if (tokenRenderer != null && TokenLoader.CharacterTokens.ContainsKey (s)) {
				tokenRenderer.sprite = TokenLoader.CharacterTokens [s];
				tokenRenderer.color = Color.white;
			} else
				SetCharacterName (Characters.Legacy.ToString());
		}
	}


	public override string ToString ()
	{
		return CharacterName;
	}

	public void UpdatePosition(AxialCoord deltaHex)
	{
		if (OnBeforeMove != null)
			OnBeforeMove ();
		position = AxialCoord.Hex_Add (position, deltaHex);
		Vector3 hexPos = AxialCoord.WorldPositionFromAxial (position);
		transform.position = hexPos;
		if (OnAfterMove != null)
			OnAfterMove ();
		//menuManager.setPositionText (position);
		//fcTransform.position = fcOldPos;
	}
	public void UpdatePosition()
	{
		Vector3 hexPos = AxialCoord.WorldPositionFromAxial (position);
		transform.position = hexPos;
		panelScript.UpdatePosition (position);
	}
	void UpdateElevation()
	{
		int elevation = Map.map.GetElevationForPosition (position);
	}
	void UpdatePanelText(Token t)
	{
		if ((statPanelCanvas != null)) {
			if (!statPanelCanvas.isActiveAndEnabled)
				statPanelCanvas.enabled = true;
		}
		string resultString = "LOS To Token " + t.CharacterName;
		resultString += MapMath.mapMath.LineOfSightTo (t.position, this.position, 6,20).ToString ();
		resultString += "\n Range: " + MapMath.mapMath.Rangeto (t.position, this.position).ToString ();
		//panelScript.UpdatePanelText (resultString);
		StartCoroutine ("ShutOffPanel");
	}
	IEnumerator ShutOffPanel()
	{
		yield return new WaitForSeconds (5);
		if ((statPanelCanvas != null)) {
			if (statPanelCanvas.isActiveAndEnabled)
				statPanelCanvas.enabled = false;
		}
	}

	}
public enum Characters{
	AbsoluteZero,
	Ambuscade,
	BaronBlade,
	Beacon,
	Bunker,
	CitizenDawn,
	Legacy,
	Proletariat,
	Ra,
	Tachyon,
	TheOperative,
	TheVisionary,
	TheWraith,
	Unity
}
//}

