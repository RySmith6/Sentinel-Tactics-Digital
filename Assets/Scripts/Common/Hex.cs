using UnityEngine;
using System.Collections;


public class Hex : MonoBehaviour
{
	public delegate void HexChangedHeight(Hex h);
	public event HexChangedHeight OnElevationChange;
	public delegate void HexChangedPosition(Hex h);
	public event HexChangedPosition OnPositionChange;
	//private HexClass hexData;
	
	public static float radius = 0.861f;
	int elevation;
	public AxialCoord position;
	public int rotations;
	public bool flipped=false;
	Outline hasOutline;
	public Outline HasOutline{ get{return hasOutline; } set{ this.hasOutline = value; SetOutlineRendererToColor ();  } }
	Color hazardColor = Color.red;
	public Color HazardColor{ get{return hazardColor; } set { this.outlineRenderer.color = value; hazardColor = value; } }
	public SpriteRenderer outlineRenderer;
	SpriteRenderer spriteRenderer;
	Sprite sprite;
	public string spriteName;
	public bool underEdit;
	public bool changed;
	public Visibility visibility;
	[SerializeField]
	ParticleSystem visibleCloud;
	[SerializeField]
	ParticleSystem invisibleCloud;


	// Use this for initialization
	void Awake ()
	{
		//hexData = null;
		elevation = 1;
		position = AxialCoord.origin;
		hasOutline = Outline.none;
		underEdit = false;
		rotations = 0;
		DontDestroyOnLoad (gameObject);
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Start()
	{
		position = AxialCoord.AxialFromWorld (transform.position);
		visibility = Visibility.visible;
		Vector2[] points = new Vector2[6];

		for (int i =0; i<6; i++) {
			int angle_deg = (60 * i);
			float angle_rad = Mathf.PI / 180 * angle_deg;
			points[i] = new Vector3 ( 1 * Mathf.Cos (angle_rad),
			                             1 * Mathf.Sin (angle_rad));
		}
	}
	// Update is called once per frame
	void Update ()
	{

	
	}
	public Outline ToggleOutline()
	{
		if (hasOutline == Outline.hazard) {
			hasOutline = Outline.none;
			outlineRenderer.color = Color.black;
		} else if (hasOutline == Outline.cover) {
			hasOutline = Outline.hazard;
			outlineRenderer.color = Color.red;
		} else if (hasOutline == Outline.none) { 
			hasOutline = Outline.cover;
			outlineRenderer.color = new Color (0, 0.5f, 0);
		}
		return hasOutline;
	}
	public void SetOutlineRendererToColor()
	{
		if (hasOutline == Outline.hazard) {
			outlineRenderer.color = Color.red;
		} else if (hasOutline == Outline.cover) {
			outlineRenderer.color = new Color (0, 0.5f, 0);
		} else if (hasOutline == Outline.none) { 
			outlineRenderer.color = Color.black;
		}
		else if (hasOutline == Outline.blue){
			outlineRenderer.color = Color.blue;
		}
	}
	public bool IsUnderEdit()
	{
		return underEdit;
	}
	public void SetUnderEdit(bool newUnderEdit)
	{
		underEdit = newUnderEdit;
		if (underEdit)
			spriteRenderer.color = Color.gray;
		else
			spriteRenderer.color = Color.white;
	}
	public void SetPosition(AxialCoord a, bool sunken=false, bool preview = false)
	{
		position = a;
		if (sunken)
			transform.position = AxialCoord.SunkenWorldPositionFromAxial (position);
		else if (preview) {
			transform.position= AxialCoord.PreviewWorldPositionFromAxial(position);
		}
		else
			transform.position = AxialCoord.FlatWorldPositionFromAxial (position);
		if (OnPositionChange != null)
			OnPositionChange (this);
	}
	public void SetTransparent()
	{
		if (HextureLoader.HexSprites.ContainsKey ("TransparentElevation" + elevation.ToString ())) {
			SetSprite(HextureLoader.HexSprites["TransparentElevation" + elevation.ToString ()],"TransparentElevation" + elevation.ToString ());
		}
	}
	public bool ToggleEditState()
	{
		underEdit = !underEdit;
		if (underEdit)
			spriteRenderer.color = Color.gray;
		else
			spriteRenderer.color = Color.white;
		return underEdit;
 	}
	public AxialCoord GetPosition()
	{
		return position;
	}
	public Vector3 GetWorldPosition()
	{
		return AxialCoord.FlatWorldPositionFromAxial (position);
	}
	public int GetElevation()
	{
		return elevation;
	}
	public void SetElevation(int e)
	{ 
		if((e>0&&e<=4))
		{
			elevation=e;
			SetBlankForElevation (e);
			if(OnElevationChange!=null)
			OnElevationChange(this);

		}
	}
	public void SetBlankForElevation(int elevation)
	{
		this.elevation = elevation;
		this.SetBlank ();
	}
	public void SetBlank()
	{
		if (HextureLoader.HexSprites.ContainsKey ("BlankElevation" + elevation.ToString ())) {
			SetSprite(HextureLoader.HexSprites["BlankElevation" + elevation.ToString ()], "BlankElevation" + elevation.ToString ());
		}
	}
	public void SetSprite(Sprite s, string name)
	{
		this.spriteRenderer.sprite = s;
		this.spriteName = name;
	}
	public void SetNamedFromHextures(string s)
	{
		if (s!=null && s != spriteName) {
			if (HextureLoader.HexSprites.ContainsKey (s)) {
				SetSprite (HextureLoader.HexSprites [s], s);
			}
		}
	}
	public bool IsNeighbor(Hex h)
	{
		return AxialCoord.DistanceInHexes (position, h.GetPosition ()) < 2;
	}
	public bool IsAlone(Hex[] potentialNeighbors)
	{
		bool friendly = false;
		foreach (Hex h in potentialNeighbors)
			friendly |= IsNeighbor (h);
		return !friendly;
	}
	public void FlipFace()
	{
		spriteRenderer.flipX = !spriteRenderer.flipX;
		if (rotations % 3 != 0) {
			int rot = rotations;
			Rotate (6 - rotations);
			SetRotations ((6-rot));
		}
		flipped = !flipped;	
	}
	public override string ToString ()
	{
		return position.ToString () + spriteName + hasOutline.ToString();
	}
	public void Rotate(int r=1)
	{
		transform.Rotate(0, 0, 60*r);
		rotations = (rotations + r) % 6;
	}
	public void SetRotations(int i)
	{
		rotations = 0;
		for(int x=0; x<i; x++)
		Rotate ();
	}
	public void displayVisibility()
	{
		var visMain = visibleCloud.main;
		var invisMain = invisibleCloud.main;
		if (visibility == Visibility.visible) 
			visMain.startColor = Color.white;
		if (visibility == Visibility.wall) 
			visMain.startColor = Color.cyan;
		if (visibility == Visibility.valley) 
			invisMain.startColor = Color.grey;
		if (visibility == Visibility.cover) 
			invisMain.startColor = Color.green;
		if (visibility == Visibility.invisible) 
			invisMain.startColor = Color.black;
		if ((visibility == Visibility.visible) || (visibility == Visibility.wall))
			visibleCloud.Play ();
		else
			invisibleCloud.Play ();
	}
	public void HideVisibility()
	{
		if (visibleCloud.isPlaying)
			visibleCloud.Stop ();
		if (invisibleCloud.isPlaying)
			invisibleCloud.Stop ();
	}

}

public enum Outline
{
	none,
	cover,
	hazard,
	blue
}
public enum Visibility
{
	visible,
	cover,
	wall,
	valley,
	invisible
}
