using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card {
	public string title = "DEFAULT CARD TITLE";
	public List<Ability> abilities = new List<Ability>(){new Ability()};
	public string flavorText = "DEFAULT CARD FLAVOR TEXT -UNITY, META-CODING #1";

}
