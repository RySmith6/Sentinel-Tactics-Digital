using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack {

	public int numberOfDice = 0;
	public bool[] autoMisses = new bool[]{false,false,false,false,false,false};
	public int range = 6;
	public int radius = -1;
	public bool vertex = false;
	public bool melee = false;
	public int numberOfTargets = 1;
	public bool requiresLOS = true;
}
