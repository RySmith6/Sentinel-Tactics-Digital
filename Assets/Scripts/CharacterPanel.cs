using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterPanel{

	public int startingHealth = 0;
	public int actions = 0;
	public int defense = 0;
	public int movement = 0;

	public List<Ability> abilities = new List<Ability>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
