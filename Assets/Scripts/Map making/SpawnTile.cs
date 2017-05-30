using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		List<GameObject> spriteList = new List<GameObject> ();
		for (int i = 1; i < 9; i++) {
			Sprite[] megalopolis = Resources.LoadAll<Sprite>("Tile Sprite Sheets/M"+i.ToString());
			for(int x = 0; x < 19; x++){
				GameObject gameObject = new GameObject ("M" + i.ToString () + "_" + x.ToString ());
				gameObject.AddComponent<SpriteRenderer> ();
				SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer> ();
				sr.sprite = megalopolis [x];
				gameObject.transform.position = PositionFromSpriteIndex (x);
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	Vector2 PositionFromSpriteIndex(int index)
	{
		return AxialCoord.TwoDWorldPosFromAxial (CoordinateFromIndex (index));
	}
	AxialCoord CoordinateFromIndex(int index)
	{
		switch (index) {
		case 0:
			return new AxialCoord (0,-2);
		case 1:
			return new AxialCoord (-1,-1);
		case 2:
			return new AxialCoord (1,-2);
		case 3:
			return new AxialCoord (-2,0);
		case 4:
			return new AxialCoord (0,-1);
		case 5:
			return new AxialCoord (2,-2);
		case 6:
			return new AxialCoord (-1,0);
		case 7:
			return new AxialCoord (1,-1);
		case 8:
			return new AxialCoord (-2,1);
		case 9:
			return new AxialCoord (0,0);
		case 10:
			return new AxialCoord (2,-1);
		case 11:
			return new AxialCoord (-1,1);
		case 12:
			return new AxialCoord (1,0);
		case 13:
			return new AxialCoord (-2,2);
		case 14:
			return new AxialCoord (0,1);
		case 15:
			return new AxialCoord (2,0);
		case 16:
			return new AxialCoord (-1,2);
		case 17:
			return new AxialCoord (1,1);
		case 18:
			return new AxialCoord (0,2);

		default:
			return new AxialCoord (0, 0);
		}
	}
}


//public struct AxialCoord{
//	public int q;
//	public int r;
//	private static AxialCoord[] directions =  {		
//		new AxialCoord(1,  0), new AxialCoord(1, -1), new AxialCoord( 0, -1),
//		new AxialCoord(-1,  0), new AxialCoord(-1, 1),new AxialCoord( 0, 1)
//	};
//	public static AxialCoord origin = new AxialCoord (0, 0);
//
//	public AxialCoord(int qCoord, int rCoord)
//	{
//		q = qCoord;
//		r = rCoord;
//	}
//	public static Vector2 TwoDWorldPosFromAxial(AxialCoord position)
//	{
//		return  new Vector2 ((.815f * (1.5f) * position.q)
//			, -(.815f * Mathf.Sqrt (3) * (position.r + position.q * 0.5f)));
//	}
//}