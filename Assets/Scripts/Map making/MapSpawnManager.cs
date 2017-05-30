using UnityEngine;
using System.Collections;

public class MapSpawnManager : MonoBehaviour
{
	public static MapSpawnManager mapSpawner;
	public Map gameMap = Map.map;
	// Use this for initialization
	void Awake ()
	{
		if (mapSpawner == null) {
			DontDestroyOnLoad(gameObject);
			mapSpawner=this;
		} else if(mapSpawner!=this) {
			Destroy(gameObject);
		}
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void WipeAndSpawnAt(AxialCoord origin)
	{
		gameMap.WipeMapHexes ();
		 SpawnTileAtAxial (origin);
	}
	public void SpawnTileAtAxial (AxialCoord origin) //int q, int r)
	{
		 SpawnRandomHelicalAtAxial (origin, 3); //new Vector3 (HexClass.radius * 1.5f * origin.q, HexClass.radius * Mathf.Sqrt (3) * (origin.r + (float)origin.q / 2));
	}
    public void SpawnRandomHelicalAtOrigin(int radius)
    {
        gameMap.WipeMapHexes();
        SpawnRandomHelicalAtAxial(AxialCoord.origin, radius);
    }
	public void SpawnRandomHelicalAtAxial (AxialCoord origin, int radius) //int q, int r)
	{
		//Center hex
		gameMap.AddHex (origin, (int)(UnityEngine.Random.value * 3)+1);
		//x<radius, in the case of the tile, four
		for (int x =1; x<radius; x++) {
			AxialCoord hex = AxialCoord.Hex_Add(origin, AxialCoord.Hex_Scale(AxialCoord.hex_direction(4),x));
			//in each cardinal direction, i 
			
			for(int i = 0; i<6; i++)
			{
				//and adding additional neighbors for larger rings
				for(int a =0; a<x;a++)
				{
					gameMap.AddHex(hex,
					               (int)(UnityEngine.Random.value * 4)+1);
					hex = AxialCoord.hex_neighbor(hex, i);
				}
			}
		}
	}
    public void SpawnSmoothHelicalAtOrigin(int radius)
    {
        gameMap.WipeMapHexes();
        SpawnSmoothHelicalAtAxial(AxialCoord.origin, radius);
    }
	public void SpawnSmoothHelicalAtAxial (AxialCoord origin, int radius) //int q, int r)
	{
		gameMap.AddHex (origin, (int)(UnityEngine.Random.value * 3)+1);
		for (int x =1; x<radius; x++) {
			AxialCoord hex = AxialCoord.Hex_Add(origin, AxialCoord.Hex_Scale(AxialCoord.hex_direction(4),x));
			for(int i = 0; i<6; i++)
			{
				//and adding additional neighbors for larger rings
				for(int a =0; a<x;a++)
				{
					gameMap.AddHex(hex, MapMath.mapMath.SmoothHeightFromNeighbors(hex));
					hex = AxialCoord.hex_neighbor(hex, i);
				}
			}
		}
	}
	public void SpawnMegalopolisTileAtAxial(AxialCoord origin, int tileNumber)
	{
		string tileHexName = "";
		AxialCoord[] tileCoords = MapMath.PositionsInRadiusFrom (AxialCoord.origin, 2).ToArray();
		foreach (AxialCoord a in tileCoords) {
			tileHexName = "M"+tileNumber.ToString()+"_"+a.ToString();

					gameMap.SetOrCreateHex(AxialCoord.Hex_Add(a,origin), tileHexName);
		}
	}
	public void SpawnIPTileAtAxial(AxialCoord origin, int tileNumber)
	{
		string tileHexName = "";
		AxialCoord[] tileCoords = MapMath.PositionsInRadiusFrom (AxialCoord.origin, 2).ToArray();
		foreach (AxialCoord a in tileCoords) {
			tileHexName = "IP"+tileNumber.ToString()+"_"+a.ToString();
			
			gameMap.SetOrCreateHex(AxialCoord.Hex_Add(a,origin), tileHexName);
		}
	}

}

