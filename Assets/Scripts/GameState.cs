using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState:MonoBehaviour {

	private Dictionary<string, AxialCoord> targetLocations;
	private Dictionary<string, AxialCoord> tokenLocations;

	//private 

	public static GameState gameState;

	private static GameState instance {
		get {
			if (!gameState) {
				gameState = FindObjectOfType (typeof(GameState)) as GameState;
				if (!gameState) {
					Debug.LogError ("GameState does not exist on an active GameObject");
				} else {
					gameState.Init ();
				}
			}
			return gameState;
		}
	}
	public void Init()
	{
		targetLocations = new Dictionary<string, AxialCoord> ();
		tokenLocations = new Dictionary<string, AxialCoord> ();
	}

	public void AddTargetToMap(string targetName, AxialCoord position)
	{
		this.targetLocations.Add (targetName, position);
	}

	public void AddTokenToMap(string tokenName, AxialCoord position)
	{
		this.tokenLocations.Add (tokenName, position);
	}

	public void MoveTargetOnMap(string targetName, AxialCoord offset)
	{
		AxialCoord position = AxialCoord.origin;
		if (this.targetLocations.TryGetValue (targetName, out position)) {
			this.targetLocations [targetName] = position + offset;
		}
	}

	public void MoveTokenOnMap(string tokenName, AxialCoord offset)
	{
		AxialCoord position = AxialCoord.origin;
		if (this.tokenLocations.TryGetValue (tokenName, out position)) {
			this.tokenLocations [tokenName] = position + offset;
		}
	}

	public void RemoveTargetFromMap(string targetName)
	{
		AxialCoord position = AxialCoord.origin;
		if (this.targetLocations.TryGetValue (targetName, out position)) {
			this.targetLocations.Remove (targetName);
		}
	}
	public void RemoveTokenFromMap(string tokenName)
	{
		AxialCoord position = AxialCoord.origin;
		if (this.tokenLocations.TryGetValue (tokenName, out position)) {
			this.tokenLocations.Remove (tokenName);
		}
	}
}
