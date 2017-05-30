using UnityEngine;
using System.Collections;

public class GameSettingsObject : MonoBehaviour {
	public static GameSettingsObject gso;
	public delegate void MapNameLoaded(string mapName);
	public static event MapNameLoaded OnMapNameLoad;


	public int numberOfTeams;
	public int[] numberOfPlayersPerTeam;
	public string mapName;
	const int maxPlayers = 10;
	void Awake(){
		if (gso == null) {
			DontDestroyOnLoad(gameObject);
			gso=this;
			numberOfTeams=1;

		} else if(gso!=this) {
			Destroy(gameObject);
		}
		numberOfTeams = 2;
		numberOfPlayersPerTeam = new int[2];
		numberOfPlayersPerTeam [0] = 3;
		numberOfPlayersPerTeam [1] = 3;
	}
	// Use this for initialization
	void Start () {
		numberOfTeams = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ConfirmSettings()
	{

	}
	public void SetMapName(string newMapName)
	{
		mapName = newMapName;
		if (OnMapNameLoad != null)
			OnMapNameLoad (newMapName);
	}
}
//public class PlayerInfo{
//	public Characters character;
//	public int turnOrder;
//	public int team;
//	public string controllingUser;
//}
