using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HextureLoader : MonoBehaviour {
	public static Dictionary<string, Texture> Hextures;
	public static Dictionary<string, Sprite> HexSprites;
	public static Dictionary<string, int> HexElevations;
	// Use this for initialization
	void Awake () {
		Hextures = new Dictionary<string, Texture> ();
		HexElevations = new Dictionary<string, int> ();
		HexSprites = new Dictionary<string, Sprite> ();
		for (int i = 1; i < 9; i++) {
			Sprite[] megalopolis = Resources.LoadAll<Sprite>("Tile Sprite Sheets/M"+i.ToString());
			Sprite[] insulaPrimalis = Resources.LoadAll<Sprite>("Tile Sprite Sheets/IP"+i.ToString());
			for(int x = 0; x < 19; x++){
				HexSprites.Add ("M" + i.ToString () + "_" + CoordinateFromIndex(x).ToString (), megalopolis [x]);
				HexSprites.Add ("IP" + i.ToString () + "_" + CoordinateFromIndex(x).ToString (), insulaPrimalis[x]);
				HexElevations.Add ("M" + i.ToString () + "_" + CoordinateFromIndex(x).ToString (),ElevationFromTileAndIndex(i,x,true));
				HexElevations.Add ("IP" + i.ToString () + "_" + CoordinateFromIndex(x).ToString (),ElevationFromTileAndIndex(i,x,false));
			} 
		}
		Sprite[] blanksAndTransparents = Resources.LoadAll<Sprite> ("Tile Sprite Sheets/Blank And Transparent Hexes");
		for (int i = 1; i < 5; i++) {
			HexSprites.Add ("TransparentElevation" + i.ToString (), blanksAndTransparents[i+3]);
			HexSprites.Add ("BlankElevation" + i.ToString (), blanksAndTransparents[i-1]);
		}
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

	int ElevationFromTileAndIndex(int tile, int index, bool megalopolis)
	{
		if (megalopolis) {
			switch (tile) {
			case 1:
				{
					switch (index) {
					case 0:
						return 4;
					case 1:
						return 4;
					case 2:
						return 1;
					case 3:
						return 1;
					case 4:
						return 3;
					case 5:
						return 3;
					case 6:
						return 1;
					case 7:
						return 1;
					case 8:
						return 3;
					case 9:
						return 1;
					case 10:
						return 4;
					case 11:
						return 4;
					case 12:
						return 3;
					case 13:
						return 3;
					case 14:
						return 1;
					case 15:
						return 4;
					case 16:
						return 3;
					case 17:
						return 3;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 2:
				{
					switch (index) {
					case 0:
						return 4;
					case 1:
						return 4;
					case 2:
						return 1;
					case 3:
						return 1;
					case 4:
						return 3;
					case 5:
						return 1;
					case 6:
						return 1;
					case 7:
						return 1;
					case 8:
						return 3;
					case 9:
						return 1;
					case 10:
						return 1;
					case 11:
						return 3;
					case 12:
						return 1;
					case 13:
						return 3;
					case 14:
						return 1;
					case 15:
						return 1;
					case 16:
						return 3;
					case 17:
						return 4;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 3:
				{
					switch (index) {
					case 0:
						return 1;
					case 1:
						return 3;
					case 2:
						return 1;
					case 3:
						return 3;
					case 4:
						return 1;
					case 5:
						return 3;
					case 6:
						return 1;
					case 7:
						return 2;
					case 8:
						return 1;
					case 9:
						return 1;
					case 10:
						return 1;
					case 11:
						return 3;
					case 12:
						return 1;
					case 13:
						return 3;
					case 14:
						return 1;
					case 15:
						return 2;
					case 16:
						return 1;
					case 17:
						return 4;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 4:
				{
					switch (index) {
					case 0:
						return 3;
					case 1:
						return 2;
					case 2:
						return 1;
					case 3:
						return 1;
					case 4:
						return 3;
					case 5:
						return 3;
					case 6:
						return 1;
					case 7:
						return 1;
					case 8:
						return 1;
					case 9:
						return 3;
					case 10:
						return 3;
					case 11:
						return 1;
					case 12:
						return 1;
					case 13:
						return 1;
					case 14:
						return 1;
					case 15:
						return 2;
					case 16:
						return 2;
					case 17:
						return 1;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 5:
				{
					switch (index) {
					case 0:
						return 3;
					case 1:
						return 1;
					case 2:
						return 1;
					case 3:
						return 3;
					case 4:
						return 3;
					case 5:
						return 1;
					case 6:
						return 1;
					case 7:
						return 1;
					case 8:
						return 2;
					case 9:
						return 1;
					case 10:
						return 3;
					case 11:
						return 1;
					case 12:
						return 1;
					case 13:
						return 1;
					case 14:
						return 1;
					case 15:
						return 2;
					case 16:
						return 2;
					case 17:
						return 1;
					case 18:
						return 2;
					default:
						return 1;
					}
				}
			case 6:
				{
					switch (index) {
					case 0:
						return 1;
					case 1:
						return 1;
					case 2:
						return 1;
					case 3:
						return 2;
					case 4:
						return 3;
					case 5:
						return 2;
					case 6:
						return 1;
					case 7:
						return 1;
					case 8:
						return 1;
					case 9:
						return 3;
					case 10:
						return 1;
					case 11:
						return 3;
					case 12:
						return 3;
					case 13:
						return 1;
					case 14:
						return 1;
					case 15:
						return 1;
					case 16:
						return 1;
					case 17:
						return 1;
					case 18:
						return 2;
					default:
						return 1;
					}
				}
			case 7:
				{
					switch (index) {
					case 0:
						return 3;
					case 1:
						return 1;
					case 2:
						return 2;
					case 3:
						return 2;
					case 4:
						return 1;
					case 5:
						return 1;
					case 6:
						return 2;
					case 7:
						return 1;
					case 8:
						return 2;
					case 9:
						return 1;
					case 10:
						return 1;
					case 11:
						return 1;
					case 12:
						return 2;
					case 13:
						return 1;
					case 14:
						return 3;
					case 15:
						return 1;
					case 16:
						return 1;
					case 17:
						return 1;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 8:
				{
					switch (index) {
					case 0:
						return 1;
					case 1:
						return 1;
					case 2:
						return 1;
					case 3:
						return 1;
					case 4:
						return 1;
					case 5:
						return 1;
					case 6:
						return 1;
					case 7:
						return 4;
					case 8:
						return 1;
					case 9:
						return 4;
					case 10:
						return 4;
					case 11:
						return 1;
					case 12:
						return 3;
					case 13:
						return 1;
					case 14:
						return 2;
					case 15:
						return 1;
					case 16:
						return 1;
					case 17:
						return 1;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			default:
				return 1;
			}
		}
		else
		{
			switch (tile) {
			case 1:
				{
					switch (index) {
					case 0:
						return 2;
					case 1:
						return 2;
					case 2:
						return 2;
					case 3:
						return 2;
					case 4:
						return 1;
					case 5:
						return 2;
					case 6:
						return 1;
					case 7:
						return 1;
					case 8:
						return 1;
					case 9:
						return 1;
					case 10:
						return 2;
					case 11:
						return 1;
					case 12:
						return 1;
					case 13:
						return 1;
					case 14:
						return 1;
					case 15:
						return 2;
					case 16:
						return 1;
					case 17:
						return 1;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 2:
				{
					switch (index) {
					case 0:
						return 2;
					case 1:
						return 2;
					case 2:
						return 2;
					case 3:
						return 2;
					case 4:
						return 1;
					case 5:
						return 3;
					case 6:
						return 1;
					case 7:
						return 2;
					case 8:
						return 1;
					case 9:
						return 2;
					case 10:
						return 2;
					case 11:
						return 1;
					case 12:
						return 1;
					case 13:
						return 1;
					case 14:
						return 2;
					case 15:
						return 2;
					case 16:
						return 1;
					case 17:
						return 1;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 3:
				{
					switch (index) {
					case 0:
						return 2;
					case 1:
						return 2;
					case 2:
						return 3;
					case 3:
						return 2;
					case 4:
						return 2;
					case 5:
						return 2;
					case 6:
						return 1;
					case 7:
						return 2;
					case 8:
						return 1;
					case 9:
						return 1;
					case 10:
						return 2;
					case 11:
						return 1;
					case 12:
						return 1;
					case 13:
						return 1;
					case 14:
						return 1;
					case 15:
						return 2;
					case 16:
						return 1;
					case 17:
						return 1;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 4:
				{
					switch (index) {
					case 0:
						return 2;
					case 1:
						return 2;
					case 2:
						return 2;
					case 3:
						return 2;
					case 4:
						return 1;
					case 5:
						return 2;
					case 6:
						return 1;
					case 7:
						return 1;
					case 8:
						return 1;
					case 9:
						return 1;
					case 10:
						return 2;
					case 11:
						return 1;
					case 12:
						return 1;
					case 13:
						return 1;
					case 14:
						return 1;
					case 15:
						return 2;
					case 16:
						return 1;
					case 17:
						return 1;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 5:
				{
					switch (index) {
					case 0:
						return 2;
					case 1:
						return 2;
					case 2:
						return 2;
					case 3:
						return 2;
					case 4:
						return 1;
					case 5:
						return 2;
					case 6:
						return 1;
					case 7:
						return 1;
					case 8:
						return 1;
					case 9:
						return 2;
					case 10:
						return 2;
					case 11:
						return 2;
					case 12:
						return 1;
					case 13:
						return 1;
					case 14:
						return 2;
					case 15:
						return 2;
					case 16:
						return 1;
					case 17:
						return 1;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 6:
				{
					switch (index) {
					case 0:
						return 3;
					case 1:
						return 3;
					case 2:
						return 3;
					case 3:
						return 2;
					case 4:
						return 3;
					case 5:
						return 2;
					case 6:
						return 1;
					case 7:
						return 1;
					case 8:
						return 1;
					case 9:
						return 1;
					case 10:
						return 2;
					case 11:
						return 1;
					case 12:
						return 1;
					case 13:
						return 1;
					case 14:
						return 1;
					case 15:
						return 2;
					case 16:
						return 1;
					case 17:
						return 1;
					case 18:
						return 1;
					default:
						return 1;
					}
				}
			case 7:
				{
					return 1;
				}
			case 8:
				{
					switch (index) {
					case 0:
						return 4;
					case 1:
						return 4;
					case 2:
						return 4;
					case 3:
						return 4;
					case 4:
						return 3;
					case 5:
						return 4;
					case 6:
						return 3;
					case 7:
						return 3;
					case 8:
						return 4;
					case 9:
						return 3;
					case 10:
						return 4;
					case 11:
						return 3;
					case 12:
						return 3;
					case 13:
						return 4;
					case 14:
						return 3;
					case 15:
						return 4;
					case 16:
						return 4;
					case 17:
						return 4;
					case 18:
						return 4;
					default:
						return 4;
					}
				}
			default:
				return 1;
			}
		}
	}
}
