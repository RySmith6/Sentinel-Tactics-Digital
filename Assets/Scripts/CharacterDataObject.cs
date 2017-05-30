using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterDataObject {

	public string characterName= "";
	public CharacterPanel panel = new CharacterPanel();
	public List<Card> cards = new List<Card>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
